using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EHVN.DataNRO.Interfaces;

namespace EHVN.DataNRO
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
            buffer = [];
        }

        /// <summary>
        /// Khởi tạo một gói tin gửi tới máy chủ
        /// </summary>
        /// <param name="command">Lệnh của gói tin</param>
        /// <param name="buffer">Dữ liệu của gói tin</param>
        public MessageSend(sbyte command, byte[] buffer)
        {
            cmd = command;
            this.buffer = [.. buffer];
        }

        /// <summary>Viết dữ liệu dưới dạng <see langword="byte"/> của <paramref name="value"/> vào dữ liệu của gói tin</summary>
        public void WriteBool(bool value) => buffer.AddRange(BitConverter.GetBytes(value));

        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteByte(byte value) => buffer.Add(value);

        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteSByte(sbyte value) => buffer.Add((byte)value);

        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteShort(short value) => buffer.AddRange([(byte)(value >> 8), (byte)(value & 0xFF)]);

        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteUShort(ushort value) => buffer.AddRange([(byte)(value >> 8), (byte)(value & 0xFF)]);

        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteChar(char value) => buffer.AddRange([(byte)(value >> 8), (byte)(value & 0xFF)]);

        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteInt(int value) => buffer.AddRange([(byte)(value >> 24), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF)]);

        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteUInt(uint value) => buffer.AddRange([(byte)(value >> 24), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF)]);

        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteLong(long value) => buffer.AddRange([(byte)(value >> 56), (byte)((value >> 48) & 0xFF), (byte)((value >> 40) & 0xFF), (byte)((value >> 32) & 0xFF), (byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF)]);

        /// <inheritdoc cref="WriteBool(bool)"/>
        public void WriteULong(ulong value) => buffer.AddRange([(byte)(value >> 56), (byte)((value >> 48) & 0xFF), (byte)((value >> 40) & 0xFF), (byte)((value >> 32) & 0xFF), (byte)((value >> 24) & 0xFF), (byte)((value >> 16) & 0xFF), (byte)((value >> 8) & 0xFF), (byte)(value & 0xFF)]);

        /// <summary>
        /// Thêm <paramref name="value"/> vào dữ liệu của gói tin
        /// </summary>
        /// <param name="value"></param>
        public void WriteBytes(byte[] value) => buffer.AddRange(value);

        /// <summary>
        /// Viết dữ liệu ASCII dưới dạng <see langword="byte"/> của <paramref name="value"/> vào dữ liệu của gói tin
        /// </summary>
        /// <param name="value"></param>
        public void WriteStringASCII(string value)
        {
            char[] chars = value.ToCharArray();
            WriteUShort((ushort)chars.Length);
            foreach (char c in chars)
                WriteByte((byte)c);
        }

        /// <summary>
        /// Viết dữ liệu UTF-8 dưới dạng <see langword="byte"/> của <paramref name="value"/> vào dữ liệu của gói tin
        /// </summary>
        /// <param name="value"></param>
        public void WriteStringUTF8(string value)
        {
            byte[] array = Encoding.Convert(Encoding.Unicode, Encoding.GetEncoding(65001), Encoding.Unicode.GetBytes(value));
            WriteUShort((ushort)array.Length);
            WriteBytes(array);
        }

        public void Dispose() => buffer.Clear();
    }
}
