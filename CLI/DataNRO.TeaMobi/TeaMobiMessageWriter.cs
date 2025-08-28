using System.IO;
using EHVN.DataNRO.Interfaces;

namespace EHVN.DataNRO.TeaMobi
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
            message.WriteStringUTF8(text);
            session.EnqueueMessage(message);
        }

        public void UpdateMap()
        {
            MessageSend message = MessageNotMap(6);
            session.EnqueueMessage(message);
        }

        public void UpdateItem()
        {
            MessageSend message = MessageNotMap(8);
            session.EnqueueMessage(message);
        }

        public void UpdateSkill()
        {
            MessageSend message = MessageNotMap(7);
            session.EnqueueMessage(message);
        }

        public void UpdateData()
        {
            MessageSend message = new MessageSend(-87);
            session.EnqueueMessage(message);
        }

        public void ClientOk()
        {
            MessageSend message = MessageNotMap(13);
            session.EnqueueMessage(message);
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
            message.WriteStringUTF8("Pc platform xxx|" + Config.gameVersion);
            Stream? stream = typeof(TeaMobiMessageWriter).Assembly.GetManifestResourceStream("EHVN.DataNRO.TeaMobi.Resources.info");
            if (stream is null)
                return;
            byte[] array = new byte[stream.Length];
            stream.Read(array, 0, array.Length);
            message.WriteShort((short)array.Length);
            message.WriteBytes(array);
            stream.Close();
            session.EnqueueMessage(message);
        }

        public void ImageSource()
        {
            MessageSend message = new MessageSend(-111);
            message.WriteShort(0);
            session.EnqueueMessage(message);
        }

        public void Login(string username, string pass, sbyte type)
        {
            MessageSend message = MessageNotLogin(0);
            message.WriteStringUTF8(username);
            message.WriteStringUTF8(pass);
            message.WriteStringUTF8(Config.gameVersion);
            message.WriteSByte(type);
            session.EnqueueMessage(message);
        }

        public void FinishUpdate()
        {
            MessageSend message = new MessageSend(-38);
            session.EnqueueMessage(message);
        }

        public void FinishLoadMap()
        {
            MessageSend message = new MessageSend(-39);
            session.EnqueueMessage(message);
        }

        public void CharMove(int x, int y)
        {
            MessageSend message = new MessageSend(-7);
            message.WriteByte(0);
            message.WriteShort((short)x);
            message.WriteShort((short)y);
            session.EnqueueMessage(message);
        }

        public void RequestChangeMap()
        {
            MessageSend message = new MessageSend(-23);
            session.EnqueueMessage(message);
        }
        
        public void GetMapOffline()
        {
            MessageSend message = new MessageSend(-33);
            session.EnqueueMessage(message);
        }

        public void RequestIcon(int id)
        {
            MessageSend message = new MessageSend(-67);
            message.WriteInt(id);
            session.EnqueueMessage(message);
        }

        public void GetResource(byte action)
        {
            MessageSend message = new MessageSend(-74);
            message.WriteByte(action);
            session.EnqueueMessage(message);
        }

        public void OpenUIZone()
        {
            MessageSend message = new MessageSend(29);
            session.EnqueueMessage(message);
        }

        public void RequestMobTemplate(short mobTemplateID)
        {
            MessageSend message = new MessageSend(11);
            message.WriteShort(mobTemplateID);
            session.EnqueueMessage(message);
        }

        public void RequestChangeZone(int zoneId)
        {
            MessageSend message = new MessageSend(21);
            message.WriteByte((byte)zoneId);
            session.EnqueueMessage(message);
        }

        public void RequestMapTemplate(int mapTemplateID)
        {
            MessageSend messageSend = MessageNotMap(10);
            messageSend.WriteByte((byte)mapTemplateID);
            session.EnqueueMessage(messageSend);
        }

        MessageSend MessageNotMap(sbyte command)
        {
            MessageSend message = new MessageSend(-28);
            message.WriteSByte(command);
            return message;
        }

        MessageSend MessageNotLogin(sbyte command)
        {
            MessageSend message = new MessageSend(-29);
            message.WriteSByte(command);
            return message;
        }
    }
}
