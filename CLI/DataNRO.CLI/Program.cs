﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DataNRO.Interfaces;
using Newtonsoft.Json;
using Starksoft.Net.Proxy;
using static DataNRO.GameData;

namespace DataNRO.CLI
{
    internal class Program
    {
        static Random random = new Random();
        static string proxyData = "";
        static int[] overwriteIconIDs = new int[0];

        [DllImport("msvcrt.dll")]
        public static extern int system(string cmd);

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
#if DEBUG
            Environment.CurrentDirectory = "D:\\Working Repositories\\DataNRO\\Output";
#endif
            new Thread(FailoverThread) { IsBackground = true }.Start();
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            proxyData = Environment.GetEnvironmentVariable("PROXY");
            string overwriteIconsEnv = Environment.GetEnvironmentVariable("OVERWRITE_ICONS");
            if (!string.IsNullOrEmpty(overwriteIconsEnv))
                overwriteIconIDs = overwriteIconsEnv.Split(',').Select(int.Parse).ToArray();
#if DEBUG
            Console.Write("DATA: ");
            string data = Console.ReadLine();
#else
            string data = Environment.GetEnvironmentVariable("DATA");
#endif
            string[] datas = data.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string d in datas)
            {
                try
                {
                    LoginAndGetData(d);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
#if DEBUG
            system("pause");
#endif
            Environment.Exit(0);
        }

        static void FailoverThread()
        {
            Thread.Sleep(1000 * 60 * 60 * 2);
            Console.WriteLine("DataNRO has been running for 2 hours, exiting...");
            Environment.Exit(1);
        }

        static void LoginAndGetData(string data)
        {
            string[] arr = data.Split('|');
            string type = arr[0];
            string host = arr[1];
            ushort port = ushort.Parse(arr[2]);
            string unregisteredUser = arr[3];
            string account = arr[4];
            string password = arr[5];
            bool requestAndSaveIcons = bool.Parse(arr[6]);
            string folderName = arr[7];
            string dataPath = $"Data\\{type}\\{folderName}";
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);
            ISession session;
            try
            {
                Console.WriteLine($"Creating session type \"{type}\" from assembly \"DataNRO.{type}.dll\"...");
                Assembly assembly = Assembly.LoadFrom($"DataNRO.{type}.dll");
                Console.WriteLine($"Loaded assembly: {assembly.FullName}");
                Console.WriteLine($"Assembly name: {assembly.GetCustomAttribute<AssemblyTitleAttribute>().Title}");
                Console.WriteLine($"Assembly description: {assembly.GetCustomAttribute<AssemblyDescriptionAttribute>().Description}");
                Console.WriteLine($"Assembly company: {assembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company}");
                Console.WriteLine($"Assembly copyright: {assembly.GetCustomAttribute<AssemblyCopyrightAttribute>().Copyright}");
                string sessionTypeName = $"DataNRO.{type}.{type}Session";
                Console.WriteLine($"Creating session type: {sessionTypeName}...");
                session = (ISession)Activator.CreateInstance(assembly.GetType(sessionTypeName), new object[] { host, port });
                Console.WriteLine($"Session type \"{type}\" has been created successfully!");
            }
            catch
            {
                Console.WriteLine($"The main assembly for server type \"{type}\" (DataNRO.{type}.dll) does not exist!");
                return;
            }
            session.Data.Path = dataPath;
            session.Data.SaveIcon = requestAndSaveIcons;
            session.Data.OverwriteIconIDs = overwriteIconIDs;
            Console.WriteLine($"Connecting to {session.Host}:{session.Port}...");
            if (!TryConnect(session))
                return;
            Console.WriteLine("Connected successfully!");
            IMessageWriter writer = session.MessageWriter;
            writer.SetClientType();
            Thread.Sleep(1500);
            writer.ImageSource();
            Thread.Sleep(1000);
            if (session.Data.SaveIcon)
            {
                Console.WriteLine($"[{session.Host}:{session.Port}] Downloading data...");
                writer.GetResource(1);
                do
                {
                    Thread.Sleep(1000);
                }
                while (!session.Data.AllResourceLoaded);
            }
            if (!string.IsNullOrEmpty(unregisteredUser))
            {
                Console.WriteLine($"[{session.Host}:{session.Port}] Login as {unregisteredUser.Substring(0, 4)}{new string('*', unregisteredUser.Length - 4)}");
                writer.Login(unregisteredUser, "a", 1);
            }
            else
            {
                Console.WriteLine($"[{session.Host}:{session.Port}] Login as {new string('*', account.Length)}");
                writer.Login(account, password, 0);
            }
            Thread.Sleep(1000);
            writer.UpdateMap();
            writer.UpdateSkill();
            writer.UpdateItem();
            writer.UpdateData();
            Thread.Sleep(1000);
            writer.ClientOk();
            writer.FinishUpdate();
            Thread.Sleep(4000);
            int count = 0;
            Location location;
            do
            {
                location = session.Player.location;
                count++;
                if (count >= 20)
                {
                    Console.WriteLine($"[{session.Host}:{session.Port}] Failed to get the player's location!");
                    return;
                }
                Thread.Sleep(1000);
            }
            while (location == null || string.IsNullOrEmpty(location.mapName));
            writer.FinishLoadMap();
            Console.WriteLine($"[{session.Host}:{session.Port}] Current map: {location.mapName} [{location.mapId}], zone {location.zoneId}");
            Thread.Sleep(2000);
            if (session.Data.SaveIcon)
            {
                if (!RequestIcons(session))
                  return;
                RequestMobsImg(session);
            }
            TryGoOutsideIfAtHome(session);
            Console.WriteLine($"[{session.Host}:{session.Port}] Disconnect from {session.Host}:{session.Port} in 10s...");
            writer.Chat("DataNRO by ElectroHeavenVN");
            Thread.Sleep(5000);
            writer.Chat("GitHub dot com slash ElectroHeavenVN slash DataNRO");
            Thread.Sleep(5000);
            session.Disconnect();

            if (session.Data.SaveIcon)
                ProcessImages(session);

            Console.WriteLine($"[{session.Host}:{session.Port}] Writing data to {session.Data.Path}\\...");
            Formatting formatting = Formatting.Indented;
            File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.Maps)}.json", JsonConvert.SerializeObject(session.Data.Maps, formatting));
            File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.NpcTemplates)}.json", JsonConvert.SerializeObject(session.Data.NpcTemplates, formatting));
            File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.MobTemplates)}.json", JsonConvert.SerializeObject(session.Data.MobTemplates, formatting));
            File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.ItemOptionTemplates)}.json", JsonConvert.SerializeObject(session.Data.ItemOptionTemplates, formatting));
            File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.NClasses)}.json", JsonConvert.SerializeObject(session.Data.NClasses, formatting));
            File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.ItemTemplates)}.json", JsonConvert.SerializeObject(session.Data.ItemTemplates, formatting));
            File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.Parts)}.json", JsonConvert.SerializeObject(session.Data.Parts, formatting));
            //if (session.Data.SaveIcon)
            //    File.WriteAllText($"{Path.GetDirectoryName(session.Data.Path)}\\{nameof(GameData.MobTemplateEffectData)}.json", JsonConvert.SerializeObject(session.Data.MobTemplateEffectData, formatting));
            File.WriteAllText($"{session.Data.Path}\\LastUpdated", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
            Thread.Sleep(3000);
            session.FileWriter.DeleteTempFiles();
            session.Dispose();
        }

        static void ProcessImages(ISession session)
        {
            Console.WriteLine($"[{session.Host}:{session.Port}] Splitting images...");
            List<Bitmap> smallImages = new List<Bitmap>();
            for (int i = 0; File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\BigIcons\\Big{i}.png"); i++)
                smallImages.Add(new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\BigIcons\\Big{i}.png"));
            for (int id = 0; id < session.Data.SmallImg.Length; id++)
            {
                if (!session.Data.CanOverwriteIcon(id) && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{id}.png"))
                    continue;
                int[] smallImg = session.Data.SmallImg[id];
                int imgBigIndex = smallImg[0];
                if (imgBigIndex < 0 || imgBigIndex >= smallImages.Count || smallImages[imgBigIndex] == null)
                    continue;
                if (smallImg[1] >= 256 || smallImg[2] >= 256 || smallImg[3] >= 256 || smallImg[4] >= 256)
                    continue;
                int x = smallImg[1] * session.Data.ZoomLevel;
                int y = smallImg[2] * session.Data.ZoomLevel;
                int width = smallImg[3] * session.Data.ZoomLevel;
                int height = smallImg[4] * session.Data.ZoomLevel;
                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.DrawImage(smallImages[imgBigIndex], new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                    }
                    bitmap.Save($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{id}.png");
                }
            }

            Console.WriteLine($"[{session.Host}:{session.Port}] Combining NPC images...");
            for (int i = 0; i < session.Data.NpcTemplates.Length; i++)
            {
                NpcTemplate npc = session.Data.NpcTemplates[i];
                if (Constants.EXCLUDED_NPCS.Contains(npc.npcTemplateId))
                    continue;
                Part partHead = npc.headId == -1 ? null : session.Data.Parts[npc.headId];
                Part partBody = npc.bodyId == -1 ? null : session.Data.Parts[npc.bodyId];
                Part partLeg = npc.legId == -1 ? null : session.Data.Parts[npc.legId];
                if (partHead == null && partBody == null && partLeg == null)
                    continue;
                Bitmap imgHead = null, imgBody = null, imgLeg = null;
                if (partHead != null)
                    imgHead = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partHead.pi[0].id}.png");
                if (partBody != null)
                    imgBody = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partBody.pi[1].id}.png");
                if (partLeg != null)
                    imgLeg = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partLeg.pi[1].id}.png");
                int maxWidth = 0, maxHeight = 0;
                if (imgHead != null)
                {
                    maxWidth = imgHead.Width;
                    maxHeight = imgHead.Height;
                }
                if (imgBody != null)
                {
                    maxWidth += imgBody.Width;
                    maxHeight += imgBody.Height;
                }
                if (imgLeg != null)
                {
                    maxWidth += imgLeg.Width;
                    maxHeight += imgLeg.Height;
                }
                using (Bitmap imgNpc = new Bitmap(maxWidth * 3, maxHeight * 3))
                {
                    using (Graphics g = Graphics.FromImage(imgNpc))
                    {
                        g.FillRectangle(Brushes.Transparent, 0, 0, imgNpc.Width, imgNpc.Height);
                        float cx = imgNpc.Width / 2f / session.Data.ZoomLevel;
                        float cy = imgNpc.Height / 2f / session.Data.ZoomLevel;
                        if (imgHead != null)
                            g.DrawImage(imgHead, (cx + -13 + partHead.pi[0].dx) * session.Data.ZoomLevel, (cy - 34 + partHead.pi[0].dy) * session.Data.ZoomLevel, imgHead.Width, imgHead.Height);
                        if (imgLeg != null)
                            g.DrawImage(imgLeg, (cx + -8 + partLeg.pi[1].dx) * session.Data.ZoomLevel, (cy - 10 + partLeg.pi[1].dy) * session.Data.ZoomLevel, imgLeg.Width, imgLeg.Height);
                        if (imgBody != null)
                            g.DrawImage(imgBody, (cx + -9 + partBody.pi[1].dx) * session.Data.ZoomLevel, (cy - 16 + partBody.pi[1].dy) * session.Data.ZoomLevel, imgBody.Width, imgBody.Height);
                    }
                    string path = $"{Path.GetDirectoryName(session.Data.Path)}\\NPCs";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    Bitmap croppedImg = CropToContent(imgNpc);
                    using (Graphics g = Graphics.FromImage(croppedImg))
                    {
                        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                        Font font = new Font("Arial", 8);
                        g.DrawString("© ElectroHeavenVN", font, Brushes.Black, 0, 30);
                    }
                    croppedImg.Save($"{path}\\{npc.npcTemplateId}.png");
                    croppedImg.Dispose();
                    imgNpc.Dispose();
                }
                imgHead?.Dispose();
                imgBody?.Dispose();
                imgLeg?.Dispose();
            }

            Console.WriteLine($"[{session.Host}:{session.Port}] Combining monster images...");
            for (int templateId = 0; templateId < session.Data.MobTemplates.Length; templateId++)
            {
                MobTemplate template = session.Data.MobTemplates[templateId];
                EffectData effectData = session.Data.MobTemplateEffectData[templateId];
                if (effectData.frame == null || effectData.frame.Length == 0)
                    continue;
                Frame frame = effectData.frame[0];
                Bitmap mobImg = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\MobImg\\{templateId}.png");
                using (Bitmap monster = new Bitmap(mobImg.Width * 3, mobImg.Height * 3))
                {
                    int x = (int)(monster.Width / 2f / session.Data.ZoomLevel);
                    int y = (int)(monster.Height / 2f / session.Data.ZoomLevel);
                    using (Graphics g = Graphics.FromImage(monster))
                    {
                        for (int i = 0; i < frame.dx.Length; i++)
                        {
                            ImageInfo imageInfo = effectData.imgInfo.FirstOrDefault(img => img.id == frame.idImg[i]);
                            if (imageInfo == null)
                                continue;
                            try
                            {
                                g.DrawImage(mobImg, new Rectangle((x + frame.dx[i]) * session.Data.ZoomLevel, (y + frame.dy[i]) * session.Data.ZoomLevel, imageInfo.w * session.Data.ZoomLevel, imageInfo.h * session.Data.ZoomLevel), new Rectangle(imageInfo.x0 * session.Data.ZoomLevel, imageInfo.y0 * session.Data.ZoomLevel, imageInfo.w * session.Data.ZoomLevel, imageInfo.h * session.Data.ZoomLevel), GraphicsUnit.Pixel);
                            }
                            catch { }
                        }
                    }
                    string path = $"{Path.GetDirectoryName(session.Data.Path)}\\Monsters";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    Bitmap croppedImg = CropToContent(monster);
                    using (Graphics g = Graphics.FromImage(croppedImg))
                    {
                        g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                        Font font = new Font("Arial", 12);
                        g.DrawString("© ElectroHeavenVN", font, Brushes.Black, 0, 30);
                    }
                    croppedImg.Save($"{path}\\{templateId}.png");
                    croppedImg.Dispose();
                }
                mobImg.Dispose();
            }
        }

        static bool RequestIcons(ISession session)
        {
            IMessageWriter writer = session.MessageWriter;
            List<int> requestedIcons = new List<int>();
            int count = 0;
            void RequestPartIcon(Part part, int index)
            {
                if (part == null)
                    return;
                int id = part.pi[index].id;
                if (requestedIcons.Contains(id))
                    return;
                if (!session.Data.CanOverwriteIcon(id) && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{id}.png"))
                    return;
                writer.RequestIcon(id);
                requestedIcons.Add(id);
                Thread.Sleep(1000 + random.Next(-200, 201));
                count++;
                if (count >= 10)
                {
                    writer.RequestChangeZone(session.Player.location.zoneId);
                    count = 0;
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {requestedIcons.Count} icons");
                }
            }

            writer.UpdateData();
            while (session.Data.Parts == null)
            {
                count++;
                if (count >= 10)
                {
                    Console.WriteLine($"[{session.Host}:{session.Port}] Get parts failed!");
                    return false;
                }
                Thread.Sleep(1000);
            }
            //item
            count = 0;
            while (session.Data.ItemTemplates == null)
            {
                Thread.Sleep(1000 + random.Next(-200, 201));
                count++;
                if (count >= 10)
                {
                    writer.RequestChangeZone(session.Player.location.zoneId);
                    count = 0;
                }
            }
            List<ItemTemplate> items = session.Data.ItemTemplates;
            while (items.Count > 0)
            {
                ItemTemplate item = items[random.Next(0, items.Count)];
                items.Remove(item);
                if (requestedIcons.Contains(item.icon))
                    continue;
                if (!session.Data.CanOverwriteIcon(item.icon) && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{item.icon}.png"))
                    continue;
                writer.RequestIcon(item.icon);
                requestedIcons.Add(item.icon);
                Thread.Sleep(1000 + random.Next(-200, 201));
                count++;
                if (count >= 10)
                {
                    writer.RequestChangeZone(session.Player.location.zoneId);
                    count = 0;
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {requestedIcons.Count} icons");
                }
            }
            //npc
            //while (session.Data.NpcTemplates == null)
            //{
            //    Thread.Sleep(1000 + random.Next(-200, 201));
            //    count++;
            //    if (count >= 10)
            //    {
            //        writer.RequestChangeZone(session.Player.location.zoneId);
            //        count = 0;
            //    }
            //}
            //foreach (NpcTemplate npc in session.Data.NpcTemplates)
            //{
            //    Part partHead = npc.headId == -1 ? null : session.Data.Parts[npc.headId];
            //    Part partBody = npc.bodyId == -1 ? null : session.Data.Parts[npc.bodyId];
            //    Part partLeg = npc.legId == -1 ? null : session.Data.Parts[npc.legId];
            //    RequestPartIcon(partHead, 0);
            //    RequestPartIcon(partBody, 1);
            //    RequestPartIcon(partLeg, 1);
            //}
            //parts
            List<Part> parts = new List<Part>(session.Data.Parts);
            while (parts.Count > 0)
            {
                Part part = parts[random.Next(0, parts.Count)];
                parts.Remove(part);
                int index = 0;
                if (part.type == 1 || part.type == 2) //body, leg
                    index = 1;
                if (part.pi[index].id <= 0)
                    continue;
                RequestPartIcon(part, index);
            }
            //skills
            while (session.Data.NClasses == null)
            {
                Thread.Sleep(1000 + random.Next(-200, 201));
                count++;
                if (count >= 10)
                {
                    writer.RequestChangeZone(session.Player.location.zoneId);
                    count = 0;
                }
            }
            foreach (NClass nClass in session.Data.NClasses)
            {
                foreach (SkillTemplate skillTemplate in nClass.skillTemplates)
                {
                    if (requestedIcons.Contains(skillTemplate.icon))
                        continue;
                    if (!session.Data.CanOverwriteIcon(skillTemplate.icon) && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{skillTemplate.icon}.png"))
                        continue;
                    writer.RequestIcon(skillTemplate.icon);
                    requestedIcons.Add(skillTemplate.icon);
                    Thread.Sleep(1000 + random.Next(-200, 201));
                    count++;
                    if (count >= 10)
                    {
                        writer.RequestChangeZone(session.Player.location.zoneId);
                        count = 0;
                        Console.WriteLine($"[{session.Host}:{session.Port}] Requested {requestedIcons.Count} icons");
                    }
                }
            }
            Console.WriteLine($"[{session.Host}:{session.Port}] Wait 10s...");
            Thread.Sleep(10000);
            return true;
        }

        static void RequestMobsImg(ISession session)
        {
            IMessageWriter writer = session.MessageWriter;
            MobTemplate[] mobTemplates = session.Data.MobTemplates;
            int count = 0;
            for (int templateID = 0; templateID < mobTemplates.Length; templateID++)
            {
                writer.RequestMobTemplate((short)templateID);
                count++;
                Thread.Sleep(1000 + random.Next(-200, 201));
                if (count >= 10)
                {
                    writer.RequestChangeZone(session.Player.location.zoneId);
                    count = 0;
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {templateID} mob templates");
                }
            }
        }

        static void TryGoOutsideIfAtHome(ISession session)
        {
            IMessageWriter writer = session.MessageWriter;
            Location location = session.Player.location;
            while (location.mapId <= 23 && location.mapId >= 21)
            {
                Console.WriteLine("The player is at home, trying to go outside...");
                int x = 0, y = 336;
                switch (location.mapId)
                {
                    case 21:
                        x = 495;
                        break;
                    case 22:
                        x = 205;
                        break;
                    case 23:
                        x = 475;
                        break;
                }
                writer.CharMove(x, y);
                Thread.Sleep(200);
                writer.CharMove(x, ++y);
                Thread.Sleep(200);
                writer.CharMove(x, --y);
                Thread.Sleep(200);
                writer.GetMapOffline();
                Thread.Sleep(1000);
                writer.FinishLoadMap();
                Thread.Sleep(3000);
                location = session.Player.location;
                Console.WriteLine($"Current map: {location.mapName} [{location.mapId}], zone {location.zoneId}");
            }
        }

        static bool TryConnect(ISession session)
        {
            int retryTimes = 0;
            try
            {
                session.Connect();
            }
            catch
            {
                while (!session.IsConnected)
                {
                    try
                    {
                        session.Connect();
                    }
                    catch
                    {
                        if (retryTimes >= 3)
                            break;
                        Console.WriteLine($"Retry {retryTimes + 1}...");
                        Thread.Sleep(1000);
                    }
                    retryTimes++;
                }
                if (!session.IsConnected)
                {
                    if (!string.IsNullOrEmpty(proxyData))
                    {
                        string[] arrP = proxyData.Split(':');
                        ProxyType proxyType = (ProxyType)Enum.Parse(typeof(ProxyType), arrP[0]);
                        string proxyHost = arrP[1];
                        ushort proxyPort = ushort.Parse(arrP[2]);
                        string proxyUsername = "";
                        string proxyPassword = "";
                        if (arrP.Length > 3)
                            proxyUsername = arrP[3];
                        if (arrP.Length > 4)
                            proxyPassword = arrP[4];
                        retryTimes = 0;
                        Console.WriteLine($"Failed to connect to the server! Retry with proxy {Regex.Replace(proxyHost, "[0-9]", "*")}:{proxyPort}...");
                        try
                        {
                            session.Connect(proxyHost, proxyPort, proxyUsername, proxyPassword, proxyType);
                        }
                        catch
                        {
                            while (!session.IsConnected)
                            {
                                try
                                {
                                    session.Connect(proxyHost, proxyPort, proxyUsername, proxyPassword, proxyType);
                                }
                                catch
                                {
                                    if (retryTimes >= 3)
                                        break;
                                    Console.WriteLine($"Retry {retryTimes + 1}...");
                                    Thread.Sleep(1000);
                                }
                                retryTimes++;
                            }
                            if (!session.IsConnected)
                            {
                                Console.WriteLine("Failed to connect to the server through the provided proxy!");
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Failed to connect to the server!");
                        return false;
                    }
                }
            }
            return true;
        }

        static Bitmap CropToContent(Bitmap source)
        {
            int width = source.Width;
            int height = source.Height;
            int minX = width, minY = height, maxX = 0, maxY = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixelColor = source.GetPixel(x, y);
                    if (pixelColor.A == 0)
                        continue;
                    minX = Math.Min(minX, x);
                    minY = Math.Min(minY, y);
                    maxX = Math.Max(maxX, x);
                    maxY = Math.Max(maxY, y);
                }
            }
            if (minX > maxX || minY > maxY)
                return new Bitmap(1, 1);
            Rectangle cropRect = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
            Bitmap croppedBitmap = new Bitmap(cropRect.Width, cropRect.Height);
            using (Graphics g = Graphics.FromImage(croppedBitmap))
            {
                g.DrawImage(source, new Rectangle(0, 0, croppedBitmap.Width, croppedBitmap.Height), cropRect, GraphicsUnit.Pixel);
            }
            return croppedBitmap;
        }
    }
}
