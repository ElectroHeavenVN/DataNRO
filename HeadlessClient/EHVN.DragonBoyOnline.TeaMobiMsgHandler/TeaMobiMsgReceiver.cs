using EHVN.DragonBoyOnline.CustomMsgHandler;
using System.Security.Cryptography;
using static EHVN.DragonBoyOnline.CustomMsgHandler.GameData;

namespace EHVN.DragonBoyOnline.TeaMobiMsgHandler
{
    public class TeaMobiMsgReceiver : MessageReceiverBase
    {
        static readonly string[] blankImageHashes =
        [
            "110B964285DEB1A8D3D13562914E1E2B51F4799A85412884B481E0316358DF48",
            "A09E276301DE73E84A35169799AFEBC9B0DEE6EC73DB827BF97D604E76395433",
            "95DB5A048C9A4A9AB381FBD97CD7B724112FB11CDEC8E7A083AC2A366D2E9CF0"
        ];

        TeaMobiGameSession session => (TeaMobiGameSession)gameSession;

        public override void OnMessageReceived(MessageRecv message)
        {
            switch (message.Command)
            {
                case 0xE4:
                    //MessageNotMap
                    switch (message.ReadUInt8())
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
                        case 10:
                            ReadMapTemplate(message);
                            break;
                    }
                    break;
                case 0xE8:
                    ReadCurrentMapInfo(message);
                    break;
                case 0xBD:
                    ReadIcon(message);
                    break;
                case 0xAE:
                    ReadTileTypeAndIndex(message);
                    break;
                case 0xA9:
                    ReadCommonData(message);
                    break;
                case 0xB6:
                    ReadResource(message);
                    break;
                case 0xB3:
                    session.Data.MaxSmall = message.ReadInt16();
                    break;
                case 0x0B:
                    ReadMobTemplate(message);
                    break;
                case 0x0C:    //read_cmdExtraBig
                    byte b = message.ReadUInt8();
                    if (b == 0)
                        ReadItemData(message);
                    break;
                //Messages
                case 0xE3:
                    if (message.ReadUInt8() != 2)
                        break;
                    OnIPAddressListReceived(message.ReadString());
                    break;
                case 0xE6:
                    OnDialogMessageReceived(message.ReadString());
                    break;
                case 0xE7:
                    OnServerMessageReceived(message.ReadString());
                    break;
                case 0x5E:
                    OnServerAlertReceived(message.ReadString());
                    break;
                case 0x5C:
                {
                    string name = message.ReadString();
                    string msg = message.ReadString();
                    if (string.IsNullOrEmpty(name))
                    {
                        OnGameNotificationReceived(msg);
                        break;
                    }
                    int charId = message.ReadInt32();
                    short head = message.ReadInt16();
                    short headIcon = message.ReadInt16();
                    short body = message.ReadInt16();
                    short bag = message.ReadInt16();
                    short leg = message.ReadInt16();
                    bool isChatServer = !message.ReadBool();
                    if (isChatServer)
                        OnServerChatReceived(name, msg);
                    else
                        OnPrivateChatReceived(name, msg);
                    break;
                }
                case 0x5D:
                    OnServerNotificationReceived(message.ReadString());
                    break;
                case 0x23:
                    OnUnknownMessageReceived(message.ReadString());
                    break;
            }
        }

        public override void OnConnectionFail()
        {
        }

        public override void OnDisconnected()
        {
        }

        public override void OnConnectOK()
        {
        }

        void ReadMapData(MessageRecv message)
        {
            message.ReadUInt8();
            int mapLength = message.ReadInt16();
            for (int i = 0; i < mapLength; i++)
            {
                Map map = new Map();
                map.id = i;
                map.name = message.ReadString();
                session.Data.Maps.Add(map);
            }
            session.Data.NpcTemplates = new NpcTemplate[message.ReadUInt8()];
            for (int i = 0; i < session.Data.NpcTemplates.Length; i++)
            {
                NpcTemplate npcTemplate = new NpcTemplate();
                npcTemplate.npcTemplateId = i;
                npcTemplate.name = message.ReadString();
                npcTemplate.headId = message.ReadInt16();
                npcTemplate.bodyId = message.ReadInt16();
                npcTemplate.legId = message.ReadInt16();
                npcTemplate.menu = new string[message.ReadUInt8()][];
                for (int j = 0; j < npcTemplate.menu.Length; j++)
                {
                    npcTemplate.menu[j] = new string[message.ReadUInt8()];
                    for (int k = 0; k < npcTemplate.menu[j].Length; k++)
                        npcTemplate.menu[j][k] = message.ReadString();
                }
                session.Data.NpcTemplates[i] = npcTemplate;
            }
            session.Data.MobTemplates = new MobTemplate[message.ReadInt16()];
            session.Data.MobTemplateEffectData = new EffectData[session.Data.MobTemplates.Length];
            for (sbyte i = 0; i < session.Data.MobTemplates.Length; i++)
            {
                MobTemplate mobTemplate = new MobTemplate();
                mobTemplate.mobTemplateId = i;
                mobTemplate.type = message.ReadInt8();
                mobTemplate.name = message.ReadString();
                mobTemplate.hp = message.ReadInt64();
                mobTemplate.rangeMove = message.ReadInt8();
                mobTemplate.speed = message.ReadInt8();
                mobTemplate.dartType = message.ReadInt8();
                session.Data.MobTemplates[i] = mobTemplate;

                session.Data.MobTemplateEffectData[i] = new EffectData();
            }
        }

