using System.IO.Pipelines;
using System.Net.Sockets;

namespace EHVN.DragonBoyOnline.CustomMsgHandler;

public interface IGameSession : IDisposable
{
    bool IsConnected { get; }
    string Host { get; }
    ushort Port { get; }
    GameData Data { get; }
    FileWriter FileWriter { get; }
    Task<bool> ConnectAsync(string host, ushort port, CancellationToken cancellationToken = default);
    void Disconnect();
    Task UpdateTask(CancellationToken cancellationToken);
    bool EnqueueMessage(MessageSend message);
}

public enum EncryptionType
{
    NONE,
    ENC_SEND,
    ENC_RECV
}

public class GameSession<TSender, TReceiver> : IGameSession
    where TSender : MessageSenderBase, new()
    where TReceiver : MessageReceiverBase, new()
{
    protected static int[] BIG_MSG_CMDs = [0xE0, 0xBE, 0x0B, 0xBD, 0xB6, 0xA9, 0x42, 0x0C];

    protected TcpClient client = new TcpClient();
    protected string host = "";
    protected ushort port;
    protected byte[] encKey = [];
    protected byte curR;
    protected byte curW;
    protected Queue<MessageSend> pendingMessages = [];
    protected bool isConnected;
    protected TSender messageSender;
    protected TReceiver messageReceiver;
    protected CancellationTokenSource updateTaskCancellationTokenSource = new CancellationTokenSource();

    public GameData Data { get; } = new GameData();
    public FileWriter FileWriter { get; }

    public bool IsConnected => isConnected;
    public string Host => host;
    public ushort Port => port;
    public TSender Sender => messageSender;
    public TReceiver Receiver => messageReceiver;

    public GameSession(TSender sender, TReceiver receiver)
    {
        FileWriter = new FileWriter(this);
        messageSender = sender;
        messageReceiver = receiver;
        sender.SetSession(this);
        receiver.SetSession(this);
    }

    public virtual async Task<bool> ConnectAsync(string host, ushort port, CancellationToken cancellationToken = default)
    {
        if (isConnected && this.host == host && this.port == port)
            return true;
        try
        {
            Disconnect();
            this.host = host;
            this.port = port;
            client = new TcpClient();
            await client.ConnectAsync(host, port, cancellationToken);
            encKey = [];
            curR = 0;
            curW = 0;
            ClearMessageQueue();
            isConnected = true;
            MessageSend initMsg = new MessageSend(0xE5); //-27
            await _sendMessageAsync(initMsg, cancellationToken);
            updateTaskCancellationTokenSource = new CancellationTokenSource();
            _ = Task.Run(() => UpdateTask(updateTaskCancellationTokenSource.Token), updateTaskCancellationTokenSource.Token);
            messageReceiver.OnConnectOK();
        }
        catch (Exception)
        {
            Disconnect();
            messageReceiver.OnConnectionFail();
            return false;
        }
        return true;
    }

    public virtual void Disconnect()
    {
        if (!isConnected)
            return;
        isConnected = false;
        client.Close();
        encKey = [];
        curR = 0;
        curW = 0;
        ClearMessageQueue();
        updateTaskCancellationTokenSource.Cancel();
        Receiver.OnDisconnected();
    }

    public virtual bool EnqueueMessage(MessageSend message)
    {
        if (!isConnected)
            return false;
        if (!client.Connected)
        {
            Disconnect();
            return false;
        }
        pendingMessages.Enqueue(message);
        return true;
    }

    public virtual async Task UpdateTask(CancellationToken cancellationToken)
    {
        while (isConnected)
        {
            if (!client.Connected)
            {
                Disconnect();
                return;
            }
            try
            {
                await _processMsgQueueAsync(cancellationToken);
                await _processReceivedMessagesAsync(cancellationToken);
            }
            catch { }
        }
    }

    public virtual void ClearMessageQueue()
    {
        pendingMessages.Clear();
    }

    protected async Task _processMsgQueueAsync(CancellationToken cancellationToken)
    {
        if (encKey.Length == 0)
            return;
        if (pendingMessages.Count == 0)
            return;
        MessageSend message = pendingMessages.Dequeue();
        await _sendMessageAsync(message, cancellationToken);
    }

    protected async Task _processReceivedMessagesAsync(CancellationToken cancellationToken)
    {
        if (client.Available == 0)
            return;
        StreamedMessageRecv message = await _readMessageAsync(cancellationToken);
        Memory<byte> buffer = new byte[1];
        if (message.Command == 0xE5) //-27
        {
            _loadEncKey(message);
            //read to end
            for (int i = message.Position; i < message.Length; ++i)
                await Stream.ReadExactlyAsync(buffer, cancellationToken);
        }
        else
        {
            messageReceiver.OnMessageReceived(message);
            //read to end
            for (int i = message.Position; i < message.Length; ++i)
            {
                await Stream.ReadExactlyAsync(buffer, cancellationToken);
                _applyEncryption(0, EncryptionType.ENC_RECV);
            }
        }
    }

    protected async Task _sendMessageAsync(MessageSend message, CancellationToken cancellationToken)
    {
        byte cmd = _applyEncryption(message.Command, EncryptionType.ENC_SEND);
        await Stream.WriteAsync(new byte[] { cmd }, cancellationToken);
        ushort length = (ushort)message.Length;
        if (encKey.Length == 0)
            await Stream.WriteAsync(BitConverter.GetBytes(length), cancellationToken);
        else
        {
            byte b = _applyEncryption((byte)((length >> 8) & 0xFF), EncryptionType.ENC_SEND);
            await Stream.WriteAsync(new byte[] { b }, cancellationToken);
            b = _applyEncryption((byte)(length & 0xFF), EncryptionType.ENC_SEND);
            await Stream.WriteAsync(new byte[] { b }, cancellationToken);
        }
        if (message.Length > 0)
        {
            byte data;
            for (int i = 0; i < message.Length; ++i)
            {
                data = _applyEncryption(message.Data[i], EncryptionType.ENC_SEND);
                await Stream.WriteAsync(new byte[] { data }, cancellationToken);
            }
        }
    }

    protected async Task<StreamedMessageRecv> _readMessageAsync(CancellationToken cancellationToken)
    {
        byte cmd = _applyEncryption((byte)Stream.ReadByte(), EncryptionType.ENC_RECV);
        int len;
        Memory<byte> buffer = new byte[1];
        if (BIG_MSG_CMDs.Contains(cmd))
        {
            // uint24
            await Stream.ReadExactlyAsync(buffer, cancellationToken);
            byte b1 = (byte)(_applyEncryption(buffer.Span[0], EncryptionType.ENC_RECV) + 128);
            await Stream.ReadExactlyAsync(buffer, cancellationToken);
            byte b2 = (byte)(_applyEncryption(buffer.Span[0], EncryptionType.ENC_RECV) + 128);
            await Stream.ReadExactlyAsync(buffer, cancellationToken);
            byte b3 = (byte)(_applyEncryption(buffer.Span[0], EncryptionType.ENC_RECV) + 128);
            len = (b3 << 16) | (b2 << 8) | b1;
        }
        else
        {
            // uint16
            await Stream.ReadExactlyAsync(buffer, cancellationToken);
            byte b1 = _applyEncryption(buffer.Span[0], EncryptionType.ENC_RECV);
            await Stream.ReadExactlyAsync(buffer, cancellationToken);
            byte b2 = _applyEncryption(buffer.Span[0], EncryptionType.ENC_RECV);
            len = (b1 << 8) | b2;
        }
        StreamedMessageRecv message = new StreamedMessageRecv(cmd, Stream, len, data => _applyEncryption(data, EncryptionType.ENC_RECV));
        return message;
    }

    protected void _loadEncKey(MessageRecv message)
    {
        byte keyLength = message.ReadUInt8();
        byte[] key = message.ReadRawBytes(keyLength);
        for (int i = 0; i < keyLength - 1; i++)
            key[i + 1] ^= key[i];
        encKey = key;
        // string IP2 = message.ReadString();
        // ushort PORT2 = (ushort)message.ReadUInt32();
        // bool isConnect2 = message.ReadUInt8() != 0;
    }

    protected byte _applyEncryption(byte data, EncryptionType type)
    {
        if (encKey.Length == 0 || type == EncryptionType.NONE)
            return data;
        byte encKeySize = (byte)encKey.Length;
        if (type == EncryptionType.ENC_SEND)
        {
            byte index = curW;
            curW++;
            if (curW >= encKeySize)
                curW %= encKeySize;
            return (byte)(data ^ encKey[index]);
        }
        else if (type == EncryptionType.ENC_RECV)
        {
            byte index = curR;
            curR++;
            if (curR >= encKeySize)
                curR %= encKeySize;
            return (byte)(data ^ encKey[index]);
        }
        return data;
    }

    protected NetworkStream Stream => client.GetStream();

    bool disposed;
    public virtual void Dispose()
    {
        if (disposed)
            return;
        try
        {
            Disconnect();
            updateTaskCancellationTokenSource.Dispose();
            client.Dispose();
        }
        catch { }
        disposed = true;
        GC.SuppressFinalize(this);
    }
}

