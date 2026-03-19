using EHVN.DragonBoyOnline.CustomMsgHandler;

namespace EHVN.DragonBoyOnline.TeaMobiMsgHandler
{
    public partial class TeaMobiMsgSender : MessageSenderBase
    {
        const string TEAMOBI_PLATFORM = "Pc platform xxx";
        const string TEAMOBI_VERSION = "2.4.7";

        static readonly byte[] INFO =
        [
            0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00, 0xCF,
            0x90, 0x73, 0x00, 0x00, 0x0D, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0xBA, 0xA4, 0x74, 0x24,
            0x94, 0x37, 0x00, 0x30, 0x00, 0x00, 0x00, 0x20,
            0x00, 0x00, 0x00, 0x02, 0x9C, 0xE4, 0x20, 0x0F,
            0xEA, 0x59, 0x2B, 0x57, 0x1D, 0x33, 0x0A, 0x00,
            0x20, 0x00, 0x00, 0x00, 0x69, 0x6E, 0x66, 0x6F,
            0x5F, 0x34, 0x2E, 0x74, 0x78, 0x74, 0xA5, 0xEA,
            0x17, 0xF7, 0x23, 0x4E, 0x09, 0xCD, 0x00, 0xF0,
            0x4F, 0x4B, 0x45, 0x5B, 0x7F, 0xB7, 0xC8, 0xC2,
            0x15, 0x34, 0xE0, 0x6E, 0x28, 0xE1, 0x4E, 0x5B,
            0xFF, 0xB0, 0x8D, 0x5B, 0xD1, 0xFB, 0xE5, 0xDE,
            0xFE, 0xC9, 0xD3, 0xF6, 0xD3, 0x27, 0xAF, 0xE6,
            0x7C, 0xC2, 0xD4, 0xD5, 0xA7, 0x27, 0x6D, 0xB8,
            0x0A, 0x60, 0xC9, 0x1D, 0xFD, 0xF5, 0x41, 0xB1,
            0x80, 0x38, 0xBB, 0xC4, 0x3D, 0x7B, 0x00, 0x40,
            0x07, 0x00
        ];

        /// <summary>
        /// Teleport to player with the given ID.
        /// The current player must equip the Yardrat disguise.
        /// </summary>
        public void GotoPlayer(int id)
        {
            MessageSend message = new MessageSend(0x12); // 18
            message.WriteInt32(id);
            EnqueueMessage(message);
        }

