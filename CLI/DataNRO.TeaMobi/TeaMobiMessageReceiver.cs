using System;
using System.IO;
using DataNRO.Interfaces;
using static DataNRO.GameData;

namespace DataNRO.TeaMobi
{
    public class TeaMobiMessageReceiver : IMessageReceiver
    {
        TeaMobiSession session;

        internal TeaMobiMessageReceiver(TeaMobiSession session)
        {
            this.session = session;
        }

        public void OnMessageReceived(MessageReceive message)
        {
            switch (message.Command)
            {
                case -28:

                    //MessageNotMap
                    switch (message.ReadSByte())
                    {
                        case 6:
                            ReadMapData(message);
                            break;
                        case 7:
                            ReadSkillData(message);
                            break;
                        case 8:
                            ReadItemData(message);
                            break;
                    }
                    break;
                case -26:
                    Console.WriteLine(message.ReadString());
                    break;
                case -24:
                    ReadCurrentMapInfo(message);
                    break;
                case -67:
                    ReadIconInfo(message);
                    break;
            }
        }

        void ReadMapData(MessageReceive message)
        {
            message.ReadByte();
            int mapLength = message.ReadByte();
            for (int i = 0; i < mapLength; i++)
            {
                Map map = new Map();
                map.id = i;
                map.name = message.ReadString();
                session.Data.Maps.Add(map);
            }
            session.Data.NpcTemplates = new NpcTemplate[message.ReadByte()];
            for (int i = 0; i < session.Data.NpcTemplates.Length; i++)
            {
                NpcTemplate npcTemplate = new NpcTemplate();
                npcTemplate.npcTemplateId = i;
                npcTemplate.name = message.ReadString();
                npcTemplate.headId = message.ReadShort();
                npcTemplate.bodyId = message.ReadShort();
                npcTemplate.legId = message.ReadShort();
                npcTemplate.menu = new string[message.ReadByte()][];
                for (int j = 0; j < npcTemplate.menu.Length; j++)
                {
                    npcTemplate.menu[j] = new string[message.ReadByte()];
                    for (int k = 0; k < npcTemplate.menu[j].Length; k++)
                        npcTemplate.menu[j][k] = message.ReadString();
                }
                session.Data.NpcTemplates[i] = npcTemplate;
            }
            session.Data.MobTemplates = new MobTemplate[message.ReadByte()];
            for (sbyte i = 0; i < session.Data.MobTemplates.Length; i++)
            {
                MobTemplate mobTemplate = new MobTemplate();
                mobTemplate.mobTemplateId = i;
                mobTemplate.type = message.ReadSByte();
                mobTemplate.name = message.ReadString();
                mobTemplate.hp = message.ReadInt();
                mobTemplate.rangeMove = message.ReadSByte();
                mobTemplate.speed = message.ReadSByte();
                mobTemplate.dartType = message.ReadSByte();
                session.Data.MobTemplates[i] = mobTemplate;
            }
        }

        void ReadSkillData(MessageReceive message)
        {
            message.ReadByte();
            int skillOptionTemplateLength = message.ReadByte();
            for (int i = 0; i < skillOptionTemplateLength; i++)
            {
                string skillOptionTemplateName = message.ReadString();
            }
            session.Data.NClasses = new NClass[message.ReadByte()];
            for (int i = 0; i < session.Data.NClasses.Length; i++)
            {
                NClass nClass = new NClass();
                nClass.classId = i;
                nClass.name = message.ReadString();
                nClass.skillTemplates = new SkillTemplate[message.ReadByte()];
                for (int j = 0; j < nClass.skillTemplates.Length; j++)
                {
                    SkillTemplate skillTemplate = new SkillTemplate();
                    skillTemplate.id = message.ReadSByte();
                    skillTemplate.name = message.ReadString();
                    skillTemplate.maxPoint = message.ReadSByte();
                    skillTemplate.manaUseType = message.ReadSByte();
                    skillTemplate.type = message.ReadSByte();
                    skillTemplate.iconId = message.ReadShort();
                    skillTemplate.damInfo = message.ReadString();
                    skillTemplate.description = message.ReadString();
                    skillTemplate.skills = new Skill[message.ReadByte()];
                    for (int k = 0; k < skillTemplate.skills.Length; k++)
                    {
                        Skill skill = new Skill();
                        skill.skillId = message.ReadShort();
                        skill.point = message.ReadSByte();
                        skill.powRequire = message.ReadLong();
                        skill.manaUse = message.ReadShort();
                        skill.coolDown = message.ReadInt();
                        skill.dx = message.ReadShort();
                        skill.dy = message.ReadShort();
                        skill.maxFight = message.ReadSByte();
                        skill.damage = message.ReadShort();
                        skill.price = message.ReadShort();
                        skill.moreInfo = message.ReadString();
                        skillTemplate.skills[k] = skill;
                    }
                    nClass.skillTemplates[j] = skillTemplate;
                }
                session.Data.NClasses[i] = nClass;
            }
        }

        void ReadItemData(MessageReceive message)
        {
            message.ReadByte();
            sbyte type = message.ReadSByte();
            if (type == 0)
            {
                session.Data.ItemOptionTemplates = new ItemOptionTemplate[message.ReadByte()];
                for (int i = 0; i < session.Data.ItemOptionTemplates.Length; i++)
                {
                    ItemOptionTemplate itemOptionTemplate = new ItemOptionTemplate();
                    itemOptionTemplate.id = i;
                    itemOptionTemplate.name = message.ReadString();
                    itemOptionTemplate.type = message.ReadSByte();
                    session.Data.ItemOptionTemplates[i] = itemOptionTemplate;
                }
            }
            else if (type == 1 || type == 2)
            {
                short start = 0;
                if (type == 2)
                    start = message.ReadShort();
                short end = message.ReadShort();
                for (int i = start; i < end; i++)
                {
                    ItemTemplate itemTemplate = new ItemTemplate();
                    itemTemplate.id = i;
                    itemTemplate.type = message.ReadSByte();
                    itemTemplate.gender = message.ReadSByte();
                    itemTemplate.name = message.ReadString();
                    itemTemplate.description = message.ReadString();
                    itemTemplate.level = message.ReadSByte();
                    itemTemplate.strRequire = message.ReadInt();
                    itemTemplate.iconID = message.ReadShort();
                    itemTemplate.part = message.ReadShort();
                    itemTemplate.isUpToUp = message.ReadBool();
                    session.Data.ItemTemplates.Add(itemTemplate);
                }
            }
            else if (type == 100)
            {
                //not used
            }
            else if (type == 101)
            {
                //not used
            }
        }

        void ReadCurrentMapInfo(MessageReceive message)
        {
            Player player = session.Player;
            Location location = player.location = new Location();
            location.mapId = message.ReadByte();
            location.planetId = message.ReadSByte();
            message.ReadSByte();
            message.ReadSByte();
            message.ReadSByte();
            location.mapName = message.ReadString();
            location.zoneId = message.ReadSByte();
            //Who cares what the rest of the data is?
        }

        void ReadIconInfo(MessageReceive message)
        {
            int iconId = message.ReadInt();
            int dataLength = message.ReadInt();
            byte[] data = message.ReadBytes(dataLength);
            File.WriteAllBytes($"{session.Data.Path}\\x{Config.zoomLevel}\\{iconId}.png", data);
        }
    }
}
