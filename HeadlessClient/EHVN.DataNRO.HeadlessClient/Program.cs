using EHVN.DragonBoyOnline.CustomMsgHandler;
using ImageMagick;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using static EHVN.DragonBoyOnline.CustomMsgHandler.GameData;
using static System.Net.Mime.MediaTypeNames;

namespace EHVN.DataNRO.HeadlessClient
{
    internal class Program
    {
        static int maxRunSeconds = 3600;
        static int[] overwriteIconIDs = [];
        static bool loggedIn = false;

        static async Task FailoverTask()
        {
            Console.WriteLine($"DataNRO.HeadlessClient will run for {maxRunSeconds} seconds!");
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(maxRunSeconds * 1000 / 10);
                Console.WriteLine($"DataNRO.HeadlessClient has been running for {(i + 1) * maxRunSeconds / 10} seconds...");
            }
            Console.WriteLine($"DataNRO.HeadlessClient has been running for {maxRunSeconds} seconds, exiting...");
            Environment.Exit(1);
        }

        static int Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
#if !DEBUG
            _ = FailoverTask().ConfigureAwait(false);
#endif
            string? overwriteIconsEnv = Environment.GetEnvironmentVariable("OVERWRITE_ICONS");
            if (!string.IsNullOrEmpty(overwriteIconsEnv))
                overwriteIconIDs = overwriteIconsEnv.Split(',').Select(int.Parse).ToArray();
            string? maxRunSecondsEnv = Environment.GetEnvironmentVariable("MAX_RUN_SECONDS");
            if (!string.IsNullOrEmpty(maxRunSecondsEnv))
                maxRunSeconds = int.Parse(maxRunSecondsEnv);
            string? data = Environment.GetEnvironmentVariable("DATA");
            if (string.IsNullOrEmpty(data))
            {
#if DEBUG
                Console.Write("DATA: ");
                data = Console.ReadLine() ?? "";
#else
                Console.WriteLine("DATA environment variable is not set!");
                return 1;
#endif
            }
            try
            {
                bool success = RunHeadlessClientAsync(data).GetAwaiter().GetResult();
#if DEBUG
                Console.Write("Press Enter key to exit...");
                Console.ReadLine();
#endif
                return success ? 0 : 1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
#if DEBUG
                Console.Write("Press Enter key to exit...");
                Console.ReadLine();
#endif
                return 1;
            }
        }

        static async Task<bool> RunHeadlessClientAsync(string data)
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
            if (!LoadMessageHandlerModule(type, out IGameSession session))
                return false;
            RegisterEventListeners(session);
            session.Data.Path = dataPath;
            session.Data.SaveIcon = requestAndSaveIcons;
            session.Data.OverwriteIconIDs = overwriteIconIDs;
            foreach (int iconId in overwriteIconIDs)    //-1 will not delete all icons
            {
                if (File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{iconId}.png"))
                    File.Delete($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{iconId}.png");
            }
            if (!await ConnectAsync(session, host, port))
                return false;
            if (!await GetDataAsync(session, account, password, unregisteredUser))
                return false;
            Console.WriteLine($"Disconnect from {session.Host}:{session.Port} in 20s...");
            await Task.Delay(20000);
            session.Disconnect();

            if (session.Data.SaveIcon)
                await ProcessImagesAsync(session);
            Console.WriteLine($"Writing data to {session.Data.Path}\\...");
            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                WriteIndented = false,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };
            if (session.Data.Maps.Count > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.Maps)}.json", JsonSerializer.Serialize(session.Data.Maps, options));
            if (session.Data.NpcTemplates.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.NpcTemplates)}.json", JsonSerializer.Serialize(session.Data.NpcTemplates, options));
            if (session.Data.MobTemplates.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.MobTemplates)}.json", JsonSerializer.Serialize(session.Data.MobTemplates, options));
            if (session.Data.ItemOptionTemplates.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.ItemOptionTemplates)}.json", JsonSerializer.Serialize(session.Data.ItemOptionTemplates, options));
            if (session.Data.NClasses.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.NClasses)}.json", JsonSerializer.Serialize(session.Data.NClasses, options));
            if (session.Data.ItemTemplates.Count > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.ItemTemplates)}.json", JsonSerializer.Serialize(session.Data.ItemTemplates, options));;
            if (session.Data.Parts.Length > 0)
                File.WriteAllText($"{session.Data.Path}\\{nameof(GameData.Parts)}.json", JsonSerializer.Serialize(session.Data.Parts, options));
            //if (session.Data.SaveIcon)
            //File.WriteAllText($"{Path.GetDirectoryName(session.Data.Path)}\\{nameof(GameData.MobTemplateEffectData)}.json", JsonSerializer.Serialize(session.Data.MobTemplateEffectData, options));
            File.WriteAllText($"{session.Data.Path}\\LastUpdated", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
            await Task.Delay(3000);
            session.FileWriter.DeleteTempFiles();
            session.Dispose();
            return true;
        }

        static async Task<bool> ConnectAsync(IGameSession session, string host, ushort port)
        {
            Console.WriteLine($"Connecting to {host}:{port}...");
            CancellationTokenSource cts = new CancellationTokenSource(15000);
            int retry = 0;
            for (; retry < 5; retry++)
            {
                try
                {
                    await session.ConnectAsync(host, port, cts.Token);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to connect to {host}:{port} (attempt {retry + 1}/5)!\r\n{ex}");
                    await cts.CancelAsync();
                    cts = new CancellationTokenSource(15000);
                }
            }
            if (retry == 5)
            {
                Console.WriteLine($"Failed to connect to {host}:{port} after 5 attempts!");
                return false;
            }
            Console.WriteLine("Connected successfully!");
            return true;
        }

        static async Task<bool> GetDataAsync(IGameSession session, string account, string password, string unregisteredUser)
        {
            MessageSenderBase sender = session.GetType().GetProperty("Sender")?.GetValue(session) as MessageSenderBase ?? throw new Exception("Failed to get MessageSender from session!");
            sender.SetClientType(Config.clientType, Config.zoomLevel, Config.screenWidth / Config.zoomLevel, Config.screenHeight / Config.zoomLevel);
            session.Data.ZoomLevel = Config.zoomLevel;
            await Task.Delay(2000);
            sender.ImageSource();
            await Task.Delay(1000);
            if (session.Data.SaveIcon)
            {
                Console.WriteLine($"Downloading data...");
                sender.GetResource(1);
                do await Task.Delay(1000); while (!session.Data.AllResourceLoaded);
            }
            if (!await LoginAsync(sender, account, password, unregisteredUser))
                return false;
            CancellationTokenSource changeZoneCts = new CancellationTokenSource();
            _ = ChangeZoneTask(sender, changeZoneCts.Token).ConfigureAwait(false);
            if (session.Data.SaveIcon)
            {
                if (session.Data.MapTileIDs.Count == 0)
                    Console.WriteLine($"No map tile IDs found, skipping map templates...");
                else
                    await RequestMapsTemplateAsync(session, sender);
                await RequestMobsImgAsync(session, sender);
                if (!await RequestIconsAsync(session, sender))
                {
                    await changeZoneCts.CancelAsync();
                    return false;
                }
            }
            await changeZoneCts.CancelAsync();
            return true;
        }

        static async Task<bool> LoginAsync(MessageSenderBase sender, string account, string password, string unregisteredUser)
        {
            int retryLogin = 0;
            for (; retryLogin < 3; retryLogin++)
            {
                loggedIn = false;
                if (!string.IsNullOrEmpty(unregisteredUser))
                {
                    Console.WriteLine($"Login as {unregisteredUser.Substring(0, 4)}{new string('*', unregisteredUser.Length - 4)}");
                    sender.Login(unregisteredUser, "a", false);
                }
                else
                {
                    Console.WriteLine($"Login as {new string('*', account.Length)}");
                    sender.Login(account, password, true);
                }
                await Task.Delay(1000);
                sender.UpdateMap();
                sender.UpdateSkill();
                sender.UpdateItem();
                sender.UpdateData();
                await Task.Delay(1000);
                sender.ClientOk();
                sender.FinishUpdate();
                await Task.Delay(4000);
                sender.FinishLoadMap();
                await Task.Delay(5000);
                int checkLoggedInCount = 0;
                for (; checkLoggedInCount < 20; checkLoggedInCount++)
                {
                    if (loggedIn)
                        break;
                    await Task.Delay(1000);
                }
                if (checkLoggedInCount == 20)
                {
                    if (retryLogin == 2)
                        break;
                    Console.WriteLine($"Login failed, retrying... (attempt {retryLogin + 1}/3)");
                    continue;
                }
                Console.WriteLine("Login successful!");
                await Task.Delay(2000);
                break;
            }
            if (retryLogin == 2)
            {
                Console.WriteLine($"Login failed after 3 attempts!");
                return false;
            }
            return true;
        }

        static async Task<bool> RequestIconsAsync(IGameSession session, MessageSenderBase writer)
        {
            List<int> requestedIcons = [];
            int count = 0;
            writer.UpdateData();
            while (session.Data.Parts.Length == 0)
            {
                count++;
                if (count >= 60)
                {
                    Console.WriteLine($"Get parts failed!");
                    return false;
                }
                await Task.Delay(1000);
            }
            count = 0;
            while (session.Data.ItemTemplates is null || session.Data.NClasses is null)
            {
                await Task.Delay(1000);
                count++;
                if (count >= 60 && (session.Data.ItemTemplates is null || session.Data.NClasses is null))
                {
                    if (session.Data.ItemTemplates is null)
                        Console.WriteLine($"Get item templates failed!");
                    else if (session.Data.NClasses is null)
                        Console.WriteLine($"Get classes failed!");
                    return false;
                }
            }
            List<int> iconIDs = [];
            for (int i = 0; i < session.Data.MaxSmall; i++)
                iconIDs.Add(i);
            while (iconIDs.Count > 0)
            {
                int index = Random.Shared.Next(0, iconIDs.Count);
                int id = iconIDs[index];
                iconIDs.RemoveAt(index);
                if (requestedIcons.Contains(id))
                    continue;
                if (!session.Data.CanOverwriteIcon(id) && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{id}.png"))
                    continue;
                writer.RequestIcon(id);
                requestedIcons.Add(id);
                await Task.Delay(1000 + Random.Shared.Next(-200, 201));
                if (requestedIcons.Count % 10 == 0)
                    Console.WriteLine($"Requested {requestedIcons.Count} icons");
            }
            Console.WriteLine($"Requested {requestedIcons.Count} icons.");
            Console.WriteLine($"Wait 10s...");
            await Task.Delay(10000);
            return true;
        }

        static async Task RequestMobsImgAsync(IGameSession session, MessageSenderBase writer)
        {
            MobTemplate[] mobTemplates = session.Data.MobTemplates;
            int templateID = 0;
            for (; templateID < mobTemplates.Length; templateID++)
            {
                writer.RequestMobTemplate((short)templateID);
                await Task.Delay(1000 + Random.Shared.Next(-200, 201));
                if ((templateID + 1) % 10 == 0)
                    Console.WriteLine($"Requested {templateID + 1} mob templates");
            }
            Console.WriteLine($"Requested {templateID} mob templates.");
        }

        static async Task RequestMapsTemplateAsync(IGameSession session, MessageSenderBase writer)
        {
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
                        Console.WriteLine($"Failed to get map template {map.id}!");
                        break;
                    }
                }
                while (session.Data.MapToReceiveTemplate is not null);
                if ((i + 1) % 10 == 0)
                    Console.WriteLine($"Requested {i + 1} map templates");
            }
            Console.WriteLine($"Requested {i} map templates.");
            session.Data.MapToReceiveTemplate = null;
        }

        static async Task ProcessImagesAsync(IGameSession session)
        {
            await SplitBigImgsAsync(session);
            CombineNPCImages(session);
            CombineMobImages(session);
            if (session.Data.MapTileIDs.Count == 0)
                Console.WriteLine($"No map tile IDs found, skipping map images...");
            else
                CombineMapImages(session);
        }

        static async Task SplitBigImgsAsync(IGameSession session)
        {
            Console.WriteLine($"Splitting images...");
            List<MagickImage> smallImages = [];
            for (int i = 0; File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\BigIcons\\Big{i}.png"); i++)
                smallImages.Add(new MagickImage($"{Path.GetDirectoryName(session.Data.Path)}\\BigIcons\\Big{i}.png"));
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
                uint width = (uint)(smallImg[3] * session.Data.ZoomLevel);
                uint height = (uint)(smallImg[4] * session.Data.ZoomLevel);
                using var icon = smallImages[imgBigIndex].CloneArea(new MagickGeometry(x, y, width, height));
                await icon.WriteAsync($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{id}.png");
            }
        }

        static void CombineNPCImages(IGameSession session)
        {
            Console.WriteLine($"Combining NPC images...");
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
                    MagickImage? imgHead = null, imgBody = null, imgLeg = null;
                    if (partHead is not null && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partHead.pi[0].id}.png"))
                        imgHead = new MagickImage($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partHead.pi[0].id}.png");
                    if (partBody is not null && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partBody.pi[1].id}.png"))
                        imgBody = new MagickImage($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partBody.pi[1].id}.png");
                    if (partLeg is not null && File.Exists($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partLeg.pi[1].id}.png"))
                        imgLeg = new MagickImage($"{Path.GetDirectoryName(session.Data.Path)}\\Icons\\{partLeg.pi[1].id}.png");
                    uint maxWidth = 0, maxHeight = 0;
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
                    using (MagickImage imgNpc = new MagickImage(MagickColors.Transparent, maxWidth * 3, maxHeight * 3))
                    {
                        float cx = imgNpc.Width / 2f / session.Data.ZoomLevel;
                        float cy = imgNpc.Height / 2f / session.Data.ZoomLevel;
                        if (imgHead is not null && partHead is not null)
                            imgNpc.Composite(imgHead, (int)((cx + -13 + partHead.pi[0].dx) * session.Data.ZoomLevel), (int)((cy - 34 + partHead.pi[0].dy) * session.Data.ZoomLevel), CompositeOperator.Over);
                        if (imgLeg is not null && partLeg is not null)
                            imgNpc.Composite(imgLeg, (int)((cx + -8 + partLeg.pi[1].dx) * session.Data.ZoomLevel), (int)((cy - 10 + partLeg.pi[1].dy) * session.Data.ZoomLevel), CompositeOperator.Over);
                        if (imgBody is not null && partBody is not null)
                            imgNpc.Composite(imgBody, (int)((cx + -9 + partBody.pi[1].dx) * session.Data.ZoomLevel), (int)((cy - 16 + partBody.pi[1].dy) * session.Data.ZoomLevel), CompositeOperator.Over);
                        string path = $"{Path.GetDirectoryName(session.Data.Path)}\\NPCs";
                        if (!Directory.Exists(path))
                            Directory.CreateDirectory(path);
                        using var croppedImg = CropToContent(imgNpc);
                        AddWatermark(croppedImg, 0, 30, 15);
                        croppedImg.Write($"{path}\\{npc.npcTemplateId}.png");
                    }
                    imgHead?.Dispose();
                    imgBody?.Dispose();
                    imgLeg?.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to combine NPC image for template ID {i}!\r\n{ex}");
                }
            }
        }

        static void CombineMobImages(IGameSession session)
        {
            Console.WriteLine($"Combining monster images...");
            for (int templateId = 0; templateId < session.Data.MobTemplates.Length; templateId++)
            {
                MobTemplate template = session.Data.MobTemplates[templateId];
                EffectData effectData = session.Data.MobTemplateEffectData[templateId];
                if (effectData.frame.Length == 0)
                    continue;
                Frame frame = effectData.frame[0];
                using MagickImage mobImg = new MagickImage($"{Path.GetDirectoryName(session.Data.Path)}\\MobImg\\{templateId}.png");
                using (MagickImage monster = new MagickImage(MagickColors.Transparent, mobImg.Width * 3, mobImg.Height * 3))
                {
                    int x = (int)(monster.Width / 2f / session.Data.ZoomLevel);
                    int y = (int)(monster.Height / 2f / session.Data.ZoomLevel);
                    for (int i = 0; i < frame.dx.Length; i++)
                    {
                        ImageInfo? imageInfo = effectData.imgInfo.FirstOrDefault(img => img.id == frame.idImg[i]);
                        if (imageInfo is null)
                            continue;
                        try
                        {
                            using var cropped = mobImg.CloneArea(imageInfo.x0 * session.Data.ZoomLevel, imageInfo.y0 * session.Data.ZoomLevel, (uint)(imageInfo.w * session.Data.ZoomLevel), (uint)(imageInfo.h * session.Data.ZoomLevel));
                            monster.Composite(cropped, (x + frame.dx[i]) * session.Data.ZoomLevel, (y + frame.dy[i]) * session.Data.ZoomLevel, CompositeOperator.Over);
                        }
                        catch { }
                    }
                    string path = $"{Path.GetDirectoryName(session.Data.Path)}\\Monsters";
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                    using var croppedImg = CropToContent(monster);
                    AddWatermark(croppedImg, 0, 30, 15);
                    croppedImg.Write($"{path}\\{templateId}.png");
                }
            }
        }

        static void CombineMapImages(IGameSession session)
        {
            Console.WriteLine($"Combining map images...");
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
                    Console.WriteLine($"Failed to combine map image for map ID {map.id}!\r\n{ex}");
                }
            }
        }

        static void CombineMapImages(IGameSession session, Map map, int tileID, bool includeTileID = false)
        {
            MapTemplate? mapTemplate = map.mapTemplate;
            if (mapTemplate is null)
                return;
            int mapID = map.id;
            uint pixelWidth = (uint)((mapTemplate.width - 2) * 24 * session.Data.ZoomLevel);
            uint pixelHeight = (uint)((mapTemplate.height - 1) * 24 * session.Data.ZoomLevel);
            using MagickImage mapImg = new MagickImage(MagickColors.Transparent, pixelWidth, pixelHeight);
            using MagickImage imgWaterfall = new MagickImage($"{Path.GetDirectoryName(session.Data.Path)}\\Resources\\wtf");
            using MagickImage imgTopWaterfall = new MagickImage($"{Path.GetDirectoryName(session.Data.Path)}\\Resources\\twtf");
            void paintTile(int frame, int x, int y)
            {
                x -= 24;
                x *= session.Data.ZoomLevel;
                y *= session.Data.ZoomLevel;
                try
                {
                    using MagickImage frameImg = new MagickImage($"{Path.GetDirectoryName(session.Data.Path)}\\Resources\\{tileID}${frame + 1}");
                    mapImg.Composite(frameImg, x, y, CompositeOperator.Over);
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
                    {
                        using var cropped = imgWaterfall.CloneArea(0, 0, (uint)(24 * session.Data.ZoomLevel), (uint)(24 * session.Data.ZoomLevel));
                        mapImg.Composite(cropped, (x - 1) * 24 * session.Data.ZoomLevel, y * 24 * session.Data.ZoomLevel, CompositeOperator.Over);
                    }
                    else if ((mapTemplate.TileTypeAt(x, y) & 0x80) == 128)
                    {
                        using var cropped = imgTopWaterfall.CloneArea(0, 0, (uint)(24 * session.Data.ZoomLevel), (uint)(24 * session.Data.ZoomLevel));
                        mapImg.Composite(cropped, (x - 1) * 24 * session.Data.ZoomLevel, y * 24 * session.Data.ZoomLevel, CompositeOperator.Over);
                    }
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
            for (int i = 0; i < 3; i++)
                AddWatermark(mapImg, Random.Shared.Next(0, (int)(pixelWidth - 500)), Random.Shared.Next(0, (int)(pixelHeight - 100)), 60);
            if (includeTileID)
                mapImg.Write($"{$"{Path.GetDirectoryName(session.Data.Path)}\\Maps"}\\{mapID}-{tileID}.png");
            else
                mapImg.Write($"{$"{Path.GetDirectoryName(session.Data.Path)}\\Maps"}\\{mapID}.png");
        }

        static async Task ChangeZoneTask(MessageSenderBase sender, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                for (int i = 0; i < 30; i++)
                {
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    await Task.Delay(1000 + Random.Shared.Next(-200, 201), cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                        return;
                    sender.RequestChangeZone((byte)Random.Shared.Next(0, 3));
                }
            }
        }

        static bool LoadMessageHandlerModule(string type, out IGameSession session)
        {
            session = null!;
            try
            {
                Console.WriteLine($"Creating session type \"{type}\" from assembly \"EHVN.DragonBoyOnline.{type}MsgHandler.dll\"...");
                Assembly assembly = Assembly.LoadFile(Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? "", $"EHVN.DragonBoyOnline.{type}MsgHandler.dll"));
                Console.WriteLine($"Loaded assembly: {assembly.FullName}");
                Console.WriteLine($"Assembly name: {assembly.GetCustomAttribute<AssemblyTitleAttribute>()!.Title}");
                Console.WriteLine($"Assembly description: {assembly.GetCustomAttribute<AssemblyDescriptionAttribute>()!.Description}");
                Console.WriteLine($"Assembly company: {assembly.GetCustomAttribute<AssemblyCompanyAttribute>()!.Company}");
                Console.WriteLine($"Assembly copyright: {assembly.GetCustomAttribute<AssemblyCopyrightAttribute>()!.Copyright}");
                string sessionTypeName = $"EHVN.DragonBoyOnline.{type}MsgHandler.{type}GameSession";
                Console.WriteLine($"Creating session type: {sessionTypeName}...");
                Type? sessionType = assembly.GetType(sessionTypeName);
                if (sessionType is null)
                {
                    Console.WriteLine($"Failed to get session type \"{sessionTypeName}\" from assembly \"EHVN.DragonBoyOnline.{type}MsgHandler.dll\"!");
                    return false;
                }
                var s = (IGameSession?)Activator.CreateInstance(sessionType);
                if (s is null)
                {
                    Console.WriteLine($"Failed to create session type \"{sessionType}\" from assembly \"EHVN.DragonBoyOnline.{type}MsgHandler.dll\"!");
                    return false;
                }
                session = s;
                Console.WriteLine($"Session type \"{type}\" has been created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create session type \"{type}\" from assembly \"DataNRO.{type}.dll\"!\r\n{ex}");
                return false;
            }
            if (session is null)
            {
                Console.WriteLine($"Failed to create session type \"{type}\" from assembly \"EHVN.DragonBoyOnline.{type}MsgHandler.dll\"!");
                return false;
            }
            return true;
        }

        static void RegisterEventListeners(IGameSession session)
        {
            MessageReceiverBase messageReceiver = session.GetType().GetProperty("Receiver")?.GetValue(session) as MessageReceiverBase ?? throw new Exception("Failed to get MessageReceiver from session!");

            messageReceiver.EventListeners.IPAddressListReceived += (msg) => Console.WriteLine($"IP address list received:\r\n" + msg);
            messageReceiver.EventListeners.DialogMessageReceived += (msg) => Console.WriteLine($"Dialog message received:\r\n" + msg);
            messageReceiver.EventListeners.ServerMessageReceived += (msg) => Console.WriteLine($"Server message received:\r\n" + msg);
            messageReceiver.EventListeners.ServerAlertReceived += (msg) => Console.WriteLine($"Server alert received:\r\n" + msg);
            messageReceiver.EventListeners.ServerChatReceived += (name, msg) => Console.WriteLine($"Server chat received from {name}:\r\n{msg}");
            messageReceiver.EventListeners.PrivateChatReceived += (name, msg) => Console.WriteLine($"Private chat received from {name}:\r\n{msg}");
            messageReceiver.EventListeners.ServerNotificationReceived += (msg) => Console.WriteLine($"Server notification received:\r\n" + msg);
            messageReceiver.EventListeners.UnknownMessageReceived += (msg) => Console.WriteLine($"Unknown message received:\r\n" + msg);

            messageReceiver.EventListeners.GameNotificationReceived += (msg) =>
            {
                Console.WriteLine($"Game notification received:\r\n" + msg);
                if (msg.StartsWith("Nhiệm vụ của bạn là") || msg.StartsWith("Your mission is") || msg.StartsWith("Misimu adalah"))
                    loggedIn = true;
            };
        }

        static IMagickImage CropToContent(MagickImage source)
        {
            uint width = source.Width;
            uint height = source.Height;
            uint minX = width, minY = height, maxX = 0, maxY = 0;
            using var pixels = source.GetPixels();
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var pixelColor = pixels.GetPixel(x, y).ToColor();
                    if (pixelColor?.A == 0)
                        continue;
                    minX = (uint)Math.Min(minX, x);
                    minY = (uint)Math.Min(minY, y);
                    maxX = (uint)Math.Max(maxX, x);
                    maxY = (uint)Math.Max(maxY, y);
                }
            }
            if (minX > maxX || minY > maxY)
                return new MagickImage(MagickColors.Transparent, 1, 1);
            return source.CloneArea((int)minX, (int)minY, maxX - minX + 1, maxY - minY + 1);
        }

        static void AddWatermark(IMagickImage img, int x, int y, int fontSize)
        {
            using MagickImage text = new MagickImage();
            MagickReadSettings settings = new MagickReadSettings
            {
                BackgroundColor = MagickColors.Transparent,
                Width = img.Width * 2,
                TextGravity = Gravity.West,
                FontPointsize = fontSize,
                FontStyle = FontStyleType.Bold,
                FontWeight = FontWeight.Bold,
                FillColor = MagickColors.Yellow,
            };
            text.Read("caption:© ElectroHeavenVN", settings);
            img.Composite(text, x, y, CompositeOperator.Over);
        }
    }
}
