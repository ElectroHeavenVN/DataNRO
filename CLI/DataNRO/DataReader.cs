using System;
using System.IO;
using System.Text;

namespace EHVN.DataNRO
{
    /// <summary>
    /// Đọc dữ liệu từ một mảng <see langword="byte"/> hoặc <see langword="sbyte"/>
    /// </summary>
    public class DataReader : IDisposable
    {
        byte[] buffer;
        BinaryReader reader;

        public byte[] Buffer => buffer;
        public long DataLength => buffer.GetLongLength(0);
        public long CurrentPosition => reader.BaseStream.Position;

        /// <summary>
        /// Khởi tạo một đối tượng <see cref="DataReader"/> từ một mảng <see langword="byte"/>
        /// </summary>
        /// <param name="buffer">
        /// Mảng <see langword="byte"/> chứa dữ liệu cần đọc
        /// </param>
        public DataReader(byte[] buffer)
        {
            this.buffer = buffer;
            reader = new BinaryReader(new MemoryStream(buffer));
        }

        /// <summary>
        /// Khởi tạo một đối tượng <see cref="DataReader"/> từ một mảng <see langword="sbyte"/>
        /// </summary>
        /// <param name="buffer">
        /// Mảng <see langword="sbyte"/> chứa dữ liệu cần đọc
        /// </param>
        public DataReader(sbyte[] buffer)
        {
            this.buffer = new byte[buffer.Length];
            for (int i = 0; i < buffer.Length; i++)
                this.buffer[i] = (byte)buffer[i];
            reader = new BinaryReader(new MemoryStream(this.buffer));
        }

        /// <summary>Đọc giá trị <see langword="bool"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public bool ReadBool() => reader.ReadBoolean();

        /// <summary>Đọc giá trị <see langword="byte"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public byte ReadByte() => reader.ReadByte();

        /// <summary>Đọc giá trị <see langword="sbyte"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public sbyte ReadSByte() => reader.ReadSByte();

        /// <summary>Đọc giá trị <see langword="short"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public short ReadShort() => (short)((reader.ReadByte() << 8) | reader.ReadByte());

        /// <summary>Đọc giá trị <see langword="ushort"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public ushort ReadUShort() => (ushort)((reader.ReadByte() << 8) | reader.ReadByte());

        /// <summary>Đọc giá trị <see langword="char"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public char ReadChar() => reader.ReadChar();

        /// <summary>Đọc giá trị <see langword="int"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public int ReadInt() => reader.ReadByte() << 24 | reader.ReadByte() << 16 | reader.ReadByte() << 8 | reader.ReadByte();

        /// <summary>Đọc giá trị <see langword="uint"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public uint ReadUInt() => (uint)(reader.ReadByte() << 24 | reader.ReadByte() << 16 | reader.ReadByte() << 8 | reader.ReadByte());

        /// <summary>Đọc giá trị <see langword="long"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public long ReadLong() => (long)(
            ((ulong)reader.ReadByte() << 56) |
            ((ulong)reader.ReadByte() << 48) |
            ((ulong)reader.ReadByte() << 40) |
            ((ulong)reader.ReadByte() << 32) |
            ((ulong)reader.ReadByte() << 24) |
            ((ulong)reader.ReadByte() << 16) |
            ((ulong)reader.ReadByte() << 8) |
            (reader.ReadByte())
        );

        /// <summary>Đọc giá trị <see langword="ulong"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public ulong ReadULong() =>
            ((ulong)reader.ReadByte() << 56) |
            ((ulong)reader.ReadByte() << 48) |
            ((ulong)reader.ReadByte() << 40) |
            ((ulong)reader.ReadByte() << 32) |
            ((ulong)reader.ReadByte() << 24) |
            ((ulong)reader.ReadByte() << 16) |
            ((ulong)reader.ReadByte() << 8) |
            reader.ReadByte();

        /// <summary>
        /// Đọc một mảng <see langword="byte"/> với độ dài là 4 byte đầu từ dữ liệu tại <see cref="CurrentPosition"/>
        /// </summary>
        public byte[] ReadBytes() => ReadBytes(ReadInt());
        /// <summary>
        /// Đọc một mảng <see langword="byte"/> từ dữ liệu tại <see cref="CurrentPosition"/>
        /// </summary>
        /// <param name="count">Độ dài mảng cần đọc</param>
        public byte[] ReadBytes(int count) => reader.ReadBytes(count);

        /// <summary>
        /// Đọc một mảng <see langword="sbyte"/> từ dữ liệu tại <see cref="CurrentPosition"/>
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

        /// <summary>Đọc giá trị <see langword="string"/> từ dữ liệu tại <see cref="CurrentPosition"/></summary>
        public string ReadString()
        {
            short length = ReadShort();
            byte[] data = reader.ReadBytes(length);
            return Encoding.UTF8.GetString(data);
        }

        public void Dispose() => reader.Dispose();
    }
}
