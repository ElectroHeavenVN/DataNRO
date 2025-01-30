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
        public FileWriter FileWriter { get; private set; }
        public bool IsConnected => tcpClient == null ? false : tcpClient.Connected;
        public GameData Data { get; } = new GameData();
        public Player Player { get; } = new Player();

        //13: low graphics
        public Dictionary<int, int> mapTileIDs = new Dictionary<int, int>()
        {
            { 0, 1 }, { 1, 1 }, { 2, 1 }, { 3, 2 }, { 4, 2 }, { 5, 3 }, { 6, 4 },
            { 7, 5 }, { 8, 5 }, { 9, 5 }, { 10, 6 }, { 11, 7 }, { 12, 7 }, { 13, 8 },
            { 14, 9 }, { 15, 9 }, { 16, 9 }, { 17, 10 }, { 18, 10 }, { 19, 11 }, { 20, 12 },
            { 21, 1 }, { 22, 5 }, { 23, 9 }, 
            { 24, 1 }, { 25, 5 }, { 26, 9 },
            { 27, 2 }, { 28, 2 }, { 29, 3 }, { 30, 3 },
            { 31, 7 }, { 32, 7 }, { 33, 8 }, { 34, 8 }, 
            { 35, 10 }, { 36, 10 }, { 37, 12 }, { 38, 12 },
            { 39, 1 }, { 40, 5 }, { 41, 9 },
            { 42, 1 }, { 43, 5 }, { 44, 9 },
            { 45, 13 }, { 46, 13 },
            { 47, 4 }, 
            { 48, 13 }, 
            { 49, 13 },
            { 50, 10 },
            { 51, 15 },
            { 52, 9 },
            { 53, 16 }, { 54, 17 }, { 55, 17 }, { 56, 17 }, { 57, 17 }, { 58, 16 }, { 59, 16 }, { 60, 18 }, { 61, 18 }, { 62, 18 },
            { 63, 19 }, { 64, 19 }, { 65, 19 }, { 66, 19 }, { 67, 19 }, 
            { 68, 20 }, { 69, 29 }, { 70, 20 }, { 71, 20 }, { 72, 20 }, 
            { 73, 21 }, { 74, 21 }, { 75, 21 }, { 76, 21 }, { 77, 21 },
            
            { 79, 19 }, { 80, 19 }, { 81, 20 }, { 82, 20 }, { 83, 21 },
            { 84, 11 },
            { 85, 20 }, { 86, 6 }, { 87, 11 }, { 88, 8 }, { 89, 3 }, { 90, 10 }, { 91, 1 },
            { 92, 22 }, { 93, 22 }, { 94, 22 }, { 95, 22 }, { 96, 22 }, { 97, 23 }, { 98, 23 }, { 99, 23 }, { 100, 23 }, { 101, 23 }, { 102, 22 }, { 103, 23 }, 
            { 104, 11 }, 
            { 105, 124 }, { 106, 24 }, { 107, 24 }, { 108, 25 }, { 109, 25 }, { 110, 25 },
            { 111, 4 },
            { 112, 26 },
            { 113, 23 }, 
            { 114, 12 }, { 115, 12 }, 
            { 116, 12 }, 
            { 117, 12 }, { 118, 13 }, { 119, 12 },
            { 120, 12 }, { 121, 12 }, 
            { 122, 1 }, { 123, 1 }, { 124, 1 },
            { 125, 12 },
            { 126, 11 },
            { 127, 11 },
            { 128, 1 },
            { 129, 23 }, { 130, 23 },
            { 131, 27 }, { 132, 27 }, { 133, 27 }, 
            { 134, 27 },
            { 135, 28 }, { 136, 28 }, { 137, 28 }, { 138, 28 },
            { 139, 9 }, { 140, 9 },
            { 141, 14 }, { 142, 14 }, { 143, 14 }, 
            { 144, 12 }, { 145, 13 }, { 146, 4 }, { 147, 3 }, { 148, 15 }, { 149, 11 }, { 150, 12 }, { 151, 12 }, { 152, 25 }, 
            { 153, 1 }, 
            { 154, 29 }, { 155, 30 }, { 156, 19 }, { 157, 19 }, { 158, 25 }, { 159, 25 }, 
            { 160, 31 }, { 161, 31 }, { 162, 31 }, { 163, 31 }, 
            
            { 165, 1 }, 
            { 166, 30 }, { 167, 32 }, { 168, 32 },
            
            { 170, 14 }, 
            { 171, 32 }, 
            { 172, 13 },

            { 176, 3 }, { 177, 10 }, { 178, 23 },

            { 78, -1 },
            { 164, -1 },
            { 169, -1 },
            { 173, -1 }, { 174, -1 }, { 175, -1 },
        };
        public Dictionary<int, int> MapTileIDs => mapTileIDs;

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
            FileWriter = new FileWriter(this);
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
                if (b == -32 || b == -66 || b == 11 || b == -67 || b == -74 || b == -87 || b == 66 || b == 12)
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
            try
            {
                Disconnect();
                tcpClient.Dispose();
            }
            catch { }
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
