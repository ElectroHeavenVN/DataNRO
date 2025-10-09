using EHVN.DataNRO.Interfaces;
using Starksoft.Net.Proxy;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace EHVN.DataNRO.TeaMobi
{
    public class TeaMobiSession : ISession
    {
        enum EncryptionType
        {
            None, Send, Receive
        }

        static readonly sbyte[] BIG_MSG_CMDs = [-32, -66, 11, -67, -74, -87, 66, 12];

        public string Host { get; }
        public ushort Port { get; }
        public IMessageReceiver MessageReceiver { get; set; }
        public IMessageWriter MessageWriter { get; set; }
        public FileWriter FileWriter { get; set; }
        public bool IsConnected => tcpClient is null ? false : tcpClient.Connected;
        public GameData Data { get; } = new GameData()
        {
            MapTileIDs = Config.MapTileIDs
        };
        public Player Player { get; } = new Player();

        TcpClient? tcpClient;
        NetworkStream? networkStream;
        ConcurrentQueue<MessageSend> sendMessages = [];
        SemaphoreSlim sendSignal = new SemaphoreSlim(0);
        byte[]? key;
        byte curR, curW;
        CancellationTokenSource cts = new CancellationTokenSource();
        public event EventHandler? OnDisconnected;

        public TeaMobiSession(string host, ushort port)
        {
            Host = host;
            Port = port;
            MessageReceiver = new TeaMobiMessageReceiver(this);
            MessageWriter = new TeaMobiMessageWriter(this);
            FileWriter = new FileWriter(this);
        }

        public async Task ConnectAsync(string? proxyHost = null, ushort proxyPort = 0, string? proxyUsername = null, string? proxyPassword = null, ProxyType proxyType = ProxyType.None, CancellationToken cancellationToken = default)
        {
            if (proxyType == ProxyType.None || string.IsNullOrEmpty(proxyHost) || proxyPort == 0 || string.IsNullOrEmpty(proxyUsername) || string.IsNullOrEmpty(proxyPassword))
            {
                tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(Host, Port, cancellationToken);
            }
            else
            {
                ProxyClientFactory proxyClientFactory = new ProxyClientFactory();
                IProxyClient proxyClient = proxyClientFactory.CreateProxyClient(proxyType, proxyHost, proxyPort, proxyUsername, proxyPassword);
                tcpClient = await proxyClient.CreateConnectionAsync(Host, Port, cancellationToken);
            }
            networkStream = tcpClient.GetStream();
            cts = new CancellationTokenSource();
            sendSignal = new SemaphoreSlim(0);
            _ = SendDataTask().ConfigureAwait(false);
            _ = ReceiveDataTask().ConfigureAwait(false);
            key = null;
            curR = 0;
            curW = 0;
            await SendMessageAsync(new MessageSend(-27), cancellationToken);
        }

        public void EnqueueMessage(MessageSend message)
        {
            if (!IsConnected)
                throw new InvalidOperationException("Not connected to server.");
            sendMessages.Enqueue(message);
            sendSignal.Release();
        }

        public void Disconnect()
        {
            try
            {
                cts.Cancel();
                sendSignal.Release(int.MaxValue);
            }
            catch { }
            try
            {
                networkStream?.Close();
                tcpClient?.Close();
                key = null;
            }
            catch { }
            try
            {
                OnDisconnected?.Invoke(this, EventArgs.Empty);
            }
            catch { }
        }

        async Task SendDataTask()
        {
            while (IsConnected && !cts.IsCancellationRequested)
            {
                while (key is null)
                    await Task.Delay(100);
                try
                {
                    await sendSignal.WaitAsync(cts.Token);
                    if (sendMessages.TryDequeue(out MessageSend? message))
                        await SendMessageAsync(message, cts.Token);
                }
                catch (Exception ex)
                {
                    if (ex is not OperationCanceledException)
                        Console.WriteLine($"[{Host}:{Port}] Exception:\r\n{ex}");
                    if (ex is SocketException || ex is TimeoutException || ex is EndOfStreamException || (ex is IOException && ex.InnerException is SocketException))
                    {
                        Disconnect();
                        return;
                    }
                }
            }
            Disconnect();
        }

        async Task ReceiveDataTask()
        {
            while (IsConnected && !cts.IsCancellationRequested)
            {
                try
                {
                    MessageReceive message = await ReadMessageAsync(cts.Token);
                    if (message.Command == -27)
                        LoadDecryptionKey(message);
                    else
                    {
                        try
                        {
                            MessageReceiver.OnMessageReceived(message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[{Host}:{Port}] OnMessageReceived Exception:\r\n{ex}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex is not OperationCanceledException)
                        Console.WriteLine($"[{Host}:{Port}] Exception:\r\n{ex}");
                    if (ex is SocketException || ex is TimeoutException || ex is EndOfStreamException || (ex is IOException && ex.InnerException is SocketException))
                    {
                        Disconnect();
                        return;
                    }
                }
            }
            Disconnect();
        }

        public async Task SendMessageAsync(MessageSend m, CancellationToken cancellationToken)
        {
            if (!IsConnected || networkStream is null)
                throw new InvalidOperationException("Not connected to server.");
            await networkStream.WriteAsync([ApplyEncryption(m.Command, EncryptionType.Send)], 0, 1, cancellationToken);
            byte[] data = BitConverter.GetBytes((ushort)m.Buffer.Length);
            if (key is null)
                await networkStream.WriteAsync(data, 0, 2, cancellationToken);
            else
            {
                Array.Reverse(data);
                await networkStream.WriteAsync(ApplyEncryption(data, EncryptionType.Send), 0, 2, cancellationToken);
            }
            if (m.Buffer.Length > 0)
                await networkStream.WriteAsync(ApplyEncryption(m.Buffer, EncryptionType.Send), 0, m.Buffer.Length, cancellationToken);
        }

        async Task<MessageReceive> ReadMessageAsync(CancellationToken cancellationToken)
        {
            if (!IsConnected || networkStream is null)
                throw new InvalidOperationException("Not connected to server.");
            byte[] buffer = new byte[1];
            await networkStream.ReadExactlyAsync(buffer, 0, 1, cancellationToken);
            sbyte b = ApplyEncryptionSigned(buffer[0], EncryptionType.Receive);
            int length;
            if (BIG_MSG_CMDs.Contains(b))
            {
                await networkStream.ReadExactlyAsync(buffer, 0, 1, cancellationToken);
                byte b1 = (byte)(ApplyEncryptionSigned(unchecked((sbyte)buffer[0]), EncryptionType.Receive) + 128);
                await networkStream.ReadExactlyAsync(buffer, 0, 1, cancellationToken);
                byte b2 = (byte)(ApplyEncryptionSigned(unchecked((sbyte)buffer[0]), EncryptionType.Receive) + 128);
                await networkStream.ReadExactlyAsync(buffer, 0, 1, cancellationToken);
                byte b3 = (byte)(ApplyEncryptionSigned(unchecked((sbyte)buffer[0]), EncryptionType.Receive) + 128);
                length = (b3 << 16) | (b2 << 8) | b1;
            }
            else
            {
                await networkStream.ReadExactlyAsync(buffer, 0, 1, cancellationToken);
                byte b1 = ApplyEncryption(buffer[0], EncryptionType.Receive);
                await networkStream.ReadExactlyAsync(buffer, 0, 1, cancellationToken);
                byte b2 = ApplyEncryption(buffer[0], EncryptionType.Receive);
                length = (b1 << 8) | b2;
            }
            buffer = new byte[length];
            await networkStream.ReadExactlyAsync(buffer, 0, length, cancellationToken);
            return new MessageReceive(b, ApplyEncryption(buffer, EncryptionType.Receive));
        }

        void LoadDecryptionKey(MessageReceive message)
        {
            byte[] bytes = message.ReadBytes(message.ReadByte());
            for (int i = 0; i < bytes.Length - 1; i++)
                bytes[i + 1] ^= bytes[i];
            key = bytes;
            string IP2 = message.ReadString();
            ushort PORT2 = (ushort)message.ReadInt();
            bool isConnect2 = message.ReadBool();
        }

        bool disposed;
        public void Dispose()
        {
            if (disposed)
                return;
            try
            {
                Disconnect();
                cts.Dispose();
                sendSignal.Dispose();
                tcpClient?.Dispose();
            }
            catch { }
            Data.Reset();
            GC.SuppressFinalize(this);
        }

        byte ApplyEncryption(sbyte b, EncryptionType type) => ApplyEncryption(unchecked((byte)b), type);

        sbyte ApplyEncryptionSigned(byte b, EncryptionType type) => ApplyEncryptionSigned(unchecked((sbyte)b), type);

        byte[] ApplyEncryption(byte[] data, EncryptionType type)
        {
            if (key is null)
                return data;
            byte[] array = new byte[data.Length];
            for (int i = 0; i < data.Length; i++)
                array[i] = ApplyEncryption(data[i], type);
            return array;
        }

        byte ApplyEncryption(byte b, EncryptionType type)
        {
            if (key is null)
                return b;
            if (type == EncryptionType.Send)
            {
                byte index = curW;
                curW++;
                if (curW >= key.Length)
                    curW %= (byte)key.Length;
                return (byte)(key[index] ^ b);
            }
            else if (type == EncryptionType.Receive)
            {
                byte index = curR;
                curR++;
                if (curR >= key.Length)
                    curR %= (byte)key.Length;
                return (byte)(key[index] ^ b);
            }
            else
                return b;
        }

        sbyte ApplyEncryptionSigned(sbyte b, EncryptionType type)
        {
            if (key is null)
                return b;
            if (type == EncryptionType.Send)
            {
                byte index = curW;
                curW++;
                if (curW >= key.Length)
                    curW %= (byte)key.Length;
                return (sbyte)((key[index] & 0xFF) ^ (b & 0xFF));
            }
            else if (type == EncryptionType.Receive)
            {
                byte index = curR;
                curR++;
                if (curR >= key.Length)
                    curR %= (byte)key.Length;
                return (sbyte)((key[index] & 0xFF) ^ (b & 0xFF));
            }
            else
                return b;
        }
    }
}
