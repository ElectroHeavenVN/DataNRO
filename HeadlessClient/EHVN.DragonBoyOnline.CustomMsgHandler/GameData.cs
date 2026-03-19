using System.Text.Json.Serialization;

namespace EHVN.DragonBoyOnline.CustomMsgHandler
{
    /// <summary>
    /// Class chứa dữ liệu của game.
    /// </summary>
    public class GameData
    {
        public class NpcTemplate
        {
            [JsonPropertyName(nameof(npcTemplateId))]
            public int npcTemplateId { get; set; }

            [JsonPropertyName(nameof(headId))]
            public int headId { get; set; }

            [JsonPropertyName(nameof(bodyId))]
            public int bodyId { get; set; }

            [JsonPropertyName(nameof(legId))]
            public int legId { get; set; }

            [JsonPropertyName(nameof(name))]
            public string name { get; set; } = "";

            [JsonPropertyName(nameof(menu))]
            public string[][] menu { get; set; } = [];
        }

        public class ImageInfo
        {
            [JsonPropertyName(nameof(id))]
            public int id { get; set; }

            [JsonPropertyName(nameof(x))]
            public int x { get; set; }

            [JsonPropertyName(nameof(y))]
            public int y { get; set; }

            [JsonPropertyName(nameof(x0))]
            public int x0 { get; set; }

            [JsonPropertyName(nameof(y0))]
            public int y0 { get; set; }

            [JsonPropertyName(nameof(w))]
            public int w { get; set; }

            [JsonPropertyName(nameof(h))]
            public int h { get; set; }
        }

        public class Frame
        {
            [JsonPropertyName(nameof(id))]
            public int id { get; set; }

            [JsonPropertyName(nameof(dx))]
            public short[] dx { get; set; } = [];

            [JsonPropertyName(nameof(dy))]
            public short[] dy { get; set; } = [];

            [JsonPropertyName(nameof(idImg))]
            public sbyte[] idImg { get; set; } = [];
        }

        public class EffectData
        {
            [JsonPropertyName(nameof(imgInfo))]
            public ImageInfo[] imgInfo { get; set; } = [];

            [JsonPropertyName(nameof(frame))]
            public Frame[] frame { get; set; } = [];

            [JsonPropertyName(nameof(arrFrame))]
            public short[] arrFrame { get; set; } = [];

            [JsonPropertyName(nameof(anim_data))]
            public short[][] anim_data { get; set; } = new short[16][];

            [JsonPropertyName(nameof(id))]
            public int id { get; set; }

            [JsonPropertyName(nameof(typeData))]
            public int typeData { get; set; }

            [JsonPropertyName(nameof(width))]
            public int width { get; set; }

            [JsonPropertyName(nameof(height))]
            public int height { get; set; }
        }

        public class MobTemplate
        {
            [JsonPropertyName(nameof(mobTemplateId))]
            public int mobTemplateId { get; set; }

            [JsonPropertyName(nameof(rangeMove))]
            public int rangeMove { get; set; }

            [JsonPropertyName(nameof(speed))]
            public int speed { get; set; }

            [JsonPropertyName(nameof(type))]
            public int type { get; set; }

            [JsonPropertyName(nameof(dartType))]
            public int dartType { get; set; }

            [JsonPropertyName(nameof(hp))]
            public long hp { get; set; }

            [JsonPropertyName(nameof(name))]
            public string name { get; set; } = "";
        }

        public class ItemOptionTemplate
        {
            [JsonPropertyName(nameof(id))]
            public int id { get; set; }

            [JsonPropertyName(nameof(type))]
            public int type { get; set; }

            [JsonPropertyName(nameof(name))]
            public string name { get; set; } = "";
        }

        public class ItemTemplate
        {
            [JsonPropertyName(nameof(isUpToUp))]
            public bool isUpToUp { get; set; }

            [JsonPropertyName(nameof(id))]
            public int id { get; set; }

            [JsonPropertyName(nameof(type))]
            public int type { get; set; }
            
            [JsonPropertyName(nameof(gender))]
            public int gender { get; set; }

            [JsonPropertyName(nameof(level))]
            public int level { get; set; }

            [JsonPropertyName(nameof(strRequire))]
            public int strRequire { get; set; }

            [JsonPropertyName(nameof(icon))]
            public int icon { get; set; }

            [JsonPropertyName(nameof(part))]
            public int part { get; set; }

            [JsonPropertyName(nameof(name))]
            public string name { get; set; } = "";

            [JsonPropertyName(nameof(description))]
            public string description { get; set; } = "";
        }

        public class NClass
        {
            [JsonPropertyName(nameof(classId))]
            public int classId { get; set; }

            [JsonPropertyName(nameof(name))]
            public string name { get; set; } = "";

            [JsonPropertyName(nameof(skillTemplates))]
            public SkillTemplate[] skillTemplates { get; set; } = [];
        }

        public class SkillTemplate
        {
            [JsonPropertyName(nameof(id))]
            public int id { get; set; }

            [JsonPropertyName(nameof(maxPoint))]
            public int maxPoint { get; set; }

            [JsonPropertyName(nameof(manaUseType))]
            public int manaUseType { get; set; }

            [JsonPropertyName(nameof(type))]
            public int type { get; set; }

            [JsonPropertyName(nameof(icon))]
            public int icon { get; set; }

