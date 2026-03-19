using EHVN.DragonBoyOnline.CustomMsgHandler;

namespace EHVN.DragonBoyOnline.TeaMobiMsgHandler
{
    partial class TeaMobiMsgSender
    {
        [Obsolete("Unused function")]
        public void RequestClan(short id)
        {
            MessageSend message = new MessageSend(0xCB); // -53
            message.WriteInt16(id);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void GetTask(sbyte npcTemplateId, sbyte menuId, sbyte optionId)
        {
            MessageSend message = new MessageSend(0x28); // 40
            message.WriteInt8(npcTemplateId);
            message.WriteInt8(menuId);
            if (optionId >= 0)
                message.WriteInt8(optionId);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void SelectCharToPlay(string charname)
        {
            MessageSend message = new MessageSend(0xE4); // -28
            message.WriteUInt8(1);
            message.WriteString(charname);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void RequestSkill(short skillId)
        {
            MessageSend message = MessageNotMap(9);
            message.WriteInt16(skillId);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void RequestItemInfo(sbyte typeUI, sbyte indexUI)
        {
            MessageSend message = new MessageSend(0x23); // 35
            message.WriteInt8(typeUI);
            message.WriteInt8(indexUI);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void RequestItemPlayer(int playerID, sbyte indexUI)
        {
            MessageSend message = new MessageSend(0x5A); // 90
            message.WriteInt32(playerID);
            message.WriteInt8(indexUI);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void UpSkill(short skillTemplateId, sbyte point)
        {
            MessageSend message = MessageSubCommand(17);
            message.WriteInt16(skillTemplateId);
            message.WriteInt8(point);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void RequestItem(sbyte typeUI)
        {
            MessageSend message = MessageSubCommand(22);
            message.WriteInt8(typeUI);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void BoxSort()
        {
            MessageSend message = MessageSubCommand(19);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void BoxCoinOut(int coinOut)
        {
            MessageSend message = MessageSubCommand(21);
            message.WriteInt32(coinOut);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void UpgradeItem(sbyte indexUI, IEnumerable<sbyte> indexUIs, bool isGold)
        {
            MessageSend message = new MessageSend(0x0E); // 14
            message.WriteBool(isGold);
            message.WriteInt8(indexUI);
            foreach (sbyte idx in indexUIs)
                message.WriteInt8(idx);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void TradeAccept()
        {
            MessageSend message = new MessageSend(0x27); // 39
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void TradeItemLock(int coin, IEnumerable<sbyte> indexUIs)
        {
            MessageSend message = new MessageSend(0x26); // 38
            message.WriteInt32(coin);
            message.WriteInt8((sbyte)indexUIs.Count());
            foreach (sbyte idx in indexUIs)
                message.WriteInt8(idx);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void ThrowItem(sbyte index)
        {
            MessageSend message = new MessageSend(0xEE); // -18
            message.WriteInt8(index);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void GetQuayso()
        {
            MessageSend message = new MessageSend(0x82); // -126
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void TradeInvite(int playerID)
        {
            MessageSend message = new MessageSend(0x24); // 36
            message.WriteInt32(playerID);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void TestInvite(int playerID)
        {
            MessageSend message = new MessageSend(0x3B); // 59
            message.WriteInt32(playerID);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void AddCuuSat(int playerID)
        {
            MessageSend message = new MessageSend(0x3E); // 62
            message.WriteInt32(playerID);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void AddParty(string name)
        {
            MessageSend message = new MessageSend(0x4B); // 75
            message.WriteString(name);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void OutParty()
        {
            MessageSend message = new MessageSend(0x4F); // 79
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void RequestPlayerInfo(IEnumerable<int> playerIDs)
        {
            MessageSend message = new MessageSend(0x12); // 18
            message.WriteUInt8((byte)playerIDs.Count());
            foreach (int playerID in playerIDs)
                message.WriteInt32(playerID);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void PleaseInputParty(string str)
        {
            MessageSend message = new MessageSend(0x10); // 16
            message.WriteString(str);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void AcceptPleaseParty(string str)
        {
            MessageSend message = new MessageSend(0x11); // 17
            message.WriteString(str);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void ChatPrivate(string to, string text)
        {
            MessageSend message = new MessageSend(0x5B); // 91
            message.WriteString(to);
            message.WriteString(text);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void SaveRms(string key, IEnumerable<byte> data)
        {
            MessageSend message = MessageSubCommand(60);
            message.WriteString(key);
            message.WriteBytes(data.ToArray());
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void LoadRMS(string key)
        {
            MessageSend message = MessageSubCommand(61);
            message.WriteString(key);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void ClearTask()
        {
            MessageSend message = MessageNotMap(17);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void DoConvertUpgrade(sbyte index1, sbyte index2, sbyte index3)
        {
            MessageSend message = MessageNotMap(33);
            message.WriteInt8(index1);
            message.WriteInt8(index2);
            message.WriteInt8(index3);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void InviteClanDun(string name)
        {
            MessageSend message = MessageNotMap(34);
            message.WriteString(name);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void InputNumSplit(sbyte indexItem, int numSplit)
        {
            MessageSend message = MessageNotMap(40);
            message.WriteInt8(indexItem);
            message.WriteInt32(numSplit);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void UpdateAccProtect(uint oldPassword, uint newPassword)
        {
            MessageSend message = MessageNotMap(38);
            message.WriteUInt32(oldPassword);
            message.WriteUInt32(newPassword);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void GetChest(sbyte action)
        {
            MessageSend message = new MessageSend(0xDD); // -35
            message.WriteInt8(action);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void GetBag(sbyte action)
        {
            MessageSend message = new MessageSend(0xDC); // -36
            message.WriteInt8(action);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void GetBody(sbyte action)
        {
            MessageSend message = new MessageSend(0xDB); // -37
            message.WriteInt8(action);
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void SendOptHat(sbyte action, short myIdHat)
        {
            MessageSend message = new MessageSend(0x18); // 24
            if (action == 1)
            {
                message.WriteUInt8(1);
                message.WriteUInt16(0);
                message.WriteRawBytes([]);
            }
            else
            {
                if (myIdHat != -1)
                    message.WriteInt8(-1);
                else
                    message.WriteInt16(0);
            }
            EnqueueMessage(message);
        }

        [Obsolete("Unused function")]
        public void SendCmdExtra(sbyte sub, string user, string pass)
        {
            MessageSend message = new MessageSend(0x18); // 24
            message.WriteInt8(sub);
            message.WriteString(user);
            message.WriteString(pass);
            EnqueueMessage(message);
        }
    }
}
