using System.IO;
using DataNRO.Interfaces;

namespace DataNRO.TeaMobi
{
    public class TeaMobiMessageWriter : IMessageWriter
    {
        TeaMobiSession session;

        public TeaMobiMessageWriter(TeaMobiSession session)
        {
            this.session = session;
        }

        public void Chat(string text)
        {
            MessageSend message = new MessageSend(44);
            message.WriteStringUTF(text);
            session.SendMessage(message);
        }

        public void UpdateMap()
        {
            MessageSend message = MessageNotMap(6);
            session.SendMessage(message);
        }

        public void UpdateItem()
        {
            MessageSend message = MessageNotMap(8);
            session.SendMessage(message);
        }

        public void UpdateSkill()
        {
            MessageSend message = MessageNotMap(7);
            session.SendMessage(message);
        }

        public void UpdateData()
        {
            MessageSend message = new MessageSend((sbyte)Commands.UpdateData);
            session.SendMessage(message);
        }

        public void ClientOk()
        {
            MessageSend message = MessageNotMap(13);
            session.SendMessage(message);
        }

        public void SetClientType()
        {
            session.Data.ZoomLevel = Config.zoomLevel;
            MessageSend message = MessageNotLogin(2);
            message.WriteByte(Config.clientType);
            message.WriteByte(Config.zoomLevel);
            message.WriteBool(false);
            message.WriteInt(Config.screenWidth / Config.zoomLevel);
            message.WriteInt(Config.screenHeight / Config.zoomLevel);
            message.WriteBool(true);
            message.WriteBool(true);
            message.WriteStringUTF("Pc platform xxx|" + Config.gameVersion);
            Stream stream = typeof(TeaMobiMessageWriter).Assembly.GetManifestResourceStream("DataNRO.TeaMobi.Resources.info");
            byte[] array = new byte[stream.Length];
            stream.Read(array, 0, array.Length);
            message.WriteShort((short)array.Length);
            message.WriteBytes(array);
            stream.Close();
            session.SendMessage(message);
        }

        public void ImageSource()
        {
            MessageSend message = new MessageSend((sbyte)Commands.GetImageSource);
            message.WriteShort(0);
            session.SendMessage(message);
        }

        public void Login(string username, string pass, sbyte type)
        {
            MessageSend message = MessageNotLogin(0);
            message.WriteStringUTF(username);
            message.WriteStringUTF(pass);
            message.WriteStringUTF(Config.gameVersion);
            message.WriteSByte(type);
            session.SendMessage(message);
        }

        public void FinishUpdate()
        {
            MessageSend message = new MessageSend((sbyte)Commands.FinishUpdate);
            session.SendMessage(message);
        }

        public void FinishLoadMap()
        {
            MessageSend message = new MessageSend((sbyte)Commands.FinishLoadMap);
            session.SendMessage(message);
        }

        public void CharMove(int x, int y)
        {
            MessageSend message = new MessageSend((sbyte)Commands.PlayerMove);
            message.WriteByte(0);
            message.WriteShort((short)x);
            message.WriteShort((short)y);
            session.SendMessage(message);
        }

        public void RequestChangeMap()
        {
            MessageSend message = new MessageSend((sbyte)Commands.ChangeMap);
            session.SendMessage(message);
        }
        
        public void GetMapOffline()
        {
            MessageSend message = new MessageSend((sbyte)Commands.GetMapOffline);
            session.SendMessage(message);
        }

        public void RequestIcon(int id)
        {
            MessageSend message = new MessageSend((sbyte)Commands.RequestIcon);
            message.WriteInt(id);
            session.SendMessage(message);
        }

        public void GetResource(byte action)
        {
            MessageSend message = new MessageSend((sbyte)Commands.GetResource);
            message.WriteByte(action);
            session.SendMessage(message);
        }

        public void OpenUIZone()
        {
            MessageSend message = new MessageSend((sbyte)Commands.OpenUIZone);
            session.SendMessage(message);
        }

        public void RequestMobTemplate(short mobTemplateID)
        {
            MessageSend message = new MessageSend((sbyte)Commands.RequestMobTemplate);
            message.WriteShort(mobTemplateID);
            session.SendMessage(message);
        }

        public void RequestChangeZone(int zoneId)
        {
            MessageSend message = new MessageSend((sbyte)Commands.ChangeZone);
            message.WriteByte((byte)zoneId);
            session.SendMessage(message);
        }

        public void RequestMapTemplate(int mapTemplateID)
        {
            MessageSend messageSend = MessageNotMap(10);
            messageSend.WriteByte((byte)mapTemplateID);
            session.SendMessage(messageSend);
        }
        
        public void GetImgByName(string name)
        {
            MessageSend messageSend = new MessageSend((sbyte)Commands.GetImgByName);
            messageSend.WriteStringUTF(name);
            session.SendMessage(messageSend);
        }

        public void GetEffectData(short id)
        {
            MessageSend message = new MessageSend((sbyte)Commands.GetEffectData);
            message.WriteShort(id);
            session.SendMessage(message);
        }

        MessageSend MessageNotMap(sbyte command)
        {
            MessageSend message = new MessageSend((sbyte)Commands.NotMap);
            message.WriteSByte(command);
            return message;
        }

        MessageSend MessageNotLogin(sbyte command)
        {
            MessageSend message = new MessageSend((sbyte)Commands.NotLogin);
            message.WriteSByte(command);
            return message;
        }
    }
}
