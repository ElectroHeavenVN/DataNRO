using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DataNRO.Interfaces;
using Starksoft.Net.Proxy;

namespace DataNRO.TeaMobi
{
    public class TeaMobiSession : ISession
    {
        public string Host { get; }
        public ushort Port { get; }
        public IMessageReceiver MessageReceiver { get; private set; }
        public IMessageWriter MessageWriter { get; private set; }
        public bool IsConnected => tcpClient == null ? false : tcpClient.Connected;
        public GameData Data { get; } = new GameData();
        public Player Player { get; } = new Player();

        Thread receiveThread;
        Thread sendThread;
        TcpClient tcpClient;
        BinaryReader reader;
        BinaryWriter writer;
        Queue<MessageSend> sendMessages = new Queue<MessageSend>();
        bool getKeyComplete;
        sbyte[] key;
        sbyte curR;
        sbyte curW;

        public TeaMobiSession(string host, ushort port)
        {
            Host = host;
            Port = port;
            MessageReceiver = new TeaMobiMessageReceiver(this);
            MessageWriter = new TeaMobiMessageWriter(this);
        }

        public void Connect()
        {
            tcpClient = new TcpClient();
            if (!tcpClient.ConnectAsync(Host, Port).Wait(5000))
                throw new TimeoutException($"Connection to {Host}:{Port} timeout");
            reader = new BinaryReader(tcpClient.GetStream(), Encoding.UTF8);
            writer = new BinaryWriter(tcpClient.GetStream(), Encoding.UTF8);
            sendThread = new Thread(SendDataThread);
            receiveThread = new Thread(ReceiveDataThread);
            sendThread.Start();
            receiveThread.Start();
            key = null;
            WriteMessageToStream(new MessageSend(-27));
        }
        
        public void Connect(string proxyHost, ushort proxyPort, string proxyUsername, string proxyPassword, ProxyType proxyType)
        {
            ProxyClientFactory proxyClientFactory = new ProxyClientFactory();
            IProxyClient proxyClient = proxyClientFactory.CreateProxyClient(proxyType, proxyHost, proxyPort, proxyUsername, proxyPassword);
            tcpClient = proxyClient.CreateConnection(Host, Port);
            reader = new BinaryReader(tcpClient.GetStream(), Encoding.UTF8);
            writer = new BinaryWriter(tcpClient.GetStream(), Encoding.UTF8);
            sendThread = new Thread(SendDataThread);
            receiveThread = new Thread(ReceiveDataThread);
            sendThread.Start();
            receiveThread.Start();
            key = null;
            WriteMessageToStream(new MessageSend(-27));
        }

        public void SendMessage(MessageSend message)
        {
            sendMessages.Enqueue(message);
        }

        public void Disconnect()
        {
            tcpClient.Close();
            reader.Close();
            writer.Close();
            sendThread.Abort();
            receiveThread.Abort();
        }

        void SendDataThread()
        {
            while (tcpClient.Connected)
            {
                try
                {
                    if (getKeyComplete && sendMessages.Count > 0)
                    {
                        MessageSend message = sendMessages.Dequeue();
                        Console.WriteLine($"[{Host}:{Port}] Send message: {message.Command}, {message.DataLength} bytes");
                        WriteMessageToStream(message);
                    }
                    Thread.Sleep(5);
                }
                catch { }
            }
        }

        void ReceiveDataThread()
        {
            while (tcpClient.Connected)
            {
                try
                {
                    MessageReceive message = ReadMessageFromStream();
                    if (message == null)
                        break;
                    if (message.Command == -27)
                        GetKey(message);
                    else
                        MessageReceiver.OnMessageReceived(message);
                    Thread.Sleep(5);
                }
                catch { }
            }
        }

