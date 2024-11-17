using System.IO;
using System.Text;
using DataNRO.Interfaces;

namespace DataNRO
{
    /// <summary>
    /// Mô tả một gói tin nhận được từ máy chủ.
    /// </summary>
    public class MessageReceive : IMessage
    {
        byte[] buffer;
        sbyte cmd;
        BinaryReader reader;

        public sbyte Command => cmd;
        public byte[] Buffer => buffer;
        public long DataLength => buffer.GetLongLength(0);
        public long CurrentPosition => reader.BaseStream.Position;

        /// <summary>
        /// Khởi tạo một gói tin nhận được từ máy chủ
        /// </summary>
        /// <param name="command">Lệnh của gói tin</param>
        /// <param name="buffer">Dữ liệu của gói tin</param>
        public MessageReceive(sbyte command, byte[] buffer)
        {
            cmd = command;
            this.buffer = buffer;
            reader = new BinaryReader(new MemoryStream(buffer));
        }

        ///<inheritdoc cref="MessageReceive(sbyte, byte[])"/> 
        public MessageReceive(sbyte command, sbyte[] buffer)
        {
            cmd = command;
            this.buffer = new byte[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
                this.buffer[i] = (byte)buffer[i];
            reader = new BinaryReader(new MemoryStream(this.buffer));
        }

        /// <summary>Đọc giá trị <see langword="bool"/> từ dữ liệu của gói tin</summary>
        public bool ReadBool() => reader.ReadBoolean();
        /// <summary>Đọc giá trị <see langword="byte"/> từ dữ liệu của gói tin</summary>
        public byte ReadByte() => reader.ReadByte();
        /// <summary>Đọc giá trị <see langword="sbyte"/> từ dữ liệu của gói tin</summary>
        public sbyte ReadSByte() => reader.ReadSByte();
        /// <summary>Đọc giá trị <see langword="short"/> từ dữ liệu của gói tin</summary>
        public short ReadShort() => reader.ReadInt16BE();
        /// <summary>Đọc giá trị <see langword="ushort"/> từ dữ liệu của gói tin</summary>
        public ushort ReadUShort() => reader.ReadUInt16BE();
        /// <summary>Đọc giá trị <see langword="char"/> từ dữ liệu của gói tin</summary>
        public char ReadChar() => reader.ReadChar();
        /// <summary>Đọc giá trị <see langword="int"/> từ dữ liệu của gói tin</summary>
        public int ReadInt() => reader.ReadInt32BE();
        /// <summary>Đọc giá trị <see langword="uint"/> từ dữ liệu của gói tin</summary>
        public uint ReadUInt() => reader.ReadUInt32BE();
        /// <summary>Đọc giá trị <see langword="long"/> từ dữ liệu của gói tin</summary>
        public long ReadLong() => reader.ReadInt64BE();
        /// <summary>Đọc giá trị <see langword="ulong"/> từ dữ liệu của gói tin</summary>
        public ulong ReadULong() => reader.ReadUInt64BE();
        /// <summary>
        /// Đọc một mảng <see langword="byte"/> với độ dài là 4 byte đầu từ dữ liệu của gói tin
        /// </summary>
        public byte[] ReadBytes() => ReadBytes(ReadInt());
        /// <summary>
        /// Đọc một mảng <see langword="byte"/> từ dữ liệu của gói tin
        /// </summary>
        /// <param name="count">Độ dài mảng cần đọc</param>
        public byte[] ReadBytes(int count) => reader.ReadBytes(count);

        /// <summary>
        /// Đọc một mảng <see langword="sbyte"/> từ dữ liệu của gói tin
        /// </summary>
        /// <param name="count">Độ dài mảng cần đọc</param>
        public sbyte[] ReadSBytes(int count)
        {
            byte[] data = reader.ReadBytes(count);
            sbyte[] result = new sbyte[count];
            for (int i = 0; i < count; i++)
                result[i] = (sbyte)data[i];
            return result;
        }

        /// <summary>Đọc giá trị <see langword="string"/> từ dữ liệu của gói tin</summary>
        public string ReadString()
        {
            short length = ReadShort();
            byte[] data = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(data);
        }

        public void Dispose() => reader.Dispose();
    }
}