            [JsonPropertyName(nameof(name))]
            public string name { get; set; } = "";

            [JsonPropertyName(nameof(description))]
            public string description { get; set; } = "";

            [JsonPropertyName(nameof(damInfo))]
            public string damInfo { get; set; } = "";

            [JsonPropertyName(nameof(skills))]
            public Skill[] skills { get; set; } = [];
        }

        public class Skill
        {
            [JsonPropertyName(nameof(point))]
            public int point { get; set; }

            [JsonPropertyName(nameof(maxFight))]
            public int maxFight { get; set; }

            [JsonPropertyName(nameof(manaUse))]
            public int manaUse { get; set; }

            [JsonPropertyName(nameof(skillId))]
            public int skillId { get; set; }

            [JsonPropertyName(nameof(dx))]
            public int dx { get; set; }

            [JsonPropertyName(nameof(dy))]
            public int dy { get; set; }

            [JsonPropertyName(nameof(damage))]
            public int damage { get; set; }

            [JsonPropertyName(nameof(price))]
            public int price { get; set; }

            [JsonPropertyName(nameof(coolDown))]
            public int coolDown { get; set; }

            [JsonPropertyName(nameof(powRequire))]
            public long powRequire { get; set; }

            [JsonPropertyName(nameof(moreInfo))]
            public string moreInfo { get; set; } = "";
        }

        public class Map
        {
            [JsonPropertyName(nameof(id))]
            public int id { get; set; }

            [JsonPropertyName(nameof(name))]
            public string name { get; set; } = "";

            [JsonIgnore]
            public MapTemplate? mapTemplate;
        }

        public class MapTemplate
        {
            //tmw
            public int width;
            //tmh
            public int height;
            public int[] maps = [];
            public int[] types = [];

            public static int[][] tileType = [];

            public static int[][][] tileIndex = [];

            void SetTile(int index, int[] mapsArr, int type)
            {
                for (int i = 0; i < mapsArr.Length; i++)
                {
                    if (maps[index] != mapsArr[i])
                        continue;
                    types[index] |= type;
                    break;
                }
            }

            public void LoadMap(int tileId)
            {
                int num = tileId - 1;
                for (int i = 0; i < width * height; i++)
                {
                    for (int j = 0; j < tileType[num].Length; j++)
                        SetTile(i, tileIndex[num][j], tileType[num][j]);
                }
            }

            public int TileTypeAt(int x, int y)
            {
                int index = y * width + x;
                if (index < 0 || index >= types.Length)
                    return 1000;
                return types[index];
            }
        }

        public class PartImage
        {
            [JsonPropertyName(nameof(id))]
            public int id { get; set; }

            [JsonPropertyName(nameof(dx))]
            public int dx { get; set; }

            [JsonPropertyName(nameof(dy))]
            public int dy { get; set; }
        }

        public class Part
        {
            [JsonPropertyName(nameof(type))]
            public int type { get; set; }

            [JsonPropertyName(nameof(pi))]
            public PartImage[] pi { get; set; } = [];

            public Part(int type)
            {
                this.type = type;
                if (type == 0)
                    pi = new PartImage[3];
                if (type == 1)
                    pi = new PartImage[17];
                if (type == 2)
                    pi = new PartImage[14];
                if (type == 3)
                    pi = new PartImage[2];
            }
        }

        /// <summary>
        /// Đường dẫn lưu dữ liệu game
        /// </summary>
        public string Path { get; set; } = "";

        /// <summary>
        /// Trạng thái lưu icon
        /// </summary>
        public bool SaveIcon { get; set; }

        /// <summary>
        /// Danh sách ID icon ghi đè, [-1] là ghi đè tất cả
        /// </summary>
        public int[] OverwriteIconIDs { get; set; } = [];

        public NpcTemplate[] NpcTemplates { get; set; } = [];
        public MobTemplate[] MobTemplates { get; set; } = [];
        public EffectData[] MobTemplateEffectData { get; set; } = [];
        public ItemOptionTemplate[] ItemOptionTemplates { get; set; } = [];
        public NClass[] NClasses { get; set; } = [];
        public List<Map> Maps { get; set; } = [];
        public Map? MapToReceiveTemplate { get; set; }
        public List<ItemTemplate> ItemTemplates { get; set; } = [];
        public Part[] Parts { get; set; } = [];
        public bool AllResourceLoaded { get; set; }
        public int ZoomLevel { get; set; }
        public int[][] SmallImg { get; set; } = [];
        public short MaxSmall { get; set; }

        /// <summary>
        /// Thư viện chứa cặp ID map và ID tile tương ứng
        /// </summary>
        public Dictionary<int, int> MapTileIDs { get; set; } = [];

        /// <summary>
        /// Đặt lại dữ liệu của game
        /// </summary>
        public void Reset()
        {
            NpcTemplates = [];
            MobTemplates = [];
            ItemOptionTemplates = [];
            NClasses = [];
            Maps = [];
            ItemTemplates = [];
            Parts = [];
            MaxSmall = 0;
            AllResourceLoaded = false;
        }

        /// <summary>
        /// Trạng thái có thể ghi đè icon
        /// </summary>
        /// <param name="iconID">ID icon</param>
        public bool CanOverwriteIcon(int iconID)
        {
            if (OverwriteIconIDs.Contains(-1))
                return true;
            return OverwriteIconIDs.Contains(iconID);
        }
    }

}
