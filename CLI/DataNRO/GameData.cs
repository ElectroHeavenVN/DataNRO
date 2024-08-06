﻿using System.Collections.Generic;

namespace DataNRO
{
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
            public int id, type, gender, level, strRequire, iconID, part;
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
            public int id, maxPoint, manaUseType, type, iconId;
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

        public NpcTemplate[] NpcTemplates { get; set; }
        public MobTemplate[] MobTemplates { get; set; }
        public ItemOptionTemplate[] ItemOptionTemplates { get; set; }
        public NClass[] NClasses { get; set; }
        public List<Map> Maps { get; set; } = new List<Map>();
        public List<ItemTemplate> ItemTemplates { get; set; } = new List<ItemTemplate>();

        public void Reset()
        {
            NpcTemplates = null;
            MobTemplates = null;
            ItemOptionTemplates = null;
            NClasses = null;
            Maps = new List<Map>();
            ItemTemplates = new List<ItemTemplate>();
        }
    }
}