using DataNRO.Interfaces;
using Newtonsoft.Json;
using Starksoft.Net.Proxy;
using System;
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
using static DataNRO.GameData;

namespace DataNRO.CLI
{
    internal class Program
    {
        static Random random = new Random();
        static string proxyData = "";
        static int[] overwriteIconIDs = new int[0];
        static int maxRunSeconds = 4500;
        static bool forceProxy;

        //static readonly int IMG_BY_NAME_BANNER_COUNT = 100;
        //static readonly int IMG_BY_NAME_DIV_2_COUNT = 200;
        //static readonly int IMG_BY_NAME_HAT_COUNT = 200;
        //static readonly int IMG_BY_NAME_SKILL_COUNT = 20;
        //static readonly short IMG_BY_NAME_SKILL_TYPE_PAINT_COUNT = 10;
        static readonly short EFFECT_DATA_COUNT = short.MaxValue;

        static int[][][] charInfo = new int[33][][]
        {
            new int[4][]
            {
                new int[3] { 0, -13, 34 },
                new int[3] { 1, -8, 10 },
                new int[3] { 1, -9, 16 },
                new int[3] { 1, -9, 45 }
            },
            new int[4][]
            {
                new int[3] { 0, -13, 35 },
                new int[3] { 1, -8, 10 },
                new int[3] { 1, -9, 17 },
                new int[3] { 1, -9, 46 }
            },
            new int[4][]
            {
                new int[3] { 1, -10, 33 },
                new int[3] { 2, -10, 11 },
                new int[3] { 2, -8, 16 },
                new int[3] { 1, -12, 49 }
            },
            new int[4][]
            {
                new int[3] { 1, -10, 32 },
                new int[3] { 3, -12, 10 },
                new int[3] { 3, -11, 15 },
                new int[3] { 1, -13, 47 }
            },
            new int[4][]
            {
                new int[3] { 1, -10, 34 },
                new int[3] { 4, -8, 11 },
                new int[3] { 4, -7, 17 },
                new int[3] { 1, -12, 47 }
            },
            new int[4][]
            {
                new int[3] { 1, -10, 34 },
                new int[3] { 5, -12, 11 },
                new int[3] { 5, -9, 17 },
                new int[3] { 1, -13, 49 }
            },
            new int[4][]
            {
                new int[3] { 1, -10, 33 },
                new int[3] { 6, -10, 10 },
                new int[3] { 6, -8, 16 },
                new int[3] { 1, -12, 47 }
            },
            new int[4][]
            {
                new int[3] { 0, -9, 36 },
                new int[3] { 7, -5, 17 },
                new int[3] { 7, -11, 25 },
                new int[3] { 1, -8, 49 }
            },
            new int[4][]
            {
                new int[3] { 0, -7, 35 },
                new int[3] { 0, -18, 22 },
                new int[3] { 7, -10, 25 },
                new int[3] { 1, -7, 48 }
            },
            new int[4][]
            {
                new int[3] { 1, -11, 35 },
                new int[3] { 10, -3, 25 },
                new int[3] { 12, -10, 26 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 1, -11, 37 },
                new int[3] { 11, -3, 25 },
                new int[3] { 12, -11, 27 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -14, 34 },
                new int[3] { 12, -8, 21 },
                new int[3] { 9, -7, 31 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -12, 35 },
                new int[3] { 8, -5, 14 },
                new int[3] { 8, -15, 29 },
                new int[3] { 1, -9, 49 }
            },
            new int[4][]
            {
                new int[3] { 1, -9, 34 },
                new int[3] { 9, -12, 9 },
                new int[3] { 10, -7, 19 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 1, -13, 34 },
                new int[3] { 9, -12, 9 },
                new int[3] { 11, -10, 19 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 1, -8, 32 },
                new int[3] { 9, -12, 9 },
                new int[3] { 2, -6, 15 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 1, -8, 32 },
                new int[3] { 9, -12, 9 },
                new int[3] { 13, -12, 16 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -10, 31 },
                new int[3] { 9, -12, 9 },
                new int[3] { 7, -13, 20 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -11, 32 },
                new int[3] { 9, -12, 9 },
                new int[3] { 8, -15, 26 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -9, 33 },
                new int[3] { 9, -12, 9 },
                new int[3] { 14, -8, 18 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -11, 33 },
                new int[3] { 9, -12, 9 },
                new int[3] { 15, -6, 19 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -16, 31 },
                new int[3] { 9, -12, 9 },
                new int[3] { 9, -8, 28 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -14, 34 },
                new int[3] { 1, -8, 10 },
                new int[3] { 8, -16, 28 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -8, 36 },
                new int[3] { 7, -5, 17 },
                new int[3] { 0, -5, 25 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -9, 31 },
                new int[3] { 9, -12, 9 },
                new int[3] { 0, -6, 20 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 2, -9, 36 },
                new int[3] { 13, -5, 17 },
                new int[3] { 16, -11, 25 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 1, -9, 34 },
                new int[3] { 8, -5, 13 },
                new int[3] { 10, -7, 19 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 1, -13, 34 },
                new int[3] { 8, -5, 13 },
                new int[3] { 11, -10, 19 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 1, -8, 32 },
                new int[3] { 8, -5, 13 },
                new int[3] { 2, -6, 15 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 1, -8, 32 },
                new int[3] { 8, -5, 13 },
                new int[3] { 13, -12, 16 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -9, 33 },
                new int[3] { 8, -5, 13 },
                new int[3] { 14, -8, 18 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -11, 33 },
                new int[3] { 8, -5, 13 },
                new int[3] { 15, -6, 19 },
                new int[3]
            },
            new int[4][]
            {
                new int[3] { 0, -16, 32 },
                new int[3] { 8, -5, 13 },
                new int[3] { 9, -8, 29 },
                new int[3]
            }
        };

