using EHVN.DataNRO.Interfaces;
using Starksoft.Net.Proxy;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static EHVN.DataNRO.GameData;

namespace EHVN.DataNRO.CLI
{
    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    internal class Program
    {
        static string proxyData = "";
        static int[] overwriteIconIDs = [];
        static int maxRunSeconds = 4500;
        static bool forceProxy;

        [DllImport("msvcrt.dll")]
        public static extern int system(string cmd);

        static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            new Thread(FailoverThread) { IsBackground = true }.Start();
            Thread.Sleep(100);
            if (!Directory.Exists("Data"))
                Directory.CreateDirectory("Data");
            proxyData = Environment.GetEnvironmentVariable("PROXY") ?? "";
            string? overwriteIconsEnv = Environment.GetEnvironmentVariable("OVERWRITE_ICONS");
            if (!string.IsNullOrEmpty(overwriteIconsEnv))
                overwriteIconIDs = overwriteIconsEnv.Split(',').Select(int.Parse).ToArray();
            string? maxRunSecondsEnv = Environment.GetEnvironmentVariable("MAX_RUN_SECONDS");
            if (!string.IsNullOrEmpty(maxRunSecondsEnv))
                maxRunSeconds = int.Parse(maxRunSecondsEnv);
            string? forceProxyEnv = Environment.GetEnvironmentVariable("FORCE_PROXY");
            if (!string.IsNullOrEmpty(forceProxyEnv))
                forceProxy = bool.Parse(forceProxyEnv);
            string? data = Environment.GetEnvironmentVariable("DATA");
            if (string.IsNullOrEmpty(data))
            {
#if DEBUG
                Console.Write("DATA: ");
                data = Console.ReadLine() ?? "";
                Console.Write("PROXY: ");
                proxyData = Console.ReadLine() ?? "";
                Console.Write("FORCE_PROXY: ");
                bool.TryParse(Console.ReadLine(), out forceProxy);
#else
                Console.WriteLine("DATA environment variable is not set!");
                return 1;
#endif
            }
            string[] datas = data.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string d in datas)
            {
                try
                {
                    LoginAndGetDataAsync(d).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
#if DEBUG
            return system("pause");
#else
            return 0;
#endif
        }

        static void FailoverThread()
        {
            Console.WriteLine($"DataNRO.CLI will run for {maxRunSeconds} seconds!");
            for (int i = 0; i < maxRunSeconds / 10; i++)
                Thread.Sleep(100 * maxRunSeconds);
            Console.WriteLine($"DataNRO.CLI has been running for {maxRunSeconds} seconds, exiting...");
            Environment.Exit(1);
        }

        static async Task LoginAndGetDataAsync(string data)
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
            ISession? session;
            try
            {
                Console.WriteLine($"Creating session type \"{type}\" from assembly \"DataNRO.{type}.dll\"...");
                Assembly assembly = Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? "", $"DataNRO.{type}.dll"));
                Console.WriteLine($"Loaded assembly: {assembly.FullName}");
                Console.WriteLine($"Assembly name: {assembly.GetCustomAttribute<AssemblyTitleAttribute>()!.Title}");
                Console.WriteLine($"Assembly description: {assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()!.Description}");
                Console.WriteLine($"Assembly company: {assembly.GetCustomAttribute<AssemblyCompanyAttribute>()!.Company}");
                Console.WriteLine($"Assembly copyright: {assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()!.Copyright}");
                string sessionTypeName = $"EHVN.DataNRO.{type}.{type}Session";
                Console.WriteLine($"Creating session type: {sessionTypeName}...");
                Type? sessionType = assembly.GetType(sessionTypeName);
                if (sessionType is null)
                {
                    Console.WriteLine($"Failed to get session type \"{sessionTypeName}\" from assembly \"DataNRO.{type}.dll\"!");
                    return;
                }
                session = (ISession?)Activator.CreateInstance(sessionType, [host, port]);
                Console.WriteLine($"Session type \"{type}\" has been created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create session type \"{type}\" from assembly \"DataNRO.{type}.dll\"!\r\n{ex}");
                return;
            }
            if (session is null)
            {
                Console.WriteLine($"Failed to create session type \"{type}\" from assembly \"DataNRO.{type}.dll\"!");
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
            if (!await TryConnectAsync(session))
                return;
            Console.WriteLine("Connected successfully!");
            IMessageWriter writer = session.MessageWriter;
            writer.SetClientType();
            await Task.Delay(2000);
            writer.ImageSource();
            await Task.Delay(1000);
            if (session.Data.SaveIcon)
            {
                Console.WriteLine($"[{session.Host}:{session.Port}] Downloading data...");
                writer.GetResource(1);
                do
                {
                    await Task.Delay(1000);
                }
                while (!session.Data.AllResourceLoaded);
            }
            for (int i = 0; i < 3; i++)
            {
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
                await Task.Delay(1000);
                writer.UpdateMap();
                writer.UpdateSkill();
                writer.UpdateItem();
                writer.UpdateData();
                await Task.Delay(1000);
                writer.ClientOk();
                writer.FinishUpdate();
                await Task.Delay(4000);
                writer.FinishLoadMap();
                int count = 0;
                Location? location;
                bool retry = false;
                do
                {
                    location = session.Player.Location;
                    count++;
                    if (count >= 20)
                    {
                        if (i == 2)
                        {
                            Console.WriteLine($"[{session.Host}:{session.Port}] Failed to get the player's location!");
                            session.Disconnect();
                            return;
                        }
                        Console.WriteLine($"[{session.Host}:{session.Port}] Failed to get the player's location! Retry {i + 1}...");
                        retry = true;
                        break;
                    }
                    await Task.Delay(1000);
                }
                while (location is null || string.IsNullOrEmpty(location.mapName));
                if (retry)
                    continue;
                Console.WriteLine($"[{session.Host}:{session.Port}] Current map: {location!.mapName} [{location.mapId}], zone {location.zoneId}");
                await Task.Delay(2000);
                break;
            }
            if (session.Data.SaveIcon)
            {
                if (session.Data.MapTileIDs.Count == 0)
                    Console.WriteLine($"[{session.Host}:{session.Port}] No map tile IDs found, skipping map templates...");
                else
                    await RequestMapsTemplateAsync(session);
                await RequestMobsImgAsync(session);
                if (!await RequestIconsAsync(session))
                    return;
            }
            await TryGoOutsideIfAtHomeAsync(session);
            Console.WriteLine($"[{session.Host}:{session.Port}] Disconnect from {session.Host}:{session.Port} in 20s...");
            writer.Chat("DataNRO by ElectroHeavenVN");
            await Task.Delay(20000);
            session.Disconnect();

            if (session.Data.SaveIcon)
                ProcessImages(session);

            Console.WriteLine($"[{session.Host}:{session.Port}] Writing data to {session.Data.Path}\\...");

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = true,
                IncludeFields = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            if (session.Data.Maps.Count > 0)
            {
                foreach (Map map in session.Data.Maps)
                    map.mapTemplate = null;
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.Maps)}.json", JsonSerializer.Serialize(session.Data.Maps, options));
            }
            if (session.Data.NpcTemplates.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.NpcTemplates)}.json", JsonSerializer.Serialize(session.Data.NpcTemplates, options));
            if (session.Data.MobTemplates.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.MobTemplates)}.json", JsonSerializer.Serialize(session.Data.MobTemplates, options));
            if (session.Data.ItemOptionTemplates.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.ItemOptionTemplates)}.json", JsonSerializer.Serialize(session.Data.ItemOptionTemplates, options));
            if (session.Data.NClasses.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.NClasses)}.json", JsonSerializer.Serialize(session.Data.NClasses, options));
            if (session.Data.ItemTemplates.Count > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.ItemTemplates)}.json", JsonSerializer.Serialize(session.Data.ItemTemplates, options));
            options = new JsonSerializerOptions(options) { WriteIndented = false };
            if (session.Data.Parts.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.Parts)}.json", JsonSerializer.Serialize(session.Data.Parts, options));

            //if (session.Data.SaveIcon)
            //File.WriteAllText($"{Path.GetDirectoryName(session.Data.Path)}\\{nameof(GameData.MobTemplateEffectData)}.json", JsonSerializer.Serialize(session.Data.MobTemplateEffectData, options));

            File.WriteAllText($"{session.Data.Path}\\LastUpdated", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
            await Task.Delay(3000);
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
            List<Bitmap> smallImages = [];
            for (int i = 0; File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\BigIcons\\Big{i}.png"); i++)
                smallImages.Add(new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\BigIcons\\Big{i}.png"));
            for (int id = 0; id < session.Data.SmallImg.Length; id++)
            {
                if (!session.Data.CanOverwriteIcon(id) && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{id}.png"))
                    continue;
                int[] smallImg = session.Data.SmallImg[id];
                int imgBigIndex = smallImg[0];
                if (imgBigIndex < 0 || imgBigIndex >= smallImages.Count)
                    continue;
                if (smallImg[1] >= 256 || smallImg[2] >= 256 || smallImg[3] >= 256 || smallImg[4] >= 256)
                    continue;
                int x = smallImg[1] * session.Data.ZoomLevel;
                int y = smallImg[2] * session.Data.ZoomLevel;
                int width = smallImg[3] * session.Data.ZoomLevel;
                int height = smallImg[4] * session.Data.ZoomLevel;
                using Bitmap bitmap = new Bitmap(width, height);
                using (Graphics g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(smallImages[imgBigIndex], new Rectangle(0, 0, width, height), new Rectangle(x, y, width, height), GraphicsUnit.Pixel);
                }
                bitmap.Save($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{id}.png");
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
                    Part? partHead = npc.headId == -1 ? null : session.Data.Parts[npc.headId];
                    Part? partBody = npc.bodyId == -1 ? null : session.Data.Parts[npc.bodyId];
                    Part? partLeg = npc.legId == -1 ? null : session.Data.Parts[npc.legId];
                    if (partHead is null && partBody is null && partLeg is null)
                        continue;
                    Bitmap? imgHead = null, imgBody = null, imgLeg = null;
                    if (partHead is not null && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partHead.pi[0].id}.png"))
                        imgHead = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partHead.pi[0].id}.png");
                    if (partBody is not null && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partBody.pi[1].id}.png"))
                        imgBody = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partBody.pi[1].id}.png");
                    if (partLeg is not null && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partLeg.pi[1].id}.png"))
                        imgLeg = new Bitmap($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partLeg.pi[1].id}.png");
                    int maxWidth = 0, maxHeight = 0;
                    if (imgHead is not null)
                    {
                        maxWidth = imgHead.Width;
                        maxHeight = imgHead.Height;
                    }
                    if (imgBody is not null)
                    {
                        maxWidth += imgBody.Width;
                        maxHeight += imgBody.Height;
                    }
                    if (imgLeg is not null)
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
                            if (imgHead is not null && partHead is not null)
                                g.DrawImage(imgHead, (cx + -13 + partHead.pi[0].dx) * session.Data.ZoomLevel, (cy - 34 + partHead.pi[0].dy) * session.Data.ZoomLevel, imgHead.Width, imgHead.Height);
                            if (imgLeg is not null && partLeg is not null)
                                g.DrawImage(imgLeg, (cx + -8 + partLeg.pi[1].dx) * session.Data.ZoomLevel, (cy - 10 + partLeg.pi[1].dy) * session.Data.ZoomLevel, imgLeg.Width, imgLeg.Height);
                            if (imgBody is not null && partBody is not null)
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
                if (effectData.frame.Length == 0)
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
                            ImageInfo? imageInfo = effectData.imgInfo.FirstOrDefault(img => img.id == frame.idImg[i]);
                            if (imageInfo is null)
                                continue;
                            try
                            {
                                g.DrawImage(mobImg, 
                                    new Rectangle((x + frame.dx[i]) * session.Data.ZoomLevel, (y + frame.dy[i]) * session.Data.ZoomLevel, imageInfo.w * session.Data.ZoomLevel, imageInfo.h * session.Data.ZoomLevel),
                                    new Rectangle(imageInfo.x0 * session.Data.ZoomLevel, imageInfo.y0 * session.Data.ZoomLevel, imageInfo.w * session.Data.ZoomLevel, imageInfo.h * session.Data.ZoomLevel),
                                    GraphicsUnit.Pixel);
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
            MapTemplate? mapTemplate = map.mapTemplate;
            if (mapTemplate is null)
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
                        mapImgG.DrawImage(imgWaterfall,
                            new Rectangle((x - 1) * 24 * session.Data.ZoomLevel, y * 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel),
                            new Rectangle(0, 0, 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel),
                            GraphicsUnit.Pixel);
                    else if ((mapTemplate.TileTypeAt(x, y) & 0x80) == 128)
                        mapImgG.DrawImage(imgTopWaterfall, 
                            new Rectangle((x - 1) * 24 * session.Data.ZoomLevel, y * 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel),
                            new Rectangle(0, 0, 24 * session.Data.ZoomLevel, 24 * session.Data.ZoomLevel),
                            GraphicsUnit.Pixel);
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
                mapImgG.DrawString("© ElectroHeavenVN", font, Brushes.Black, Random.Shared.Next(0, pixelWidth - 500), Random.Shared.Next(0, pixelHeight - 100));
            if (includeTileID)
                mapImg.Save($"{$"{Path.GetDirectoryName(session.Data.Path)}\\Maps"}\\{mapID}-{tileID}.png");
            else
                mapImg.Save($"{$"{Path.GetDirectoryName(session.Data.Path)}\\Maps"}\\{mapID}.png");
            mapImgG.Dispose();
            mapImg.Dispose();
            imgWaterfall.Dispose();
            imgTopWaterfall.Dispose();
        }

        static async Task<bool> RequestIconsAsync(ISession session)
        {
            IMessageWriter writer = session.MessageWriter;
            List<int> requestedIcons = [];
            int count = 0;
            async Task RequestPartIcon(Part? part, int index)
            {
                if (part is null)
                    return;
                int id = part.pi[index].id;
                if (requestedIcons.Contains(id))
                    return;
                if (!session.Data.CanOverwriteIcon(id) && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{id}.png"))
                    return;
                writer.RequestIcon(id);
                requestedIcons.Add(id);
                await Task.Delay(1000 + Random.Shared.Next(-200, 201));
                if (requestedIcons.Count % 10 == 0)
                {
                    writer.RequestChangeZone(session.Player.Location?.zoneId ?? 0);
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {requestedIcons.Count} icons");
                }
            }

            writer.UpdateData();
            while (session.Data.Parts.Length == 0)
            {
                count++;
                if (count >= 10)
                {
                    Console.WriteLine($"[{session.Host}:{session.Port}] Get parts failed!");
                    return false;
                }
                await Task.Delay(1000);
            }
            //item
            count = 0;
            while (session.Data.ItemTemplates is null)
            {
                await Task.Delay(1000 + Random.Shared.Next(-200, 201));
                count++;
                if (count >= 10)
                {
                    writer.RequestChangeZone(session.Player.Location?.zoneId ?? 0);
                    count = 0;
                }
            }
            List<ItemTemplate> items = [.. session.Data.ItemTemplates];
            while (items.Count > 0)
            {
                ItemTemplate item = items[Random.Shared.Next(0, items.Count)];
                items.Remove(item);
                if (requestedIcons.Contains(item.icon))
                    continue;
                if (!session.Data.CanOverwriteIcon(item.icon) && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{item.icon}.png"))
                    continue;
                writer.RequestIcon(item.icon);
                requestedIcons.Add(item.icon);
                await Task.Delay(1000 + Random.Shared.Next(-200, 201));
                if (requestedIcons.Count % 10 == 0)
                {
                    writer.RequestChangeZone(session.Player.Location?.zoneId ?? 0);
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {requestedIcons.Count} icons");
                }
            }
            //npc
            //while (session.Data.NpcTemplates.Length == 0)
            //{
            //    await Task.Delay(1000 + random.Next(-200, 201));
            //    count++;
            //    if (count >= 10)
            //    {
            //        writer.RequestChangeZone(session.Player.location.zoneId);
            //        count = 0;
            //    }
            //}
            //foreach (NpcTemplate npc in session.Data.NpcTemplates)
            //{
            //    Part? partHead = npc.headId == -1 ? null : session.Data.Parts[npc.headId];
            //    Part? partBody = npc.bodyId == -1 ? null : session.Data.Parts[npc.bodyId];
            //    Part? partLeg = npc.legId == -1 ? null : session.Data.Parts[npc.legId];
            //    RequestPartIcon(partHead, 0);
            //    RequestPartIcon(partBody, 1);
            //    RequestPartIcon(partLeg, 1);
            //}
            //parts
            List<Part> parts = [.. session.Data.Parts];
            while (parts.Count > 0)
            {
                Part part = parts[Random.Shared.Next(0, parts.Count)];
                parts.Remove(part);
                int index = 0;
                if (part.type == 1 || part.type == 2) //body, leg
                    index = 1;
                if (part.pi[index].id <= 0)
                    continue;
                await RequestPartIcon(part, index);
            }
            //skills
            while (session.Data.NClasses is null)
            {
                await Task.Delay(1000 + Random.Shared.Next(-200, 201));
                count++;
                if (count >= 10)
                {
                    writer.RequestChangeZone(session.Player.Location?.zoneId ?? 0);
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
                    await Task.Delay(1000 + Random.Shared.Next(-200, 201));
                    if (requestedIcons.Count % 10 == 0)
                    {
                        writer.RequestChangeZone(session.Player.Location?.zoneId ?? 0);
                        Console.WriteLine($"[{session.Host}:{session.Port}] Requested {requestedIcons.Count} icons");
                    }
                }
            }
            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {requestedIcons.Count} icons.");
            Console.WriteLine($"[{session.Host}:{session.Port}] Wait 10s...");
            await Task.Delay(10000);
            return true;
        }

        static async Task RequestMobsImgAsync(ISession session)
        {
            IMessageWriter writer = session.MessageWriter;
            MobTemplate[] mobTemplates = session.Data.MobTemplates;
            int templateID = 0;
            for (; templateID < mobTemplates.Length; templateID++)
            {
                writer.RequestMobTemplate((short)templateID);
                await Task.Delay(1000 + Random.Shared.Next(-200, 201));
                if (templateID % 10 == 0)
                {
                    writer.RequestChangeZone(session.Player.Location?.zoneId ?? 0);
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {templateID} mob templates");
                }
            }
            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {templateID} mob templates.");
        }

        static async Task RequestMapsTemplateAsync(ISession session)
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
                    await Task.Delay(1000 + Random.Shared.Next(-200, 201));
                    count++;
                    if (count >= 5)
                    {
                        Console.WriteLine($"[{session.Host}:{session.Port}] Failed to get map template {map.id}!");
                        break;
                    }
                }
                while (session.Data.MapToReceiveTemplate is not null);
                if (i % 10 == 0)
                {
                    writer.RequestChangeZone(session.Player.Location?.zoneId ?? 0);
                    Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} map templates");
                }
            }
            Console.WriteLine($"[{session.Host}:{session.Port}] Requested {i} map templates.");
            session.Data.MapToReceiveTemplate = null;
        }

        static async Task TryGoOutsideIfAtHomeAsync(ISession session)
        {
            IMessageWriter writer = session.MessageWriter;
            Location? location = session.Player.Location;
            while (location is null || location.mapId <= 23 && location.mapId >= 21)
            {
                Console.WriteLine("The player is at home, trying to go outside...");
                int x = 0, y = 336;
                switch (location?.mapId)
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
                    default:
                        x = 390;
                        break;
                }
                writer.CharMove(x, y);
                await Task.Delay(200);
                writer.CharMove(x, ++y);
                await Task.Delay(200);
                writer.CharMove(x, --y);
                await Task.Delay(200);
                writer.GetMapOffline();
                await Task.Delay(1000);
                writer.FinishLoadMap();
                await Task.Delay(3000);
                location = session.Player.Location;
                if (location is null)
                    Console.WriteLine($"Current map: null");
                else 
                    Console.WriteLine($"Current map: {location.mapName} [{location.mapId}], zone {location.zoneId}");
            }
        }

        static async Task<bool> TryConnectAsync(ISession session)
        {
            CancellationTokenSource cts = new CancellationTokenSource(15000);
            int retryTimes = 0;
            try
            {
                if (forceProxy)
                {
                    if (string.IsNullOrEmpty(proxyData))
                    {
                        Console.WriteLine("No proxy data provided! Falling back to direct connection...");
                        await session.ConnectAsync(cancellationToken: cts.Token);
                    }
                    else
                        return await TryConnectProxyAsync(session);
                }
                else
                    await session.ConnectAsync(cancellationToken: cts.Token);
            }
            catch
            {
                while (!session.IsConnected)
                {
                    await cts.CancelAsync();
                    cts = new CancellationTokenSource(15000);
                    try
                    {
                        await session.ConnectAsync(cancellationToken: cts.Token);
                    }
                    catch
                    {
                        if (retryTimes >= 3)
                            break;
                        Console.WriteLine($"Retry {retryTimes + 1}...");
                        await Task.Delay(1000);
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
                        return await TryConnectProxyAsync(session);
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

        static async Task<bool> TryConnectProxyAsync(ISession session)
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
            CancellationTokenSource cts = new CancellationTokenSource(15000);
            try
            {
                await session.ConnectAsync(proxyHost, proxyPort, proxyUsername, proxyPassword, proxyType, cts.Token);
                return true;
            }
            catch
            {
                while (!session.IsConnected)
                {
                    await cts.CancelAsync();
                    cts = new CancellationTokenSource(15000);
                    try
                    {
                        await session.ConnectAsync(proxyHost, proxyPort, proxyUsername, proxyPassword, proxyType, cts.Token);
                    }
                    catch
                    {
                        if (retryTimes >= 3)
                            break;
                        Console.WriteLine($"Retry {retryTimes + 1}...");
                        await Task.Delay(1000);
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
    }
}
