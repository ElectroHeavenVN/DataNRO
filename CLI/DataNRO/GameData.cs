using System;
using System.Collections.Generic;
using System.Linq;

namespace EHVN.DataNRO
{
    /// <summary>
    /// Class chứa dữ liệu của game.
    /// </summary>
    public class GameData
    {
        public class NpcTemplate
        {
            public int npcTemplateId, headId, bodyId, legId;
            public string name = "";
            public string[][] menu = [];
        }

        public class ImageInfo
        {
            public int id, x, y, x0, y0, w, h;
        }

        public class Frame
        {
            public int id;
            public short[] dx = [];
            public short[] dy = [];
            public sbyte[] idImg = [];
        }

        public class EffectData
        {
            public ImageInfo[] imgInfo = [];
            public Frame[] frame = [];
            public short[] arrFrame = [];
            public short[][] anim_data = new short[16][];
            public int id, typeData, width, height;
        }

        public class MobTemplate
        {
            public int mobTemplateId, rangeMove, speed, type, dartType;
            public long hp;
            public string name = "";
        }

        public class ItemOptionTemplate
        {
            public int id, type;
            public string name = "";
        }

        public class ItemTemplate
        {
            public bool isUpToUp;
            public int id, type, gender, level, strRequire, icon, part;
            public string name = "";
            public string description = "";
        }

        public class NClass
        {
            public int classId;
            public string name = "";
            public SkillTemplate[] skillTemplates = [];
        }

        public class SkillTemplate
        {
            public int id, maxPoint, manaUseType, type, icon;
            public string name = "";
            public string description = "";
            public string damInfo = "";
            public Skill[] skills = [];
        }

        public class Skill
        {
            public int point, maxFight, manaUse, skillId, dx, dy, damage, price, coolDown;
            public long powRequire;
            public string moreInfo = "";
        }

        public class Map
        {
            public int id;
            public string name = "";
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
            public int id, dx, dy;
        }

        public class Part
        {
            public int type;
            public PartImage[] pi = [];

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
