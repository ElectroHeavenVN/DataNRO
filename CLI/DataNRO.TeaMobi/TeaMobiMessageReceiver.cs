using System;
using System.Linq;
using System.Security.Cryptography;
using DataNRO.Interfaces;
using static DataNRO.GameData;

namespace DataNRO.TeaMobi
{
    public class TeaMobiMessageReceiver : IMessageReceiver
    {
        static readonly string[] blankImageHashes = new string[]
        {
            "110B964285DEB1A8D3D13562914E1E2B51F4799A85412884B481E0316358DF48",
            "A09E276301DE73E84A35169799AFEBC9B0DEE6EC73DB827BF97D604E76395433",
            "95DB5A048C9A4A9AB381FBD97CD7B724112FB11CDEC8E7A083AC2A366D2E9CF0"
        };

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
                case -29:
                    if (message.ReadSByte() != 2)
                        break;
                    Console.WriteLine("IP address list received: " + message.ReadString());
                    break;
                case -26:
                    Console.WriteLine("Message received: " + message.ReadString());
                    break;
                case -24:
                    ReadCurrentMapInfo(message);
                    break;
                case -67:
                    ReadIcon(message);
                    break;
                case -87:
                    ReadCommonData(message);
                    break;
                case -74:
                    ReadResource(message);
                    break;
                case 11:
                    ReadMobTemplate(message);
                    break;
                case 12:    //read_cmdExtraBig
                    byte b = message.ReadByte();
                    if (b == 0)
                        ReadItemData(message);
                    break;
            }
        }

        void ReadMapData(MessageReceive message)
        {
            message.ReadByte();
            int mapLength = message.ReadShort();
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
            session.Data.MobTemplates = new MobTemplate[message.ReadShort()];
            session.Data.MobTemplateEffectData = new EffectData[session.Data.MobTemplates.Length];
            for (sbyte i = 0; i < session.Data.MobTemplates.Length; i++)
            {
                MobTemplate mobTemplate = new MobTemplate();
                mobTemplate.mobTemplateId = i;
                mobTemplate.type = message.ReadSByte();
                mobTemplate.name = message.ReadString();
                mobTemplate.hp = message.ReadLong();
                mobTemplate.rangeMove = message.ReadSByte();
                mobTemplate.speed = message.ReadSByte();
                mobTemplate.dartType = message.ReadSByte();
                session.Data.MobTemplates[i] = mobTemplate;

                session.Data.MobTemplateEffectData[i] = new EffectData();
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
                    skillTemplate.icon = message.ReadShort();
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
                session.Data.ItemOptionTemplates = new ItemOptionTemplate[message.ReadShort()];
                for (int i = 0; i < session.Data.ItemOptionTemplates.Length; i++)
                {
                    ItemOptionTemplate itemOptionTemplate = new ItemOptionTemplate();
                    itemOptionTemplate.id = i;
                    itemOptionTemplate.name = message.ReadString();
                    itemOptionTemplate.type = message.ReadSByte();
                    session.Data.ItemOptionTemplates[i] = itemOptionTemplate;
                }
            }
            else if (type == 1)
            {
                short start = 0;
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
                    itemTemplate.icon = message.ReadShort();
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

        void ReadIcon(MessageReceive message)
        {
            if (!session.Data.SaveIcon)
                return;
            int iconId = message.ReadInt();
            byte[] data = message.ReadBytes();
            if (data.Length < 500)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(data);
                    if (blankImageHashes.Contains(BitConverter.ToString(hash).Replace("-", "")))
                        return;
                }
            }
            session.FileWriter.WriteIcon(iconId, data);
        }

        void ReadCommonData(MessageReceive message)
        {
            message.ReadByte();
            byte[] nr_dart = message.ReadBytes();
            byte[] nr_arrow = message.ReadBytes();
            byte[] nr_effect = message.ReadBytes();
            byte[] nr_image = message.ReadBytes();
            byte[] nr_part = message.ReadBytes();
            byte[] nr_skill = message.ReadBytes();

            using (MessageReceive partReader = new MessageReceive(0, nr_part))
            {
                Part[] parts = new Part[partReader.ReadShort()];
                for (int i = 0; i < parts.Length; i++)
                {
                    parts[i] = new Part(partReader.ReadSByte());
                    for (int j = 0; j < parts[i].pi.Length; j++)
                    {
                        PartImage partImage = new PartImage();
                        partImage.id = partReader.ReadShort();
                        partImage.dx = partReader.ReadSByte();
                        partImage.dy = partReader.ReadSByte();
                        parts[i].pi[j] = partImage;
                    }
                }
                session.Data.Parts = parts;
            }

            using (MessageReceive imageReader = new MessageReceive(0, nr_image))
            {
                session.Data.SmallImg = new int[imageReader.ReadShort()][];
                for (int i = 0; i < session.Data.SmallImg.Length; i++)
                    session.Data.SmallImg[i] = new int[5];
                for (int i = 0; i < session.Data.SmallImg.Length; i++)
                {
                    session.Data.SmallImg[i][0] = imageReader.ReadByte();
                    session.Data.SmallImg[i][1] = imageReader.ReadShort();
                    session.Data.SmallImg[i][2] = imageReader.ReadShort();
                    session.Data.SmallImg[i][3] = imageReader.ReadShort();
                    session.Data.SmallImg[i][4] = imageReader.ReadShort();
                }
            }
        }

        void ReadResource(MessageReceive message)
        {
            if (!session.Data.SaveIcon)
                return;
            byte b = message.ReadByte();
            switch (b)
            {
                case 0:
                    break;
                case 1:
                    message.ReadShort();
                    session.MessageWriter.GetResource(2);
                    break;
                case 2:
                    string fileName = message.ReadString();
                    if (!fileName.Contains("Big"))
                        break;
                    fileName = fileName.Substring(fileName.IndexOf("Big"));
                    byte[] data = message.ReadBytes();
                    session.FileWriter.WriteBigIcon(fileName, data);
                    break;
                case 3:
                    message.ReadInt();
                    session.Data.AllResourceLoaded = true;
                    break;
            }
        }

        void ReadMobTemplate(MessageReceive message)
        {
            int templateID = message.ReadShort();
            byte type = message.ReadByte();
            byte[] data = message.ReadBytes();
            if (type == 0)
                ReadMobData(templateID, data);
            else
                ReadNewMobData(templateID, data, type);
            byte[] imgData = message.ReadBytes();
            session.FileWriter.WriteMobImg(templateID, imgData);
            byte typeData = message.ReadByte();
        }

        void ReadMobData(int templateID, byte[] data)
        {
            MessageReceive reader = new MessageReceive(0, data);
            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;
            EffectData effectData = session.Data.MobTemplateEffectData[templateID];
            try
            {
                ImageInfo[] imgInfos = new ImageInfo[reader.ReadByte()];
                for (int i = 0; i < imgInfos.Length; i++)
                {
                    ImageInfo imgInfo = new ImageInfo();
                    imgInfo.id = reader.ReadSByte();
                    imgInfo.x0 = reader.ReadByte();
                    imgInfo.y0 = reader.ReadByte();
                    imgInfo.w = reader.ReadByte();
                    imgInfo.h = reader.ReadByte();
                    imgInfos[i] = imgInfo;
                }
                effectData.imgInfo = imgInfos;
                Frame[] frame = new Frame[reader.ReadShort()];
                for (int i = 0; i < frame.Length; i++)
                {
                    frame[i] = new Frame();
                    frame[i].dx = new short[reader.ReadByte()];
                    frame[i].dy = new short[frame[i].dx.Length];
                    frame[i].idImg = new sbyte[frame[i].dx.Length];
                    for (int j = 0; j < frame[i].dx.Length; j++)
                    {
                        frame[i].dx[j] = reader.ReadShort();
                        frame[i].dy[j] = reader.ReadShort();
                        frame[i].idImg[j] = reader.ReadSByte();
                        if (i == 0)
                        {
                            if (left > frame[i].dx[j])
                                left = frame[i].dx[j];
                            if (top > frame[i].dy[j])
                                top = frame[i].dy[j];
                            if (right < frame[i].dx[j] + imgInfos[frame[i].idImg[j]].w)
                                right = frame[i].dx[j] + imgInfos[frame[i].idImg[j]].w;
                            if (bottom < frame[i].dy[j] + imgInfos[frame[i].idImg[j]].h)
                                bottom = frame[i].dy[j] + imgInfos[frame[i].idImg[j]].h;
                            effectData.width = right - left;
                            effectData.height = bottom - top;
                        }
                    }
                }
                effectData.frame = frame;
                short[] arrFrame = new short[reader.ReadShort()];
                if (effectData.id >= 201)
                {
                    short index = 0;
                    short[] array = new short[arrFrame.Length];
                    int count = 0;
                    bool flag = false;
                    for (int i = 0; i < array.Length; i++)
                    {
                        arrFrame[i] = reader.ReadShort();
                        if (arrFrame[i] + 500 >= 500)
                        {
                            array[count++] = arrFrame[i];
                            flag = true;
                            continue;
                        }
                        index = (short)Math.Abs(arrFrame[i] + 500);
                        effectData.anim_data[index] = new short[count];
                        Array.Copy(array, 0, effectData.anim_data[index], 0, count);
                        count = 0;
                    }
                    if (!flag)
                    {
                        effectData.anim_data[0] = new short[count];
                        Array.Copy(array, 0, effectData.anim_data[index], 0, count);
                        return;
                    }
                    for (int i = 0; i < 16; i++)
                    {
                        if (effectData.anim_data[i] == null)
                            effectData.anim_data[i] = effectData.anim_data[2];
                    }
                }
                else
                {
                    for (int i = 0; i < arrFrame.Length; i++)
                        arrFrame[i] = reader.ReadShort();
                }
                effectData.arrFrame = arrFrame;
            }
            catch { }
        }

        void ReadNewMobData(int templateID, byte[] data, byte typeRead)
        {
            MessageReceive reader = new MessageReceive(0, data);
            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;
            EffectData effectData = session.Data.MobTemplateEffectData[templateID];
            try
            {
                ImageInfo[] imgInfos = new ImageInfo[reader.ReadByte()];
                for (int i = 0; i < imgInfos.Length; i++)
                {
                    imgInfos[i] = new ImageInfo();
                    imgInfos[i].id = reader.ReadByte();
                    if (typeRead == 1)
                    {
                        imgInfos[i].x0 = reader.ReadByte();
                        imgInfos[i].y0 = reader.ReadByte();
                    }
                    else
                    {
                        imgInfos[i].x0 = reader.ReadShort();
                        imgInfos[i].y0 = reader.ReadShort();
                    }
                    imgInfos[i].w = reader.ReadByte();
                    imgInfos[i].h = reader.ReadByte();
                }
                effectData.imgInfo = imgInfos;
                Frame[] frames = new Frame[reader.ReadShort()];
                for (int i = 0; i < frames.Length; i++)
                {
                    frames[i] = new Frame();
                    frames[i].dx = new short[reader.ReadByte()];
                    frames[i].dy = new short[frames[i].dx.Length];
                    frames[i].idImg = new sbyte[frames[i].dx.Length];
                    for (int j = 0; j < frames[i].dx.Length; j++)
                    {
                        frames[i].dx[j] = reader.ReadShort();
                        frames[i].dy[j] = reader.ReadShort();
                        frames[i].idImg[j] = reader.ReadSByte();
                        if (i == 0)
                        {
                            if (left > frames[i].dx[j])
                                left = frames[i].dx[j];
                            if (top > frames[i].dy[j])
                                top = frames[i].dy[j];
                            if (right < frames[i].dx[j] + imgInfos[frames[i].idImg[j]].w)
                                right = frames[i].dx[j] + imgInfos[frames[i].idImg[j]].w;
                            if (bottom < frames[i].dy[j] + imgInfos[frames[i].idImg[j]].h)
                                bottom = frames[i].dy[j] + imgInfos[frames[i].idImg[j]].h;
                            effectData.width = right - left;
                            effectData.height = bottom - top;
                        }
                    }
                }
                effectData.frame = frames;
                short[] arrFrame = new short[reader.ReadShort()];
                for (int l = 0; l < arrFrame.Length; l++)
                    arrFrame[l] = reader.ReadShort();
                effectData.arrFrame = arrFrame;
            }
            catch { }
        }
    }
}