        void ReadSkillData(MessageRecv message)
        {
            byte vcSkill = message.ReadUInt8();
            int skillOptionTemplateLength = message.ReadUInt8();
            for (int i = 0; i < skillOptionTemplateLength; i++)
            {
                string skillOptionTemplateName = message.ReadString();
                //do something with skillOptionTemplateName if needed
            }
            session.Data.NClasses = new NClass[message.ReadUInt8()];
            for (int i = 0; i < session.Data.NClasses.Length; i++)
            {
                NClass nClass = new NClass();
                nClass.classId = i;
                nClass.name = message.ReadString();
                nClass.skillTemplates = new SkillTemplate[message.ReadUInt8()];
                for (int j = 0; j < nClass.skillTemplates.Length; j++)
                {
                    SkillTemplate skillTemplate = new SkillTemplate();
                    skillTemplate.id = message.ReadInt8();
                    skillTemplate.name = message.ReadString();
                    skillTemplate.maxPoint = message.ReadInt8();
                    skillTemplate.manaUseType = message.ReadInt8();
                    skillTemplate.type = message.ReadInt8();
                    skillTemplate.icon = message.ReadInt16();
                    skillTemplate.damInfo = message.ReadString();
                    skillTemplate.description = message.ReadString();
                    skillTemplate.skills = new Skill[message.ReadUInt8()];
                    for (int k = 0; k < skillTemplate.skills.Length; k++)
                    {
                        Skill skill = new Skill();
                        skill.skillId = message.ReadInt16();
                        skill.point = message.ReadInt8();
                        skill.powRequire = message.ReadInt64();
                        skill.manaUse = message.ReadInt16();
                        skill.coolDown = message.ReadInt32();
                        skill.dx = message.ReadInt16();
                        skill.dy = message.ReadInt16();
                        skill.maxFight = message.ReadInt8();
                        skill.damage = message.ReadInt16();
                        skill.price = message.ReadInt16();
                        skill.moreInfo = message.ReadString();
                        skillTemplate.skills[k] = skill;
                    }
                    nClass.skillTemplates[j] = skillTemplate;
                }
                session.Data.NClasses[i] = nClass;
            }
        }