        public void AndroidPack(string androidPack)
        {
            MessageSend message = new MessageSend(0x7E); // 126
            message.WriteString(androidPack);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Provide character registration information.
        /// Information correctness is not verified.
        /// </summary>
        public void CharInfo(string day, string month, string year, string address, string cmnd, string dayCmnd, string noiCapCmnd, string sdt, string name)
        {
            MessageSend message = new MessageSend(0x2A); // 42
            message.WriteString(day);
            message.WriteString(month);
            message.WriteString(year);
            message.WriteString(address);
            message.WriteString(cmnd);
            message.WriteString(dayCmnd);
            message.WriteString(noiCapCmnd);
            message.WriteString(sdt);
            message.WriteString(name);
            EnqueueMessage(message);
        }

        public void CheckAd(sbyte status)
        {
            MessageSend message = new MessageSend(0xD4); // -44
            message.WriteInt8(status);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Combine items.
        /// </summary>
        /// <param name="action">
        /// 1 = combine.
        /// </param>
        public void Combine(byte action, IEnumerable<sbyte> indexUIs)
        {
            MessageSend message = new MessageSend(0xAF); // -81
            message.WriteUInt8(action);
            if (action == 1)
            {
                message.WriteUInt8((byte)indexUIs.Count());
                foreach (sbyte indexUI in indexUIs)
                    message.WriteInt8(indexUI);
            }
            EnqueueMessage(message);
        }

        /// <summary>
        /// Trade gold and items.
        /// </summary>
        /// <param name="action">
        /// 0 = request trade
        /// 1 = accept trade request
        /// 2 = add item to trade
        /// 3 = cancel trade
        /// 4 = remove item from trade
        /// 5 = lock trade
        /// 7 = confirm trade
        /// </param>
        /// <remarks>
        /// To add an item to the trade, provide the index and quantity.
        /// To add gold to the trade, set index to -1 and provide the amount in quantity.
        /// For other actions, set the playerID, index, and quantity to -1.
        /// </remarks>
        public void Trade(byte action, int playerID, sbyte index, int quantity)
        {
            MessageSend message = new MessageSend(0xAA); // -86
            message.WriteUInt8(action);
            if (action == 0 || action == 1)
                message.WriteInt32(playerID);
            else if (action == 2)
            {
                message.WriteInt8(index);
                message.WriteInt32(quantity);
            }
            else if (action == 4)
                message.WriteInt8(index);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Send client input.
        /// </summary>
        public void SendClientInput(IEnumerable<string> inputs)
        {
            MessageSend message = new MessageSend(0x83); // -125
            message.WriteUInt8((byte)inputs.Count());
            foreach (string input in inputs)
                message.WriteString(input);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Use special skill.
        /// </summary>
        public void SpeacialSkill(byte index = 0)
        {
            MessageSend message = new MessageSend(0x70); // 112
            message.WriteUInt8(index);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Solve captcha returned by server, character by character.
        /// </summary>
        public void MobCapcha(char ch)
        {
            MessageSend message = new MessageSend(0xAB); // -85
                                                         // only the low byte is sent
            message.WriteInt8(0);
            message.WriteUInt8((byte)ch);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Friend actions.
        /// </summary>
        /// <param name="action">
        /// 0 = get friend list
        /// 1 = add friend
        /// 2 = remove friend
        /// </param>
        public void Friend(byte action, int playerId = -1)
        {
            MessageSend message = new MessageSend(0xB0); // -80
            message.WriteUInt8(action);
            if (playerId != -1)
                message.WriteInt32(playerId);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Get achievement.
        /// </summary>
        public void GetAchievement(sbyte index)
        {
            MessageSend message = new MessageSend(0xB4); // -76
            message.WriteInt8(index);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Get player menu for the given player ID.
        /// </summary>
        public void GetPlayerMenu(int playerID)
        {
            MessageSend message = new MessageSend(0xB1); // -79
            message.WriteInt32(playerID);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Get clan image.
        /// </summary>
        public void ClanImage(sbyte id)
        {
            MessageSend message = new MessageSend(0xC2); // -62
            message.WriteInt8(id);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Use currently selected skill without a target.
        /// </summary>
        public void SkillNotFocus(byte status)
        {
            MessageSend message = new MessageSend(0xD3); // -45
            message.WriteUInt8(status);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Donate a senzu bean to the requested member.
        /// </summary>
        /// <param name="id">
        /// Target message ID to donate
        /// </param>
        public void ClanDonate(int id)
        {
            MessageSend message = new MessageSend(0xCA); // -54
            message.WriteInt32(id);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Clan message actions.
        /// </summary>
        /// <param name="type">
        /// 0 = send chat message
        /// 1 = request 5 senzu beans
        /// 2 = join clan (???)
        /// </param>
        public void ClanMessage(byte type, string text, int clanID)
        {
            MessageSend message = new MessageSend(0xCD); // -51
            message.WriteUInt8(type);
            if (type == 0)
                message.WriteString(text);
            else if (type == 2)
                message.WriteInt32(clanID);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Use item actions.
        /// </summary>
        /// <param name="type">
        /// 0: Use item
        /// 1: Remove item from inventory
        /// </param>
        public void UseItem(sbyte type = -1, sbyte where = -1, sbyte index = -1, short templateId = -1)
        {
            MessageSend message = new MessageSend(0xD5); // -43
            message.WriteInt8(type);
            message.WriteInt8(where);
            message.WriteInt8(index);
            if (index != -1)
                message.WriteInt16(templateId);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Review clan join request.
        /// </summary>
        /// <param name="id">
        /// Clan message ID
        /// </param>
        /// <param name="action">
        /// 1 = approve
        /// 2 = reject
        /// </param>
        public void ReviewClanJoinRequest(int id, byte action)
        {
            MessageSend message = new MessageSend(0xCF); // -49
            message.WriteInt32(id);
            message.WriteUInt8(action);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Request clan members info for the given clan ID.
        /// </summary>
        public void ClanMember(int id)
        {
            MessageSend message = new MessageSend(0xCE); // -50
            message.WriteInt32(id);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Search for clans.
        /// </summary>
        public void SearchClan(string name)
        {
            MessageSend message = new MessageSend(0xD1); // -47
            message.WriteString(name);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Change role of a clan member.
        /// </summary>
        /// <param name="role">
        /// 0 = leader
        /// 1 = co-leader
        /// 2 = member
        /// -1 = remove from clan
        /// </param>
        public void ClanRemote(int id, sbyte role)
        {
            MessageSend message = new MessageSend(0xC8); // -56
            message.WriteInt32(id);
            message.WriteInt8(role);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Leave the current clan.
        /// </summary>
        public void LeaveClan()
        {
            MessageSend message = new MessageSend(0xC9); // -55
            EnqueueMessage(message);
        }

        /// <summary>
        /// Invite a player to a clan.
        /// </summary>
        /// <param name="action">
        /// 0 = invite (require playerID)
        /// 1 = accept invite (require clanID and invite code)
        /// 2 = reject invite (require clanID and invite code)
        /// </param>
        public void ClanInvite(byte action, int playerID = -1, int clanID = -1, int code = -1)
        {
            MessageSend message = new MessageSend(0xC7); // -57
            message.WriteUInt8(action);
            if (action == 0)
                message.WriteInt32(playerID);
            else if (action == 1 || action == 2)
            {
                message.WriteInt32(clanID);
                message.WriteInt32(code);
            }
            EnqueueMessage(message);
        }

        /// <summary>
        /// Clan actions.
        /// </summary>
        /// <param name="action">
        /// 1 = create clan
        /// 2 = commit clan creation (icon, name)
        /// 3 = change clan info
        /// 4 = commit clan changes (icon, slogan)
        /// </param>
        public void GetClan(byte action, short imgID = -1, string text = "")
        {
            MessageSend message = new MessageSend(0xD2); // -46
            message.WriteUInt8(action);
            if (action == 2 || action == 4)
            {
                message.WriteInt16(imgID);
                message.WriteString(text);
            }
            EnqueueMessage(message);
        }

        public void UpdateCaption(byte gender)
        {
            MessageSend message = new MessageSend(0xD7); // -41
            message.WriteUInt8(gender);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Equip/unequip/move item.
        /// </summary>
        /// <param name="type">
        /// 0 = move item from chest to inventory
        /// 1 = move item from inventory to chest
        /// 2 = equip item from chest
        /// 3 = unequip item to chest
        /// 4 = equip item from inventory
        /// 5 = unequip item to inventory
        /// 6 = equip item from inventory to disciple/pet
        /// 7 = unequip item from disciple/pet to inventory
        /// </param>
        /// <param name="index">Item index in the source (chest/equipped slots+inventory/pet).</param>
        public void GetItem(byte type, sbyte index)
        {
            MessageSend message = new MessageSend(0xD8); // -40
            message.WriteUInt8(type);
            message.WriteInt8(index);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Send information about the client device to the server. Must be called after initial connection and before login.
        /// </summary>
        public override void SetClientType(byte typeClient = 4, byte zoomLevel = 1, uint width = 720, uint height = 320, bool isQwerty = true, bool isTouch = true)
        {
            MessageSend message = MessageNotLogin(2);
            message.WriteUInt8(typeClient);
            message.WriteUInt8(zoomLevel);
            message.WriteUInt8(0);
            message.WriteUInt32(width);
            message.WriteUInt32(height);
            message.WriteBool(isQwerty);
            message.WriteBool(isTouch);
            message.WriteString(TEAMOBI_PLATFORM + "|" + TEAMOBI_VERSION);
            message.WriteUInt16((ushort)INFO.Length);
            message.WriteRawBytes(INFO, INFO.Length);
            EnqueueMessage(message);
        }

        public void SetClientType2(byte typeClient = 4, byte zoomLevel = 1, uint width = 720, uint height = 320, bool isQwerty = true, bool isTouch = true)
        {
            MessageSend message = MessageNotLogin(2);
            message.WriteUInt8(typeClient);
            message.WriteUInt8(zoomLevel);
            message.WriteUInt8(0);
            message.WriteUInt32(width);
            message.WriteUInt32(height);
            message.WriteBool(isQwerty);
            message.WriteBool(isTouch);
            message.WriteString(TEAMOBI_PLATFORM + "|" + TEAMOBI_VERSION);
            message.WriteUInt16((ushort)INFO.Length);
            message.WriteRawBytes(INFO, INFO.Length);
            EnqueueMessage(message);
        }

        // May be useful for ping check.
        public void SendCheckController()
        {
            MessageSend message = new MessageSend(0x88); // -120
            EnqueueMessage(message);
        }

        // May be useful for ping check.
        public void SendCheckMap()
        {
            MessageSend message = new MessageSend(0x87); // -121
            EnqueueMessage(message);
        }

        /// <summary>
        /// Login to the server.
        /// </summary>
        public override void Login(string username, string password, bool registered)
        {
            MessageSend message = MessageNotLogin(0);
            message.WriteString(username);
            message.WriteString(password);
            message.WriteString(TEAMOBI_VERSION);
            message.WriteBool(!registered);
            EnqueueMessage(message);
        }

        /// <summary> 
        /// Register account (bind the unregistered account with a <paramref name="username"/> and <paramref name="password"/>).
        /// </summary>
        public void RequestRegister(string username, string password, string userAo = "", string passAo = "a")
        {
            MessageSend message = MessageNotLogin(1);
            message.WriteString(username);
            message.WriteString(password);
            if (!string.IsNullOrEmpty(userAo))
            {
                message.WriteString(userAo);
                message.WriteString(passAo);
            }
            EnqueueMessage(message);
        }

        /// <summary> 
        /// Request to change map if the player is close to a waypoint.
        /// </summary>
        public void RequestChangeMap()
        {
            MessageSend message = new MessageSend(0xE9); // -23
            EnqueueMessage(message);
        }

        /// <summary>
        /// Use the senzu bean tree.
        /// </summary>
        /// <param name="type">
        /// 1 = open menu
        /// 2 = load the senzu bean tree (called when loading the home map)
        /// </param>
        public void MagicTree(byte type)
        {
            MessageSend message = new MessageSend(0xDE); // -34
            message.WriteUInt8(type);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Request to change zone.
        /// </summary>
        public override void RequestChangeZone(byte zoneId)
        {
            MessageSend message = new MessageSend(0x15); // 21
            message.WriteUInt8(zoneId);
            EnqueueMessage(message);
        }

        public void CheckMMove(int second)
        {
            MessageSend message = new MessageSend(0xB2); // -78
            message.WriteInt32(second);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Move player to the given coordinates.
        /// </summary>
        /// <param name="type">
        /// 0 = walk
        /// 1 = fly
        /// </param>
        public void Move(short x, short y, byte type)
        {
            MessageSend message = new MessageSend(0xF9); // -7
            message.WriteUInt8(type);
            message.WriteInt16(x);
            message.WriteInt16(y);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Move player to the given X coordinate. Y coordinate is the current Y position.
        /// </summary>
        /// <param name="type">
        /// 0 = walk
        /// 1 = fly
        /// </param>
        public void Move(short x, byte type)
        {
            MessageSend message = new MessageSend(0xF9); // -7
            message.WriteUInt8(type);
            message.WriteInt16(x);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Create a new character with the given <paramref name="name"/>, <paramref name="gender">, and <paramref name="hair"/> style.
        /// </summary>
        /// <param name="gender">
        /// 0 = Earth
        /// 1 = Namekian
        /// 2 = Saiyan
        /// </param>
        /// <param name="hair">
        /// Limited to 3 options per gender.
        /// </param>
        public void CreateChar(string name, byte gender, byte hair)
        {
            MessageSend message = new MessageSend(0xE4); // -28
            message.WriteUInt8(2);
            message.WriteString(name);
            message.WriteUInt8(gender);
            message.WriteUInt8(hair);
            EnqueueMessage(message);
        }

        public override void RequestMobTemplate(short mobTemplateId)
        {
            MessageSend message = new MessageSend(0x0B); // 11
            message.WriteInt16(mobTemplateId);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Sale item.
        /// </summary>
        /// <param name="action">
        /// 0 = sell (type: 0 = equipped item, 1 = inventory item, id: item index)
        /// 1 = confirm sale (type and id returned by server)
        /// </param>
        public void SaleItem(byte action, sbyte type, short id)
        {
            MessageSend message = new MessageSend(0x07); // 7
            message.WriteUInt8(action);
            message.WriteInt8(type);
            message.WriteInt16(id);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Buy item from shop, or process lucky spin items.
        /// </summary>
        /// <param name="type">
        /// 0 = buy item with templateID id using gold
        /// 1 = buy item with templateID id using gem
        /// * For lucky spin:
        /// 0 = receive item at index id
        /// 1 = delete item at index id
        /// 2 = receive all items
        /// </param>
        public void BuyItem(byte type, short id, ushort quantity)
        {
            MessageSend message = new MessageSend(0x06); // 6
            message.WriteUInt8(type);
            message.WriteInt16(id);
            if (quantity > 1)
                message.WriteUInt16(quantity);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Select skill with the given template ID.
        /// </summary>
        public void SelectSkill(ushort skillTemplateID)
        {
            MessageSend message = new MessageSend(0x22); // 34
            message.WriteUInt16(skillTemplateID);
            EnqueueMessage(message);
        }

        public void GetEffData(short id)
        {
            MessageSend message = new MessageSend(0xBE); // -66
            message.WriteInt16(id);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Request zones information of the current map.
        /// </summary>
        public void OpenUIZone()
        {
            MessageSend message = new MessageSend(0x1D); // 29
            EnqueueMessage(message);
        }

        /// <summary>
        /// Confirm menu selection with the given NPC ID and selection index.
        /// NPC menu must be currently open.
        /// </summary>
        public void ConfirmMenu(ushort npcID, byte select)
        {
            MessageSend message = new MessageSend(0x20); // 32
            message.WriteUInt16(npcID);
            message.WriteUInt8(select);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Open menu of the given NPC ID.
        /// </summary>
        public void OpenMenu(ushort npcId)
        {
            MessageSend message = new MessageSend(0x21); // 33
            message.WriteUInt16(npcId);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Open menu of the given NPC ID and select the given menu and option ID.
        /// </summary>
        public void Menu(byte npcId, byte menuId, byte optionID)
        {
            MessageSend message = new MessageSend(0x16); // 22
            message.WriteUInt8(npcId);
            message.WriteUInt8(menuId);
            message.WriteUInt8(optionID);
            EnqueueMessage(message);
        }

        public void MenuID(short menuID)
        {
            MessageSend message = new MessageSend(0x1B); // 27
            message.WriteInt16(menuID);
            EnqueueMessage(message);
        }

        public void TextBoxID(short menuID, string str)
        {
            MessageSend message = new MessageSend(0x58); // 88
            message.WriteInt16(menuID);
            message.WriteString(str);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Maybe unused
        /// </summary>
        public void CrystalCollectLock(IEnumerable<sbyte> indexUIs)
        {
            MessageSend message = new MessageSend(0x0D); // 13
            foreach (sbyte idx in indexUIs)
                message.WriteInt8(idx);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Maybe unused
        /// </summary>
        public void AcceptInviteTrade(int playerMapId)
        {
            MessageSend message = new MessageSend(0x25); // 37
            message.WriteInt32(playerMapId);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Maybe unused
        /// </summary>
        public void CancelInviteTrade()
        {
            MessageSend message = new MessageSend(0x32); // 50
            EnqueueMessage(message);
        }

        /// <summary>
        /// Attack mobs with the given IDs.
        /// </summary>
        /// <param name="cdir">the current direction of the player (-1 = left, 1 = right).</param>
        public void SendMobAttack(IEnumerable<sbyte> mobIDs, sbyte cdir = 1)
        {
            MessageSend message = new MessageSend(0x36); // 54
            foreach (sbyte mobID in mobIDs)
                message.WriteInt8(mobID);
            message.WriteInt8(cdir);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Attack mobs created by the "Summon" skill.
        /// </summary>
        /// <param name="cdir">the current direction of the player (-1 = left, 1 = right).</param>
        public void SendMobMeAttack(IEnumerable<int> mobIDs, sbyte cdir = 1)
        {
            MessageSend message = new MessageSend(0x36); // 54
            foreach (int mobID in mobIDs)
            {
                message.WriteInt8(-1);
                message.WriteInt32(mobID);
            }
            message.WriteInt8(cdir);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Attack both kind of mobs (normal and created by the "Summon" skill).
        /// </summary>
        /// <param name="cdir">the current direction of the player (-1 = left, 1 = right).</param>
        public void SendMobsAttack(IEnumerable<sbyte> mobIDs, IEnumerable<int> mobMeIDs, sbyte cdir = 1)
        {
            MessageSend message = new MessageSend(0x36); // 54
            foreach (sbyte mobID in mobIDs)
                message.WriteInt8(mobID);
            foreach (int mobID in mobMeIDs)
            {
                message.WriteInt8(-1);
                message.WriteInt32(mobID);
            }
            message.WriteInt8(cdir);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Attack players with the given IDs.
        /// </summary>
        /// <param name="cdir">the current direction of the player (-1 = left, 1 = right).</param>
        public void SendPlayerAttack(IEnumerable<int> playerIDs, sbyte cdir = 1)
        {
            MessageSend message = new MessageSend(0xC4); // -60
            foreach (int playerID in playerIDs)
                message.WriteInt32(playerID);
            message.WriteInt8(cdir);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Attack both players and mobs.
        /// </summary>
        /// <param name="type">main target type (1 = mob, 2 = other player).</param>
        /// <param name="cdir">the current direction of the player (-1 = left, 1 = right).</param>
        public void SendPlayerAndMobAttack(IEnumerable<sbyte> mobIDs, IEnumerable<int> playerIDs, byte type, sbyte cdir = 1)
        {
            MessageSend message;
            if (type == 1)
                message = new MessageSend(0xFC); // -4
            else if (type == 2)
                message = new MessageSend(0x43); // 67
            else
                return; // invalid type
            message.WriteUInt8((byte)mobIDs.Count());
            foreach (sbyte mobID in mobIDs)
                message.WriteInt8(mobID);
            foreach (int playerID in playerIDs)
                message.WriteInt32(playerID);
            message.WriteInt8(cdir);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Pick up item from the ground with the given item ID in the map.
        /// </summary>
        public void PickItem(short itemMapId)
        {
            MessageSend message = new MessageSend(0xEC); // -20
            message.WriteInt16(itemMapId);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Return to home from dead state.
        /// </summary>
        public void ReturnTownFromDead()
        {
            MessageSend message = new MessageSend(0xF1); // -15
            EnqueueMessage(message);
        }

        /// <summary>
        /// Revive from dead state. Cost: 1 gem.
        /// </summary>
        public void WakeUpFromDead()
        {
            MessageSend message = new MessageSend(0xF0); // -16
            EnqueueMessage(message);
        }

        /// <summary>
        /// Send chat message to all players in the current map.
        /// </summary>
        public void Chat(string text)
        {
            MessageSend message = new MessageSend(0x2C); // 44
            message.WriteString(text);
            EnqueueMessage(message);
        }

        public override void UpdateData()
        {
            MessageSend message = new MessageSend(0xA9); // -87
            EnqueueMessage(message);
        }

        public override void UpdateMap()
        {
            MessageSend message = MessageNotMap(6);
            EnqueueMessage(message);
        }

        public override void UpdateSkill()
        {
            MessageSend message = MessageNotMap(7);
            EnqueueMessage(message);
        }

        public override void UpdateItem()
        {
            MessageSend message = MessageNotMap(8);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Acknowledge that the game client is ready.
        /// </summary>
        public override void ClientOk()
        {
            MessageSend message = MessageNotMap(13);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Maybe unused
        /// </summary>
        public void AddFriend(string name)
        {
            MessageSend message = new MessageSend(0x35); // 53
            message.WriteString(name);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Maybe unused
        /// </summary>
        public void AddPartyAccept(int playerID)
        {
            MessageSend message = new MessageSend(0x4C); // 76
            message.WriteInt32(playerID);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Maybe unused
        /// </summary>
        public void AddPartyCancel(int playerID)
        {
            MessageSend message = new MessageSend(0x4D); // 77
            message.WriteInt32(playerID);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Send or accept PvP request.
        /// </summary>
        /// <param name="action">
        /// 0 = send PvP request
        /// 1 = accept PvP request
        /// 2 = ???
        /// </param>
        /// <param name="type">
        /// 3 = challenge
        /// 4 = practice
        /// </param>
        public void PvP(byte action, byte type, int playerId)
        {
            MessageSend message = new MessageSend(0xC5); // -59
            message.WriteUInt8(action);
            message.WriteUInt8(type);
            message.WriteInt32(playerId);
            EnqueueMessage(message);
        }

        public override void RequestMapTemplate(int mapTemplateId)
        {
            MessageSend message = MessageNotMap(10);
            message.WriteUInt8((byte)mapTemplateId);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Send private chat message to the given player ID.
        /// </summary>
        public void ChatPlayer(string text, int id)
        {
            MessageSend message = new MessageSend(0xB8); // -72
            message.WriteInt32(id);
            message.WriteString(text);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Send global chat message to all players in the server.
        /// Cost: 5 gems.
        /// </summary>
        public void ChatGlobal(string text)
        {
            MessageSend message = new MessageSend(0xB9); // -71
            message.WriteString(text);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Top-up using card serial and pin.
        /// </summary>
        public void SendCardInfo(string serial, string pin)
        {
            MessageSend message = MessageNotMap(16);
            message.WriteString(serial);
            message.WriteString(pin);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Change character name.
        /// </summary>
        public void ChangeName(string name, int id)
        {
            MessageSend message = MessageNotMap(18);
            message.WriteInt32(id);
            message.WriteString(name);
            EnqueueMessage(message);
        }

        public override void RequestIcon(int id)
        {
            MessageSend message = new MessageSend(0xBD); // -67
            message.WriteInt32(id);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Maybe unused
        /// </summary>
        public void ActiveAccProtect(uint password)
        {
            MessageSend message = MessageNotMap(37);
            message.WriteUInt32(password);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Clear account protection with the given password.
        /// </summary>
        public void ClearAccProtect(uint password)
        {
            MessageSend message = MessageNotMap(41);
            message.WriteUInt32(password);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Maybe unused
        /// </summary>
        public void OpenLockAccProtect(uint password)
        {
            MessageSend message = MessageNotMap(39);
            message.WriteUInt32(password);
            EnqueueMessage(message);
        }

        public void GetBgTemplate(short id)
        {
            MessageSend message = new MessageSend(0xE0); // -32
            message.WriteInt16(id);
            EnqueueMessage(message);
        }

        public void GetMapOffline()
        {
            MessageSend message = new MessageSend(0xDF); // -33
            EnqueueMessage(message);
        }

        /// <summary>
        /// Acknowledge that the update has been completed.
        /// </summary>
        public override void FinishUpdate()
        {
            MessageSend message = new MessageSend(0xDA); // -38
            EnqueueMessage(message);
        }

        public void FinishUpdate(int playerID)
        {
            MessageSend message = new MessageSend(0xDA); // -38
            message.WriteInt32(playerID);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Acknowledge that the map has been loaded.
        /// </summary>
        public override void FinishLoadMap()
        {
            MessageSend message = new MessageSend(0xD9); // -39
            EnqueueMessage(message);
        }

        public void RequestBagImage(short ID)
        {
            MessageSend message = new MessageSend(0xC1); // -63
            message.WriteInt16(ID);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Create a new game character.
        /// </summary>
        public void NewGame()
        {
            MessageSend message = new MessageSend(0x9B); // -101
            message.WriteUInt16(0);
            message.WriteUInt8(1);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Update the number of senzu beans on the tree.
        /// </summary>
        public void GetMagicTree(byte action = 2)
        {
            MessageSend message = new MessageSend(0xDE); // -34
            message.WriteUInt8(action);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Level up the current player's base points.
        /// </summary>
        /// <param name="type">
        /// 0 = Base HP
        /// 1 = Base MP
        /// 2 = Base attack
        /// 3 = Base defense
        /// 4 = Base critical
        /// </param>
        /// <param name="points">
        /// The number of points to increase, must be 1, 10 or 100.
        /// </param>
        public void UpPotential(byte type, ushort points)
        {
            MessageSend message = MessageSubCommand(16);
            message.WriteUInt8(type);
            message.WriteUInt16(points);
            EnqueueMessage(message);
        }


        /// <summary>
        /// Request resource update from the server.
        /// </summary>
        /// <param name="action">
        /// 1 = request resource
        /// 2 = acknowledge resource received
        /// 3 = finish resource update
        /// </param>
        public override void GetResource(byte action)
        {
            MessageSend message = new MessageSend(0xB6); // -74
            message.WriteUInt8(action);
            EnqueueMessage(message);
        }
        /// <summary>
        /// Go to the selected map using the spaceship. The current player must have the "10 Capsules pack" or "Special Capsule" item in the inventory in order to use the spaceship to quickly travel to other maps.
        /// </summary>
        /// <param name="selected">
        /// Index of the selected map in the spaceship menu.
        /// </param>
        public void RequestMapSelect(byte selected)
        {
            MessageSend message = new MessageSend(0xA5); // -91
            message.WriteUInt8(selected);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Request pet information.
        /// </summary>
        public void PetInfo()
        {
            MessageSend message = new MessageSend(0x95); // -107
            EnqueueMessage(message);
        }

        /// <summary>
        /// Send challenge request to the selected player in the <paramref name="topName"/> leaderboard.
        /// </summary>
        /// <param name="selected">
        /// Index of the selected player in the leaderboard.
        /// </param>
        public void SendTop(string topName, byte selected)
        {
            MessageSend message = new MessageSend(0xA0); // -96
            message.WriteString(topName);
            message.WriteUInt8(selected);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Manage enemy list.
        /// </summary>
        /// <param name="action">
        /// 0 = Request enemy list information.
        /// 1 = teleport to the enemy's location and take revenge. Cost: 1 gem.
        /// 2 = remove enemy from the list.
        /// </param>
        public void Enemy(byte action, int playerID = -1)
        {
            MessageSend message = new MessageSend(0x9D); // -99
            message.WriteUInt8(action);
            message.WriteInt32(playerID);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Manage consigned items.
        /// </summary>
        /// <param name="action">
        /// 0 = consign item (requires itemID, moneyType, cost, quantity)
        /// 1 = cancel item consignment (requires itemID)
        /// 2 = receive money from sold item (requires itemID)
        /// 3 = buy consigned item (requires itemID, moneyType, cost)
        /// 4 = load consigned items list (requires moneyType, cost becomes page number)
        /// 5 = bump the consigned item to the top of the list (requires itemID)
        /// </param>
        /// <param name="moneyType">
        /// 0 = gold
        /// 1 = gem
        /// </param>
        public void Consign(byte action, short itemID = -1, sbyte moneyType = -1, int cost = -1, int quantity = -1)
        {
            MessageSend message = new MessageSend(0x9C); // -100
            message.WriteUInt8(action);
            if (action == 0)
            {
                message.WriteInt16(itemID);
                message.WriteInt8(moneyType);
                message.WriteInt32(cost);
                message.WriteInt32(quantity);
            }
            else if (action == 1 || action == 2)
                message.WriteInt16(itemID);
            else if (action == 3)
            {
                message.WriteInt16(itemID);
                message.WriteInt8(moneyType);
                message.WriteInt32(cost);
            }
            else if (action == 4)
            {
                message.WriteInt8(moneyType);
                message.WriteInt8((sbyte)cost);
            }
            if (action == 5)
                message.WriteInt16(itemID);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Get/Set flag for the current player.
        /// </summary>
        /// <param name="action">
        /// 0 = request flag information
        /// 1 = set flag
        /// 2 = request flag image
        /// </param>
        /// <param name="flagType">
        /// 0 = remove flag
        /// 1 = cyan flag
        /// 2 = red flag
        /// 3 = violet flag
        /// 4 = yellow flag
        /// 5 = green flag
        /// 6 = pink flag
        /// 7 = orange flag
        /// 8 = grey flag
        /// 9 = Kaio flag
        /// 10 = Mabu flag
        /// 11 = blue flag
        /// 12 = white flag
        /// 13 = black flag
        /// </param>
        public void GetFlag(byte action, sbyte flagType = -1)
        {
            MessageSend message = new MessageSend(0x99); // -103
            message.WriteUInt8(action);
            if (action != 0)
                message.WriteInt8(flagType);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Lock the inventory with the given <paramref name="password"/>.
        /// </summary>
        /// <param name="password">
        /// 6-digit number used to lock the inventory.
        /// </param>
        public void SetLockInventory(uint password)
        {
            MessageSend message = new MessageSend(0x98); // -104
            message.WriteUInt32(password);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Update disciple/pet status.
        /// </summary>
        /// <param name="status">
        /// 0 = follow
        /// 1 = protect
        /// 2 = attack
        /// 3 = go home
        /// 4 = fusion
        /// 5 = permanently fuse (only for Namekian)
        /// </param>
        public void PetStatus(byte status)
        {
            MessageSend message = new MessageSend(0x94); // -108
            message.WriteUInt8(status);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Accelerate spaceship to Future. Cost: 1 gem per use.
        /// </summary>
        public void TransportNow()
        {
            MessageSend message = new MessageSend(0x97); // -105
            EnqueueMessage(message);
        }

        public void Funsion(byte type)
        {
            // type: 4, 5
            // check the Char::setFusion method.
            MessageSend message = new MessageSend(0x7D); // 125
            message.WriteUInt8(type);
            EnqueueMessage(message);
        }

        public override void ImageSource()
        {
            MessageSend message = new MessageSend(0x91); // -111
            message.WriteUInt16(0);
            EnqueueMessage(message);
        }

        public void SendServerData(byte action, int id, IEnumerable<byte>? data = null)
        {
            MessageSend message = new MessageSend(0x92); // -110
            message.WriteUInt8(action);
            if (action == 1)
            {
                message.WriteInt32(id);
                if (data != null)
                {
                    message.WriteInt16((short)data.Count());
                    message.WriteRawBytes(data.ToArray());
                }
            }
            EnqueueMessage(message);
        }

        /// <summary>
        /// Change skill shortcut displayed on the game screen.
        /// </summary>
        public void ChangeOnKeyScr(IEnumerable<sbyte> skillTemplateIDs)
        {
            MessageSend message = new MessageSend(0x8F); // -113
            foreach (sbyte sk in skillTemplateIDs)
                message.WriteInt8(sk);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Harvest senzu beans from the tree.
        /// Only applicable when the current player is using the "Auto play" item.
        /// </summary>
        public void RequestBean()
        {
            MessageSend message = new MessageSend(0x8E); // -114
            EnqueueMessage(message);
        }

        /// <summary>
        /// Send challenge request to the player in the top players leaderboard.
        /// </summary>
        public void SendChallenge(int topPid)
        {
            MessageSend message = new MessageSend(0x8A); // -118
            message.WriteInt32(topPid);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Open player menu for the given <paramref name="playerID"/>.
        /// </summary>
        /// <param name="playerID"></param>
        public void MessagePlayerMenu(int playerID)
        {
            MessageSend message = new MessageSend(0xE2); // -30
            message.WriteInt8(63);
            message.WriteInt32(playerID);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Perform action in the player menu for the given player ID.
        /// </summary>
        /// <param name="select">
        /// Index of the selected action in the player menu.
        /// </param>
        public void PlayerMenuAction(int playerID, ushort select)
        {
            MessageSend message = new MessageSend(0xE2); // -30
            message.WriteInt8(64);
            message.WriteInt32(playerID);
            message.WriteUInt16(select);
            EnqueueMessage(message);
        }

        public void GetImgByName(string nameImg)
        {
            MessageSend message = new MessageSend(0x42); // 66
            message.WriteString(nameImg);
            EnqueueMessage(message);
        }

        public void SendCrackBall(byte type, byte quantity)
        {
            MessageSend message = new MessageSend(0x81); // -127
            message.WriteUInt8(type);
            message.WriteUInt8(quantity);
            EnqueueMessage(message);
        }

        public void SendRada(sbyte i, short id)
        {
            MessageSend message = new MessageSend(0x7F); // 127
            message.WriteInt8(i);
            message.WriteInt16(id);
            EnqueueMessage(message);
        }

        /// <summary>
        /// Request account deletion.
        /// </summary>
        public void SendDelAcc()
        {
            MessageSend message = new MessageSend(0x45); // 69
            EnqueueMessage(message);
        }

        /// <summary>
        /// Use currently selected skill without a target. 
        /// </summary>
        /// <remarks>
        /// The current skill must be a "9th skill" (the new special skills: Mafuba, Renzoku Energy Dan, and Super Kamekameha).
        /// </remarks>
        /// <param name="cdir">
        /// The current direction of the player (-1 = left, 1 = right).
        /// </param>
        public void NewSkillNotFocus(byte idTemplateSkill, sbyte cdir, short targetX, short targetY, short cx, short cy)
        {
            MessageSend message = new MessageSend(0xD3); // -45
            message.WriteInt8(20);
            message.WriteUInt8(idTemplateSkill);
            message.WriteInt16(cx);
            message.WriteInt16(cy);
            message.WriteInt8(cdir);
            message.WriteInt16(targetX);
            message.WriteInt16(targetY);
            EnqueueMessage(message);
        }

        public void SendCTReady(sbyte sub, sbyte subSub)
        {
            MessageSend message = new MessageSend(0x18); // 24
            message.WriteInt8(sub);
            message.WriteInt8(subSub);
            EnqueueMessage(message);
        }

        private MessageSend MessageNotMap(byte command)
        {
            MessageSend message = new MessageSend(0xE4); // -28
            message.WriteUInt8(command);
            return message;
        }

        private MessageSend MessageNotLogin(byte command)
        {
            MessageSend message = new MessageSend(0xE3); // -29
            message.WriteUInt8(command);
            return message;
        }

        private MessageSend MessageSubCommand(byte command)
        {
            MessageSend message = new MessageSend(0xE2); // -30
            message.WriteUInt8(command);
            return message;
        }
    }
}