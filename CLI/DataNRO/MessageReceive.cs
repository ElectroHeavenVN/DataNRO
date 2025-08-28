using System.IO;
using System.Text;
using EHVN.DataNRO.Interfaces;

namespace EHVN.DataNRO
{
    /// <summary>
    /// Mô tả một gói tin nhận được từ máy chủ.
    /// </summary>
    public class MessageReceive : IMessage
    {
        sbyte cmd;
        DataReader reader;

        public sbyte Command => cmd;
        public byte[] Buffer => reader.Buffer;
        public long DataLength => reader.Buffer.GetLongLength(0);
        public long CurrentPosition => reader.CurrentPosition;

        /// <summary>
        /// Khởi tạo một gói tin nhận được từ máy chủ
        /// </summary>
        /// <param name="command">Lệnh của gói tin</param>
        /// <param name="buffer">Dữ liệu của gói tin</param>
        public MessageReceive(sbyte command, byte[] buffer)
        {
            cmd = command;
            reader = new DataReader(buffer);
        }

        ///<inheritdoc cref="MessageReceive(sbyte, byte[])"/> 
        public MessageReceive(sbyte command, sbyte[] buffer)
        {
            cmd = command;
            reader = new DataReader(buffer);
        }

        /// <summary>Đọc giá trị <see langword="bool"/> từ dữ liệu của gói tin</summary>
        public bool ReadBool() => reader.ReadBool();
        
        /// <summary>Đọc giá trị <see langword="byte"/> từ dữ liệu của gói tin</summary>
        public byte ReadByte() => reader.ReadByte();
        
        /// <summary>Đọc giá trị <see langword="sbyte"/> từ dữ liệu của gói tin</summary>
        public sbyte ReadSByte() => reader.ReadSByte();

        /// <summary>Đọc giá trị <see langword="short"/> từ dữ liệu của gói tin</summary>
        public short ReadShort() => reader.ReadShort();

        /// <summary>Đọc giá trị <see langword="ushort"/> từ dữ liệu của gói tin</summary>
        public ushort ReadUShort() => reader.ReadUShort();

        /// <summary>Đọc giá trị <see langword="char"/> từ dữ liệu của gói tin</summary>
        public char ReadChar() => reader.ReadChar();
        
        /// <summary>Đọc giá trị <see langword="int"/> từ dữ liệu của gói tin</summary>
        public int ReadInt() => reader.ReadInt();

        /// <summary>Đọc giá trị <see langword="uint"/> từ dữ liệu của gói tin</summary>
        public uint ReadUInt() => reader.ReadUInt();

        /// <summary>Đọc giá trị <see langword="long"/> từ dữ liệu của gói tin</summary>
        public long ReadLong() => reader.ReadLong();

        /// <summary>Đọc giá trị <see langword="ulong"/> từ dữ liệu của gói tin</summary>
        public ulong ReadULong() => reader.ReadULong();

        /// <summary>
        /// Đọc một mảng <see langword="byte"/> với độ dài là 4 byte đầu từ dữ liệu của gói tin
        /// </summary>
        public byte[] ReadBytes() => reader.ReadBytes();
        /// <summary>
        /// Đọc một mảng <see langword="byte"/> từ dữ liệu của gói tin
        /// </summary>
        /// <param name="count">Độ dài mảng cần đọc</param>
        public byte[] ReadBytes(int count) => reader.ReadBytes(count);

        /// <summary>
        /// Đọc một mảng <see langword="sbyte"/> từ dữ liệu của gói tin
        /// </summary>
        /// <param name="count">Độ dài mảng cần đọc</param>
        public sbyte[] ReadSBytes(int count) => reader.ReadSBytes(count);

        /// <summary>Đọc giá trị <see langword="string"/> từ dữ liệu của gói tin</summary>
        public string ReadString() => reader.ReadString();

        public void Dispose() => reader.Dispose();
    }
}