#nullable disable

public abstract class MessageSenderBase
{
    public void SetSession(IGameSession session)
    {
        gameSession = session;
    }

    protected void EnqueueMessage(MessageSend message)
    {
        gameSession.EnqueueMessage(message);
    }

    protected IGameSession gameSession;

    public abstract void UpdateMap();
    public abstract void UpdateItem();
    public abstract void UpdateSkill();
    public abstract void UpdateData();
    public abstract void ClientOk();
    public abstract void SetClientType(byte typeClient = 4, byte zoomLevel = 1, uint width = 720, uint height = 320, bool isQwerty = true, bool isTouch = true);
    public abstract void ImageSource();
    public abstract void Login(string username, string password, bool registered);
    public abstract void FinishUpdate();
    public abstract void FinishLoadMap();
    public abstract void RequestIcon(int id);
    public abstract void RequestChangeZone(byte zoneId);
    public abstract void GetResource(byte action);
    public abstract void RequestMobTemplate(short mobTemplateID);
    public abstract void RequestMapTemplate(int maptemplateID);
}

public abstract class MessageReceiverBase
{
    public GameEvents EventListeners { get; } = new GameEvents();

    public void SetSession(IGameSession session)
    {
        gameSession = session;
    }

    public abstract void OnMessageReceived(MessageRecv message);

    public abstract void OnConnectionFail();

    public abstract void OnDisconnected();

    public abstract void OnConnectOK();

    protected IGameSession gameSession;

    protected void OnIPAddressListReceived(string ipList) => EventListeners.OnIPAddressListReceived(ipList);
    protected void OnDialogMessageReceived(string message) => EventListeners.OnDialogMessageReceived(message);
    protected void OnServerMessageReceived(string message) => EventListeners.OnServerMessageReceived(message);
    protected void OnServerAlertReceived(string alert) => EventListeners.OnServerAlertReceived(alert);
    protected void OnGameNotificationReceived(string notification) => EventListeners.OnGameNotificationReceived(notification);
    protected void OnServerChatReceived(string sender, string message) => EventListeners.OnServerChatReceived(sender, message);
    protected void OnPrivateChatReceived(string sender, string message) => EventListeners.OnPrivateChatReceived(sender, message);
    protected void OnServerNotificationReceived(string notification) => EventListeners.OnServerNotificationReceived(notification);
    protected void OnUnknownMessageReceived(string message) => EventListeners.OnUnknownMessageReceived(message);
}

#nullable restore