        [DllImport("msvcrt.dll")]
        public static extern int system(string cmd);

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            new Thread(FailoverThread) { IsBackground = true }.Start();
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            proxyData = Environment.GetEnvironmentVariable("PROXY");
            string overwriteIconsEnv = Environment.GetEnvironmentVariable("OVERWRITE_ICONS");
            if (!string.IsNullOrEmpty(overwriteIconsEnv))
                overwriteIconIDs = overwriteIconsEnv.Split(',').Select(int.Parse).ToArray();
            string maxRunSecondsEnv = Environment.GetEnvironmentVariable("MAX_RUN_SECONDS");
            if (!string.IsNullOrEmpty(maxRunSecondsEnv))
                maxRunSeconds = int.Parse(maxRunSecondsEnv);
            string forceProxyEnv = Environment.GetEnvironmentVariable("FORCE_PROXY");
            if (!string.IsNullOrEmpty(forceProxyEnv))
                forceProxy = bool.Parse(forceProxyEnv);
            string data = Environment.GetEnvironmentVariable("DATA");
            if (string.IsNullOrEmpty(data))
            {
#if DEBUG
                Console.Write("DATA: ");
                data = Console.ReadLine();
                Console.Write("PROXY: ");
                proxyData = Console.ReadLine();
                Console.Write("FORCE_PROXY: ");
                bool.TryParse(Console.ReadLine(), out forceProxy);
#else
                Console.WriteLine("DATA environment variable is not set!");
                Environment.Exit(1);
                return;
#endif
            }
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
#if !DEBUG
            Console.WriteLine($"DataNRO.CLI will run for {maxRunSeconds} seconds!");
            for (int i = 0; i < maxRunSeconds / 10; i++)
                Thread.Sleep(100 * maxRunSeconds);
            Console.WriteLine($"DataNRO.CLI has been running for {maxRunSeconds} seconds, exiting...");
            Environment.Exit(1);
#endif
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
                Assembly assembly = Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location), $"DataNRO.{type}.dll"));
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
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create session type \"{type}\" from assembly \"DataNRO.{type}.dll\"!\r\n{ex}");
                return;
            }
            session.Data.Path = dataPath;
            session.Data.SaveIcon = requestAndSaveIcons;
            session.Data.OverwriteIconIDs = overwriteIconIDs;
            foreach (int iconId in overwriteIconIDs)    //-1 will not delete all icons
            {
                if (File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{iconId}.png"))
                    File.Delete($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{iconId}.png");
            }
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
                //if (session.Data.MapTileIDs.Count == 0)
                //    Console.WriteLine($"[{session.Host}:{session.Port}] No map tile IDs found, skipping map templates...");
                //else
                //    RequestMapTemplates(session);
                RequestEffectData(session);
                //if (!RequestMobImgs(session))
                //    return;
                //if (!RequestImgByNames(session))    //not working right now
                //    return;
                //if (!RequestIcons(session))
                //    return;
            }
            TryGoOutsideIfAtHome(session);
            Console.WriteLine($"[{session.Host}:{session.Port}] Disconnect from {session.Host}:{session.Port} in 10s...");
            writer.Chat("DataNRO by ElectroHeavenVN");
            Thread.Sleep(10000);
            session.Disconnect();

            if (session.Data.SaveIcon)
                ProcessImages(session);

            Console.WriteLine($"[{session.Host}:{session.Port}] Writing data to {session.Data.Path}\\...");
            Formatting formatting = Formatting.Indented;
            if (session.Data.Maps.Count > 0)
            {
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.Maps)}.json", JsonConvert.SerializeObject(session.Data.Maps, formatting, new JsonSerializerSettings()
                {
                    ContractResolver = new IgnoreMapTemplateResolver()
                }));
            }
            if (session.Data.NpcTemplates.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.NpcTemplates)}.json", JsonConvert.SerializeObject(session.Data.NpcTemplates, formatting));
            if (session.Data.MobTemplates.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.MobTemplates)}.json", JsonConvert.SerializeObject(session.Data.MobTemplates, formatting));
            if (session.Data.ItemOptionTemplates.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.ItemOptionTemplates)}.json", JsonConvert.SerializeObject(session.Data.ItemOptionTemplates, formatting));
            if (session.Data.NClasses.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.NClasses)}.json", JsonConvert.SerializeObject(session.Data.NClasses, formatting));
            if (session.Data.ItemTemplates.Count > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.ItemTemplates)}.json", JsonConvert.SerializeObject(session.Data.ItemTemplates, formatting));
            if (session.Data.Parts.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.Parts)}.json", JsonConvert.SerializeObject(session.Data.Parts, formatting));

            //if (session.Data.SaveIcon)
            //File.WriteAllText($"{Path.GetDirectoryName(session.Data.Path)}\\{nameof(GameData.MobTemplateEffectData)}.json", JsonConvert.SerializeObject(session.Data.MobTemplateEffectData, formatting));

            File.WriteAllText($"{session.Data.Path}\\LastUpdated", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
            Thread.Sleep(3000);
            session.FileWriter.DeleteTempFiles();
            session.Dispose();
        }

        static void ProcessImages(ISession session)
        {
            SplitBigImgs(session);
            CombineNPCImages(session);
            CombineMobImages(session);
            if (session.Data.MapTileIDs.Count == 0)
                Console.WriteLine($"[{session.Host}:{session.Port}] No map tile IDs found, skipping map images...");
            else
                CombineMapImages(session);
        }

        static void SplitBigImgs(ISession session)
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
        }

        static void CombineNPCImages(ISession session)
        {
            Console.WriteLine($"[{session.Host}:{session.Port}] Combining NPC images...");
            for (int i = 0; i < session.Data.NpcTemplates.Length; i++)
            {
                try
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
                    if (partHead != null && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partHead.pi[0].id}.png"))
                        imgHead = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partHead.pi[0].id}.png");
                    if (partBody != null && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partBody.pi[1].id}.png"))
                        imgBody = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partBody.pi[1].id}.png");
                    if (partLeg != null && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partLeg.pi[1].id}.png"))
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
                    if (maxWidth == 0 || maxHeight == 0)
                        continue;
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
                catch { }
            }
        }

        static void CombineMobImages(ISession session)
        {
            Console.WriteLine($"[{session.Host}:{session.Port}] Combining monster images...");
            for (int templateId = 0; templateId < session.Data.MobTemplates.Length; templateId++)
            {
                MobTemplate template = session.Data.MobTemplates[templateId];
                EffectData effectData = session.Data.MobTemplateEffectData[templateId];
                if (effectData.frame == null || effectData.frame.Length == 0)
                    continue;
                Frame frame = effectData.frame[0];
                Bitmap mobImg = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\EffectDataMob\\{templateId}.png");
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

        static void CombineMapImages(ISession session)
        {
            Console.WriteLine($"[{session.Host}:{session.Port}] Combining map images...");
            string path = $"{Path.GetDirectoryName(session.Data.Path)}\\Maps";
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            for (int i = 0; i < session.Data.Maps.Count; i++)
            {
                Map map = session.Data.Maps[i];
                if (!session.Data.MapTileIDs.ContainsKey(map.id) || session.Data.MapTileIDs[map.id] == -1)
                {
                    //Combine maps with all tileIDs then manually check them later
                    //for (int tileID = 1; tileID <= 42; tileID++)
                    //{
                    //    try
                    //    {
                    //        CombineMapImages(session, map, tileID, true); 
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Console.WriteLine(ex);
                    //    }
                    //}
                    continue;
                }
                try
                {
                    int tileID = session.Data.MapTileIDs[map.id];
                    CombineMapImages(session, map, tileID);
                    File.Copy($"{Path.GetDirectoryName(session.Data.Path)}\\Resources\\{tileID}$1", $"{Path.GetDirectoryName(session.Data.Path)}\\Maps\\{map.id}_tile.png", true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        static void CombineMapImages(ISession session, Map map, int tileID, bool includeTileID = false)
        {
            MapTemplate mapTemplate = map.mapTemplate;
            if (mapTemplate == null)
                return;
            int mapID = map.id;
            int pixelWidth = (mapTemplate.width - 2) * 24 * session.Data.ZoomLevel;
            int pixelHeight = (mapTemplate.height - 1) * 24 * session.Data.ZoomLevel;
            Bitmap mapImg = new Bitmap(pixelWidth, pixelHeight);

            Bitmap imgWaterfall = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Resources\\wtf");
            Bitmap imgTopWaterfall = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Resources\\twtf");

            Graphics mapImgG = Graphics.FromImage(mapImg);

            void paintTile(int frame, int x, int y)
            {
                x -= 24;
                x *= session.Data.ZoomLevel;
                y *= session.Data.ZoomLevel;
                try
                {
                    Bitmap frameImg = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Resources\\{tileID}${frame + 1}");
                    mapImgG.DrawImage(frameImg, x, y);
                    frameImg.Dispose();
                }
                catch { }
            }

            mapTemplate.LoadMap(tileID);

            for (int x = 1; x < mapTemplate.width - 1; x++)
            {
                for (int y = 0; y < mapTemplate.height; y++)
                {
                    int num = mapTemplate.maps[y * mapTemplate.width + x] - 1;
                    if ((mapTemplate.TileTypeAt(x, y) & 0x100) == 256)
                        continue;
                    if ((mapTemplate.TileTypeAt(x, y) & 0x20) == 32)
                        mapImgG.DrawImage(imgWaterfall, new Rectangle((x - 1) * 24 * session.Data.ZoomLevel, y * 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel), new Rectangle(0, 0, 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel), GraphicsUnit.Pixel);
                    else if ((mapTemplate.TileTypeAt(x, y) & 0x80) == 128)
                        mapImgG.DrawImage(imgTopWaterfall, new Rectangle((x - 1) * 24 * session.Data.ZoomLevel, y * 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel), new Rectangle(0, 0, 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel), GraphicsUnit.Pixel);
                    else
                    {
                        //if (tileID == 13 && num != -1)
                        //    continue;
                        if (tileID == 2 && (mapTemplate.TileTypeAt(x, y) & 0x200) == 512 && num != -1)
                        {
                            paintTile(num, x * 24, y * 24);
                            paintTile(num, x * 24, y * 24 + 1);
                        }
                        if ((mapTemplate.TileTypeAt(x, y) & 0x200) == 512)
                        {
                            if (num != -1)
                            {
                                paintTile(num, x * 24, y * 24);
                                paintTile(num, x * 24, y * 24 + 1);
                            }
                        }
                        else if (num != -1)
                            paintTile(num, x * 24, y * 24);
                    }

                }
            }
            mapImgG.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            Font font = new Font("Arial", 50);
            for (int i = 0; i < 3; i++)
                mapImgG.DrawString("© ElectroHeavenVN", font, Brushes.Black, random.Next(0, pixelWidth - 500), random.Next(0, pixelHeight - 100));
            if (includeTileID)
                mapImg.Save($"{$"{Path.GetDirectoryName(session.Data.Path)}\\Maps"}\\{mapID}-{tileID}.png");
            else
                mapImg.Save($"{$"{Path.GetDirectoryName(session.Data.Path)}\\Maps"}\\{mapID}.png");
            mapImgG.Dispose();
            mapImg.Dispose();
            imgWaterfall.Dispose();
            imgTopWaterfall.Dispose();
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
                if (requestedIcons.Count % 10 == 0)
                {
                    writer.RequestChangeZone(session.Player.location.zoneId);
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
            List<ItemTemplate> items = new List<ItemTemplate>(session.Data.ItemTemplates);
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
                if (requestedIcons.Count % 10 == 0)
                {
                    writer.RequestChangeZone(session.Player.location.zoneId);
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {requestedIcons.Count} icons");
                }
            }
            //parts
            List<Part> parts = new List<Part>(session.Data.Parts);
            while (parts.Count > 0)
            {
                Part part = parts[random.Next(0, parts.Count)];
                parts.Remove(part);
                if (part is null)
                    continue;
                //int index = 0;
                //if (part.type == 1 || part.type == 2) //body, leg
                //    index = 1;
                for (int cf = 0; cf < charInfo.Length; cf++)
                {
                    for (int i = 0; i < charInfo[cf].Length; i++)
                    {
                        int index = charInfo[cf][i][0];
                        if (part.pi[index].id <= 0)
                            continue;
                        RequestPartIcon(part, index);
                    }
                }
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
                    if (requestedIcons.Count % 10 == 0)
                    {
                        writer.RequestChangeZone(session.Player.location.zoneId);
                        Console.WriteLine($"[{session.Host}:{session.Port}] Requested {requestedIcons.Count} icons");
                    }
                }
            }
            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {requestedIcons.Count} icons.");
            Console.WriteLine($"[{session.Host}:{session.Port}] Wait 10s...");
            Thread.Sleep(10000);
            return true;
        }

        static bool RequestMobImgs(ISession session)
        {
            IMessageWriter writer = session.MessageWriter;
            int count = 0;
            while (session.Data.MobTemplates == null)
            {
                count++;
                if (count >= 10)
                {
                    Console.WriteLine($"[{session.Host}:{session.Port}] Get monster images failed!");
                    return false;
                }
                Thread.Sleep(1000);
            }
            MobTemplate[] mobTemplates = session.Data.MobTemplates;
            int templateID = 0;
            for (; templateID < mobTemplates.Length; templateID++)
            {
                writer.RequestMobTemplate((short)templateID);
                Thread.Sleep(1000 + random.Next(-200, 201));
                if (templateID % 10 == 0)
                {
                    writer.RequestChangeZone(session.Player.location.zoneId);
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {templateID} mob templates");
                }
            }
            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {templateID} mob templates.");
            return true;
        }

        static void RequestMapTemplates(ISession session)
        {
            IMessageWriter writer = session.MessageWriter;
            List<Map> maps = session.Data.Maps;
            int i = 0;
            for (; i < maps.Count; i++)
            {
                Map map = maps[i];
                session.Data.MapToReceiveTemplate = map;
                writer.RequestMapTemplate(map.id);
                int count = 0;
                do
                {
                    Thread.Sleep(1000 + random.Next(-200, 201));
                    count++;
                    if (count >= 5)
                    {
                        Console.WriteLine($"[{session.Host}:{session.Port}] Failed to get map template {map.id}!");
                        break;
                    }
                }
                while (session.Data.MapToReceiveTemplate != null);
                if (i % 10 == 0)
                {
                    writer.RequestChangeZone(session.Player.location.zoneId);
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} map templates");
                }
            }
            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} map templates.");
            session.Data.MapToReceiveTemplate = null;
        }

        static void RequestEffectData(ISession session)
        {
            IMessageWriter writer = session.MessageWriter;
            for (short i = 0; i < EFFECT_DATA_COUNT; i++)
            {
                writer.GetEffectData(i);
                Thread.Sleep(1000 + random.Next(-200, 201));
                if (i % 10 == 0)
                {
                    writer.RequestChangeZone(session.Player.location.zoneId);
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} effect data");
                }
            }
            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {session.Data.EffectDataDict.Count} effect data.");
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
                if (forceProxy)
                {
                    if (string.IsNullOrEmpty(proxyData))
                    {
                        Console.WriteLine("No proxy data provided! Falling back to direct connection...");
                        session.Connect();
                    }
                    else
                        return TryConnectProxy(session);
                }
                else
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
                        string proxyHost = arrP[1];
                        ushort proxyPort = ushort.Parse(arrP[2]);
                        Console.WriteLine($"Failed to connect to the server! Retry with proxy {Regex.Replace(proxyHost, "[0-9]", "*")}:{proxyPort}...");
                        return TryConnectProxy(session);
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

        static bool TryConnectProxy(ISession session)
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
            Console.WriteLine("Using proxy: " + Regex.Replace(proxyHost, "[0-9]", "*") + ":" + proxyPort + " (" + proxyType + ")");
            int retryTimes = 0;
            try
            {
                session.Connect(proxyHost, proxyPort, proxyUsername, proxyPassword, proxyType);
                return true;
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
                return true;
            }
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

        //static bool RequestImgByNames(ISession session)
        //{
        //    IMessageWriter writer = session.MessageWriter;
        //    int count = 0;
        //    while (session.Data.NClasses == null)
        //    {
        //        count++;
        //        if (count >= 10)
        //        {
        //            Console.WriteLine($"[{session.Host}:{session.Port}] Get ImgByName(s) failed!");
        //            return false;
        //        }
        //        Thread.Sleep(1000);
        //    }
        //    count = 0;
        //    for (int i = 0; i < IMG_BY_NAME_BANNER_COUNT; i++)
        //    {
        //        string name = "banner_" + i;
        //        writer.GetImgByName(name);
        //        Thread.Sleep(1000 + random.Next(-200, 201));
        //        if (i % 10 == 0)
        //        {
        //            writer.RequestChangeZone(session.Player.location.zoneId);
        //            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} banner ImgByName(s)");
        //        }
        //        count++;
        //    }
        //    for (int i = 0; i < IMG_BY_NAME_DIV_2_COUNT; i++)
        //    {
        //        string name = "mount_" + i / 2 + "_" + i % 2;
        //        writer.GetImgByName(name);
        //        Thread.Sleep(1000 + random.Next(-200, 201));
        //        if (i % 10 == 0)
        //        {
        //            writer.RequestChangeZone(session.Player.location.zoneId);
        //            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} mount ImgByName(s)");
        //        }
        //        count++;
        //    }
        //    for (int i = 0; i < IMG_BY_NAME_DIV_2_COUNT; i++)   //only 10 * 2
        //    {
        //        string name = "aura_" + i / 2 + "_" + i % 2;
        //        writer.GetImgByName(name);
        //        Thread.Sleep(1000 + random.Next(-200, 201));
        //        if (i % 10 == 0)
        //        {
        //            writer.RequestChangeZone(session.Player.location.zoneId);
        //            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} aura ImgByName(s)");
        //        }
        //        count++;
        //    }
        //    for (int i = 0; i < IMG_BY_NAME_DIV_2_COUNT; i++)   //only 10 * 2
        //    {
        //        string name = "set_eff_" + i / 2 + "_" + i % 2;
        //        writer.GetImgByName(name);
        //        Thread.Sleep(1000 + random.Next(-200, 201));
        //        if (i % 10 == 0)
        //        {
        //            writer.RequestChangeZone(session.Player.location.zoneId);
        //            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} set item ImgByName(s)");
        //        }
        //        count++;
        //    }
        //    for (int i = 0; i < IMG_BY_NAME_HAT_COUNT; i++)
        //    {
        //        string name = "hat_sau_" + i;
        //        writer.GetImgByName(name);
        //        Thread.Sleep(1000 + random.Next(-200, 201));
        //        if (i % 10 == 0)
        //        {
        //            writer.RequestChangeZone(session.Player.location.zoneId);
        //            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} hat_sau ImgByName(s)");
        //        }
        //        count++;
        //    }
        //    for (int i = 0; i < IMG_BY_NAME_HAT_COUNT; i++)
        //    {
        //        string name = "hat_sau_ngang_" + i;
        //        writer.GetImgByName(name);
        //        Thread.Sleep(1000 + random.Next(-200, 201));
        //        if (i % 10 == 0)
        //        {
        //            writer.RequestChangeZone(session.Player.location.zoneId);
        //            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} hat_sau_ngang ImgByName(s)");
        //        }
        //        count++;
        //    }
        //    for (int i = 0; i < IMG_BY_NAME_HAT_COUNT; i++)
        //    {
        //        string name = "hat_truoc_" + i;
        //        writer.GetImgByName(name);
        //        Thread.Sleep(1000 + random.Next(-200, 201));
        //        if (i % 10 == 0)
        //        {
        //            writer.RequestChangeZone(session.Player.location.zoneId);
        //            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} hat_truoc ImgByName(s)");
        //        }
        //        count++;
        //    }
        //    for (int i = 0; i < IMG_BY_NAME_HAT_COUNT; i++)
        //    {
        //        string name = "hat_truoc_ngang_"; 
        //        writer.GetImgByName(name);
        //        Thread.Sleep(1000 + random.Next(-200, 201));
        //        if (i % 10 == 0)
        //        {
        //            writer.RequestChangeZone(session.Player.location.zoneId);
        //            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} hat_truoc_ngang ImgByName(s)");
        //        }
        //        count++;
        //    }
        //    foreach (NClass nClass in session.Data.NClasses)
        //    {
        //        foreach (SkillTemplate skillTemplate in nClass.skillTemplates)
        //        {
        //            for (int typePaint = 0; typePaint < IMG_BY_NAME_SKILL_TYPE_PAINT_COUNT; typePaint++)
        //            {
        //                for (int i = 0; i < IMG_BY_NAME_SKILL_COUNT; i++)
        //                {
        //                    string name = $"Skills_{skillTemplate.id}_{typePaint}_{i}";
        //                    writer.GetImgByName(name);
        //                    Thread.Sleep(1000 + random.Next(-200, 201));
        //                    if (i % 10 == 0)
        //                    {
        //                        writer.RequestChangeZone(session.Player.location.zoneId);
        //                        Console.WriteLine($"[{session.Host}:{session.Port}] Requested {count} skill ImgByName(s)");
        //                    }
        //                    count++;
        //                }
        //            }
        //        }
        //    }
        //    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {count} ImgByName(s).");
        //    return true;
        //}
    }
}
