using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataNRO.Interfaces;

namespace DataNRO
{
    /// <summary>
    /// Mô tả một gói tin gửi tới máy chủ.
    /// </summary>
    public class MessageSend : IMessage
    {
        List<byte> buffer;
        sbyte cmd;

        public sbyte Command => cmd;
        public byte[] Buffer => buffer.ToArray();
        public long DataLength => buffer.LongCount();
        public long CurrentPosition => DataLength;

        /// <summary>
        /// Khởi tạo một gói tin gửi tới máy chủ không chứa dữ liệu
        /// </summary>
        /// <param name="command">Lệnh của gói tin</param>
        public MessageSend(sbyte command)
        {
            cmd = command;
            buffer = new List<byte>();
        }

        /// <summary>
        /// Khởi tạo một gói tin gửi tới máy chủ
        /// </summary>
        /// <param name="command">Lệnh của gói tin</param>
        /// <param name="buffer">Dữ liệu của gói tin</param>
        public MessageSend(sbyte command, byte[] buffer)
        {
            cmd = command;
            this.buffer = new List<byte>(buffer);
        }

        /// <summary>Viết dữ liệu dưới dạng <see langword="byte"/> của <paramref name="value"/> vào dữ liệu của gói tin</summary>
        public void WriteBool(bool value) => buffer.AddRange(BitConverter.GetBytes(value));
        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteByte(byte value) => buffer.Add(value);
        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteSByte(sbyte value) => buffer.Add((byte)value);
        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteShort(short value) => buffer.AddRange(BitConverter.GetBytes(value).Reverse());
        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteUShort(ushort value) => buffer.AddRange(BitConverter.GetBytes(value).Reverse());
        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteChar(char value) => buffer.AddRange(BitConverter.GetBytes(value).Reverse());
        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteInt(int value) => buffer.AddRange(BitConverter.GetBytes(value).Reverse());
        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteUInt(uint value) => buffer.AddRange(BitConverter.GetBytes(value).Reverse());
        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteLong(long value) => buffer.AddRange(BitConverter.GetBytes(value).Reverse());
        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteULong(ulong value) => buffer.AddRange(BitConverter.GetBytes(value).Reverse());
        /// <summary>
        /// Thêm <paramref name="value"/> vào dữ liệu của gói tin
        /// </summary>
        /// <param name="value"></param>
        public void WriteBytes(byte[] value) => buffer.AddRange(value);

        /// <summary>
        /// Viết dữ liệu Unicode dưới dạng <see langword="byte"/> của <paramref name="value"/> vào dữ liệu của gói tin
        /// </summary>
        /// <param name="value"></param>
        public void WriteString(string value)
        {
            char[] chars = value.ToCharArray();
            WriteShort((short)chars.Length);
            buffer.AddRange(chars.Cast<byte>());
        }

        /// <summary>
        /// Viết dữ liệu UTF-8 dưới dạng <see langword="byte"/> của <paramref name="value"/> vào dữ liệu của gói tin
        /// </summary>
        /// <param name="value"></param>
        public void WriteStringUTF(string value)
        {
            byte[] array = Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(65001), Encoding.Unicode.GetBytes(value));
            WriteShort((short)array.Length);
            WriteBytes(array);
        }

        public void Dispose() => buffer.Clear();
    }
}