        void ReadItemData(MessageRecv message)
        {
            message.ReadUInt8();
            sbyte type = message.ReadInt8();
            if (type == 0)
            {
                session.Data.ItemOptionTemplates = new ItemOptionTemplate[message.ReadInt16()];
                for (int i = 0; i < session.Data.ItemOptionTemplates.Length; i++)
                {
                    ItemOptionTemplate itemOptionTemplate = new ItemOptionTemplate();
                    itemOptionTemplate.id = i;
                    itemOptionTemplate.name = message.ReadString();
                    itemOptionTemplate.type = message.ReadInt8();
                    session.Data.ItemOptionTemplates[i] = itemOptionTemplate;
                }
            }
            else if (type == 1)
            {
                short start = 0;
                short end = message.ReadInt16();
                for (int i = start; i < end; i++)
                {
                    ItemTemplate itemTemplate = new ItemTemplate();
                    itemTemplate.id = i;
                    itemTemplate.type = message.ReadInt8();
                    itemTemplate.gender = message.ReadInt8();
                    itemTemplate.name = message.ReadString();
                    itemTemplate.description = message.ReadString();
                    itemTemplate.level = message.ReadInt8();
                    itemTemplate.strRequire = message.ReadInt32();
                    itemTemplate.icon = message.ReadInt16();
                    itemTemplate.part = message.ReadInt16();
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

        void ReadMapTemplate(MessageRecv message)
        {
            Map? map = session.Data.MapToReceiveTemplate;
            if (map is null)
                return;
            map.mapTemplate = new MapTemplate();
            map.mapTemplate.width = message.ReadUInt8();
            map.mapTemplate.height = message.ReadUInt8();
            int count = map.mapTemplate.width * map.mapTemplate.height;
            map.mapTemplate.maps = new int[count];
            for (int i = 0; i < count; i++)
            {
                map.mapTemplate.maps[i] = message.ReadUInt8();
            }
            map.mapTemplate.types = new int[count];
            session.Data.MapToReceiveTemplate = null;
        }

        void ReadTileTypeAndIndex(MessageRecv message)
        {
            try
            {
                MapTemplate.tileIndex = new int[message.ReadUInt8()][][];
                MapTemplate.tileType = new int[MapTemplate.tileIndex.Length][];
                for (int i = 0; i < MapTemplate.tileIndex.Length; i++)
                {
                    byte length = message.ReadUInt8();
                    MapTemplate.tileType[i] = new int[length];
                    MapTemplate.tileIndex[i] = new int[length][];
                    for (int j = 0; j < length; j++)
                    {
                        MapTemplate.tileType[i][j] = message.ReadInt32();
                        MapTemplate.tileIndex[i][j] = new int[message.ReadUInt8()];
                        for (int k = 0; k < MapTemplate.tileIndex[i][j].Length; k++)
                            MapTemplate.tileIndex[i][j][k] = message.ReadUInt8();
                    }
                }
            }
            catch { }
        }

        void ReadCurrentMapInfo(MessageRecv message)
        {
            byte mapId = message.ReadUInt8();
            sbyte planetId = message.ReadInt8();
            sbyte tileID = message.ReadInt8();
            sbyte bgID = message.ReadInt8();
            sbyte typeMap = message.ReadInt8();
            string mapName = message.ReadString();
            sbyte zoneId = message.ReadInt8();
            //Who cares what the rest of the data is?
        }

        void ReadIcon(MessageRecv message)
        {
            if (!session.Data.SaveIcon)
                return;
            int iconId = message.ReadInt32();
            byte[] data = message.ReadBytes();
            if (data.Length < 500)
            {
                byte[] hash = SHA256.HashData(data);
                if (blankImageHashes.Contains(Convert.ToHexString(hash)))
                    return;
            }
            session.FileWriter.WriteIcon(iconId, data);
        }

        void ReadCommonData(MessageRecv message)
        {
            message.ReadUInt8();
            byte[] nr_dart = message.ReadBytes();
            byte[] nr_arrow = message.ReadBytes();
            byte[] nr_effect = message.ReadBytes();
            byte[] nr_image = message.ReadBytes();
            byte[] nr_part = message.ReadBytes();
            byte[] nr_skill = message.ReadBytes();

            BufferedMessageRecv partReader = new BufferedMessageRecv(0, nr_part);
                Part[] parts = new Part[partReader.ReadInt16()];
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = new Part(partReader.ReadInt8());
                for (int j = 0; j < parts[i].pi.Length; j++)
                {
                    PartImage partImage = new PartImage();
                    partImage.id = partReader.ReadInt16();
                    partImage.dx = partReader.ReadInt8();
                    partImage.dy = partReader.ReadInt8();
                    parts[i].pi[j] = partImage;
                }
            }
            session.Data.Parts = parts;
            BufferedMessageRecv imageReader = new BufferedMessageRecv(0, nr_image);
            session.Data.SmallImg = new int[imageReader.ReadInt16()][];
            for (int i = 0; i < session.Data.SmallImg.Length; i++)
                session.Data.SmallImg[i] = new int[5];
            for (int i = 0; i < session.Data.SmallImg.Length; i++)
            {
                session.Data.SmallImg[i][0] = imageReader.ReadUInt8();
                session.Data.SmallImg[i][1] = imageReader.ReadInt16();
                session.Data.SmallImg[i][2] = imageReader.ReadInt16();
                session.Data.SmallImg[i][3] = imageReader.ReadInt16();
                session.Data.SmallImg[i][4] = imageReader.ReadInt16();
            }
        }

        void ReadResource(MessageRecv message)
        {
            if (!session.Data.SaveIcon)
                return;
            byte b = message.ReadUInt8();
            switch (b)
            {
                case 0:
                    int resVersion = message.ReadInt32();
                    //Do something with resVersion if needed
                    break;
                case 1:
                    short nBig = message.ReadInt16();
                    //Do something with nBig if needed
                    session.Sender.GetResource(2);
                    break;
                case 2:
                    string fileName = message.ReadString();
                    if (fileName.Contains("Big"))
                    {
                        fileName = fileName.Substring(fileName.IndexOf("Big"));
                        byte[] data = message.ReadBytes();
                        session.FileWriter.WriteBigIcon(fileName, data);
                    }
                    else
                    {
                        fileName = fileName.Split('/').Last();
                        byte[] data = message.ReadBytes();
                        session.FileWriter.WriteResource(fileName, data);
                    }
                    break;
                case 3:
                    int newResVersion = message.ReadInt32();
                    // Do something with newResVersion if needed
                    session.Data.AllResourceLoaded = true;
                    break;
            }
        }

        void ReadMobTemplate(MessageRecv message)
        {
            int templateID = message.ReadInt16();
            byte type = message.ReadUInt8();
            byte[] data = message.ReadBytes();
            if (type == 0)
                ReadMobData(templateID, data);
            else
                ReadNewMobData(templateID, data, type);
            byte[] imgData = message.ReadBytes();
            session.FileWriter.WriteMobImg(templateID, imgData);
            byte typeData = message.ReadUInt8();
        }

        void ReadMobData(int templateID, byte[] data)
        {
            BufferedMessageRecv reader = new BufferedMessageRecv(0, data);
            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;
            EffectData effectData = session.Data.MobTemplateEffectData[templateID];
            try
            {
                ImageInfo[] imgInfos = new ImageInfo[reader.ReadUInt8()];
                for (int i = 0; i < imgInfos.Length; i++)
                {
                    ImageInfo imgInfo = new ImageInfo();
                    imgInfo.id = reader.ReadInt8();
                    imgInfo.x0 = reader.ReadUInt8();
                    imgInfo.y0 = reader.ReadUInt8();
                    imgInfo.w = reader.ReadUInt8();
                    imgInfo.h = reader.ReadUInt8();
                    imgInfos[i] = imgInfo;
                }
                effectData.imgInfo = imgInfos;
                Frame[] frame = new Frame[reader.ReadInt16()];
                for (int i = 0; i < frame.Length; i++)
                {
                    frame[i] = new Frame();
                    frame[i].dx = new short[reader.ReadUInt8()];
                    frame[i].dy = new short[frame[i].dx.Length];
                    frame[i].idImg = new sbyte[frame[i].dx.Length];
                    for (int j = 0; j < frame[i].dx.Length; j++)
                    {
                        frame[i].dx[j] = reader.ReadInt16();
                        frame[i].dy[j] = reader.ReadInt16();
                        frame[i].idImg[j] = reader.ReadInt8();
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
                short[] arrFrame = new short[reader.ReadInt16()];
                if (effectData.id >= 201)
                {
                    short index = 0;
                    short[] array = new short[arrFrame.Length];
                    int count = 0;
                    bool flag = false;
                    for (int i = 0; i < array.Length; i++)
                    {
                        arrFrame[i] = reader.ReadInt16();
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
                        if (effectData.anim_data[i] is null)
                            effectData.anim_data[i] = effectData.anim_data[2];
                    }
                }
                else
                {
                    for (int i = 0; i < arrFrame.Length; i++)
                        arrFrame[i] = reader.ReadInt16();
                }
                effectData.arrFrame = arrFrame;
            }
            catch { }
        }

        void ReadNewMobData(int templateID, byte[] data, byte typeRead)
        {
            BufferedMessageRecv reader = new BufferedMessageRecv(0, data);
            int left = 0;
            int top = 0;
            int right = 0;
            int bottom = 0;
            EffectData effectData = session.Data.MobTemplateEffectData[templateID];
            try
            {
                ImageInfo[] imgInfos = new ImageInfo[reader.ReadUInt8()];
                for (int i = 0; i < imgInfos.Length; i++)
                {
                    imgInfos[i] = new ImageInfo();
                    imgInfos[i].id = reader.ReadUInt8();
                    if (typeRead == 1)
                    {
                        imgInfos[i].x0 = reader.ReadUInt8();
                        imgInfos[i].y0 = reader.ReadUInt8();
                    }
                    else
                    {
                        imgInfos[i].x0 = reader.ReadInt16();
                        imgInfos[i].y0 = reader.ReadInt16();
                    }
                    imgInfos[i].w = reader.ReadUInt8();
                    imgInfos[i].h = reader.ReadUInt8();
                }
                effectData.imgInfo = imgInfos;
                Frame[] frames = new Frame[reader.ReadInt16()];
                for (int i = 0; i < frames.Length; i++)
                {
                    frames[i] = new Frame();
                    frames[i].dx = new short[reader.ReadUInt8()];
                    frames[i].dy = new short[frames[i].dx.Length];
                    frames[i].idImg = new sbyte[frames[i].dx.Length];
                    for (int j = 0; j < frames[i].dx.Length; j++)
                    {
                        frames[i].dx[j] = reader.ReadInt16();
                        frames[i].dy[j] = reader.ReadInt16();
                        frames[i].idImg[j] = reader.ReadInt8();
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
                short[] arrFrame = new short[reader.ReadInt16()];
                for (int l = 0; l < arrFrame.Length; l++)
                    arrFrame[l] = reader.ReadInt16();
                effectData.arrFrame = arrFrame;
            }
            catch { }
        }
    }
}