        void WriteMessageToStream(MessageSend m)
        {
            sbyte[] data = new sbyte[m.Buffer.Length];
            Buffer.BlockCopy(m.Buffer, 0, data, 0, m.Buffer.Length);
            try
            {
                if (getKeyComplete)
                {
                    sbyte value = WriteKey(m.Command);
                    writer.Write(value);
                }
                else
                    writer.Write(m.Command);
                if (data != null)
                {
                    ushort length = (ushort)data.Length;
                    if (getKeyComplete)
                    {
                        byte[] d = BitConverter.GetBytes(length);
                        writer.Write(WriteKey((sbyte)d[1]));
                        writer.Write(WriteKey((sbyte)d[0]));
                        for (int i = 0; i < data.Length; i++)
                        {
                            sbyte value2 = WriteKey(data[i]);
                            writer.Write(value2);
                        }
                    }
                    else
                        writer.Write(length);
                }
                else
                {
                    if (getKeyComplete)
                    {
                        writer.Write(WriteKey(0));
                        writer.Write(WriteKey(0));
                    }
                    else
                        writer.Write((ushort)0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{Host}:{Port}] Error sending message:\r\n{ex}");
                writer.Flush();
            }
        }

        MessageReceive ReadMessageFromStream()
        {
            try
            {
                sbyte b = reader.ReadSByte();
                if (getKeyComplete)
                    b = ReadKey(b);
                if (b == -32 || b == -66 || b == 11 || b == -67 || b == -74 || b == -87 || b == 66)
                {
                    int num = ReadKey(reader.ReadSByte()) + 128;
                    int num2 = ReadKey(reader.ReadSByte()) + 128;
                    int num3 = ((ReadKey(reader.ReadSByte()) + 128) * 256 + num2) * 256 + num;
                    sbyte[] data = new sbyte[num3];
                    Buffer.BlockCopy(reader.ReadBytes(num3), 0, data, 0, num3);
                    if (getKeyComplete)
                    {
                        for (int i = 0; i < data.Length; i++)
                            data[i] = ReadKey(data[i]);
                    }
                    return new MessageReceive(b, data);
                }
                int length;
                if (getKeyComplete)
                    length = ((ReadKey(reader.ReadSByte()) & 0xFF) << 8) | (ReadKey(reader.ReadSByte()) & 0xFF);
                else
                    length = reader.ReadUInt16BE();
                sbyte[] arr = new sbyte[length];
                Buffer.BlockCopy(reader.ReadBytes(length), 0, arr, 0, length);
                if (getKeyComplete)
                {
                    for (int i = 0; i < arr.Length; i++)
                        arr[i] = ReadKey(arr[i]);
                }
                return new MessageReceive(b, arr);
            }
            catch { }
            return null;
        }

        void GetKey(MessageReceive message)
        {
            try
            {
                sbyte b = message.ReadSByte();
                key = new sbyte[b];
                for (int i = 0; i < b; i++)
                {
                    key[i] = message.ReadSByte();
                }
                for (int j = 0; j < key.Length - 1; j++)
                {
                    ref sbyte reference = ref key[j + 1];
                    reference ^= key[j];
                }
                getKeyComplete = true;
                string IP2 = message.ReadString();
                ushort PORT2 = (ushort)message.ReadInt();
                bool isConnect2 = message.ReadBool();
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
            Disconnect();
            tcpClient.Dispose();
            Data.Reset();
        }

        sbyte ReadKey(sbyte b)
        {
            sbyte[] array = key;
            sbyte num = curR;
            curR = (sbyte)(num + 1);
            sbyte result = (sbyte)((array[num] & 0xFF) ^ (b & 0xFF));
            if (curR >= key.Length)
                curR %= (sbyte)key.Length;
            return result;
        }

        sbyte WriteKey(sbyte b)
        {
            sbyte[] array = key;
            sbyte num = curW;
            curW = (sbyte)(num + 1);
            sbyte result = (sbyte)((array[num] & 0xFF) ^ (b & 0xFF));
            if (curW >= key.Length)
                curW %= (sbyte)key.Length;
            return result;
        }
    }
}
