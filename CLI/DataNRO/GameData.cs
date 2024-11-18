using System.Collections.Generic;
using System.Linq;

namespace DataNRO
{
    /// <summary>
    /// Class chứa dữ liệu của game.
    /// </summary>
    public class GameData
    {
        public class NpcTemplate
        {
            public int npcTemplateId, headId, bodyId, legId;
            public string name;
            public string[][] menu;
        }

        public class MobTemplate
        {
            public int mobTemplateId, rangeMove, speed, type, dartType;
            public long hp;
            public string name;
        }

        public class ItemOptionTemplate
        {
            public int id, type;
            public string name;
        }

        public class ItemTemplate
        {
            public bool isUpToUp;
            public int id, type, gender, level, strRequire, icon, part;
            public string name, description;
        }

        public class NClass
        {
            public int classId;
            public string name;
            public SkillTemplate[] skillTemplates;
        }

        public class SkillTemplate
        {
            public int id, maxPoint, manaUseType, type, icon;
            public string name, description, damInfo;
            public Skill[] skills;
        }

        public class Skill
        {
            public int point, maxFight, manaUse, skillId, dx, dy, damage, price, coolDown;
            public long powRequire;
            public string moreInfo;
        }

        public class Map
        {
            public int id;
            public string name;
        }

        public class PartImage
        {
            public int id, dx, dy;
        }

        public class Part
        {
            public int type;
            public PartImage[] pi;

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
        public string Path { get; set; }

        /// <summary>
        /// Trạng thái lưu icon
        /// </summary>
        public bool SaveIcon { get; set; }

        /// <summary>
        /// Danh sách ID icon ghi đè, [-1] là ghi đè tất cả
        /// </summary>
        public int[] OverwriteIconIDs { get; set; } = new int[0];

        public NpcTemplate[] NpcTemplates { get; set; }
        public MobTemplate[] MobTemplates { get; set; }
        public ItemOptionTemplate[] ItemOptionTemplates { get; set; }
        public NClass[] NClasses { get; set; }
        public List<Map> Maps { get; set; } = new List<Map>();
        public List<ItemTemplate> ItemTemplates { get; set; } = new List<ItemTemplate>();
        public Part[] Parts { get; set; }
        public bool AllResourceLoaded { get; set; }
        public int ZoomLevel { get; set; }
        public int[][] SmallImg { get; set; }

        /// <summary>
        /// Đặt lại dữ liệu của game
        /// </summary>
        public void Reset()
        {
            NpcTemplates = null;
            MobTemplates = null;
            ItemOptionTemplates = null;
            NClasses = null;
            Maps = new List<Map>();
            ItemTemplates = new List<ItemTemplate>();
            Parts = null;
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
