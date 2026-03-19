using System.Collections.ObjectModel;
using System.Text;

namespace EHVN.DragonBoyOnline.CustomMsgHandler;

public abstract class BaseMessage
{
    protected byte command;

    public BaseMessage(byte cmd)
    {
        command = cmd;
    }

    public byte Command => command;

    public abstract int Length { get; }
}

public class MessageSend : BaseMessage
{
    List<byte> data;

    public MessageSend(byte cmd) : base(cmd)
    {
        data = [];
    }

    public override int Length => data.Count;
    public ReadOnlyCollection<byte> Data => data.AsReadOnly();

    public void WriteBool(bool value) => data.Add((byte)(value ? 1 : 0));

    public void WriteInt8(sbyte value) => data.Add((byte)value);
    public void WriteUInt8(byte value) => data.Add(value);

    public void WriteInt16(short value)
    {
        data.Add((byte)((value >> 8) & 0xFF));
        data.Add((byte)(value & 0xFF));
    }
    public void WriteUInt16(ushort value)
    {
        data.Add((byte)((value >> 8) & 0xFF));
        data.Add((byte)(value & 0xFF));
    }

    public void WriteInt32(int value)
    {
        for (int i = 3; i >= 0; --i)
            data.Add((byte)((value >> (i * 8)) & 0xFF));
    }
    public void WriteUInt32(uint value)
    {
        for (int i = 3; i >= 0; --i)
            data.Add((byte)((value >> (i * 8)) & 0xFF));
    }

    public void WriteInt64(long value)
    {
        for (int i = 7; i >= 0; --i)
            data.Add((byte)((value >> (i * 8)) & 0xFF));
    }
    public void WriteUInt64(ulong value)
    {
        for (int i = 7; i >= 0; --i)
            data.Add((byte)((value >> (i * 8)) & 0xFF));
    }

    public void WriteRawBytes(byte[] bytes, int length) => data.AddRange(bytes.Take(length));
    public void WriteRawBytes(byte[] bytes) => WriteRawBytes(bytes, bytes.Length);

    public void WriteBytes(byte[] bytes, uint length)
    {
        WriteUInt32(length);
        data.AddRange(bytes.Take((int)length));
    }
    public void WriteBytes(byte[] bytes) => WriteBytes(bytes, (uint)bytes.Length);

    public void WriteString(char[] str, ushort length)
    {
        WriteUInt16(length);
        for (ushort i = 0; i < length; ++i)
            data.Add((byte)str[i]);
    }
    public void WriteString(string str) => WriteString(str, (ushort)str.Length);
    public void WriteString(string str, ushort length) => WriteString(str.ToCharArray(), length);
}

public abstract class MessageRecv : BaseMessage
{
    public abstract int Available { get; }
    public abstract int Position { get; }

    public MessageRecv(byte cmd) : base(cmd) { }

    public bool ReadBool() => ReadUInt8() != 0;
    public sbyte ReadInt8() => (sbyte)ReadUInt8();
    public short ReadInt16() => (short)ReadUInt16();
    public int ReadInt32() => (int)ReadUInt32();
    public long ReadInt64() => (long)ReadUInt64();
    public byte[] ReadBytes() => ReadRawBytes((int)ReadUInt32());
    public sbyte[] ReadSBytes() => ReadRawSBytes((int)ReadUInt32());

    public abstract byte ReadUInt8();
    public abstract ushort ReadUInt16();
    public abstract uint ReadUInt32();
    public abstract ulong ReadUInt64();
    public abstract byte[] ReadRawBytes(int length);
    public abstract sbyte[] ReadRawSBytes(int length);
    public abstract string ReadString();
    public abstract int ReadToBuffer(ref sbyte[] buffer);
    public abstract void ReadToBufferOffset(ref sbyte[] buffer, int offset, int count);
    public abstract void ReadToBufferFull(ref sbyte[] buffer);
}

public class BufferedMessageRecv : MessageRecv
{
    byte[] data;
    int pos;
    int posMark;

    public BufferedMessageRecv(byte cmd, byte[] inputData) : base(cmd)
    {
        data = inputData;
        pos = 0;
    }

    public BufferedMessageRecv(byte cmd, sbyte[] inputData) : base(cmd)
    {
        data = new byte[inputData.Length];
        Array.Copy(inputData, data, inputData.Length);
        pos = 0;
    }

    public override int Position => pos;
    public override int Length => data.Length;
    public override int Available => data.Length - pos;

    public void Rewind() => pos = 0;
    public bool Seek(int position)
    {
        if (position > data.Length)
            return false;
        pos = position;
        return true;
    }

    public override byte ReadUInt8()
    {
        if (!CheckSize(1))
            return 0;
        return data[pos++];
    }

    public override ushort ReadUInt16()
    {
        if (!CheckSize(2))
            return 0;
        ushort value = (ushort)((data[pos] << 8) | data[pos + 1]);
        pos += 2;
        return value;
    }

    public override uint ReadUInt32()
    {
        if (!CheckSize(4))
            return 0;
        uint value = 0;
        for (int i = 0; i < 4; ++i)
            value = (value << 8) | data[pos + i];
        pos += 4;
        return value;
    }

    public override ulong ReadUInt64()
    {
        if (!CheckSize(8))
            return 0;
        ulong value = 0;
        for (int i = 0; i < 8; ++i)
            value = (value << 8) | data[pos + i];
        pos += 8;
        return value;
    }

    public override byte[] ReadRawBytes(int length)
    {
        if (!CheckSize(length))
            return [];
        byte[] bytes = new byte[length];
        Array.Copy(data, pos, bytes, 0, length);
        pos += length;
        return bytes;
    }

    public override sbyte[] ReadRawSBytes(int length)
    {
        if (!CheckSize(length))
            return [];
        sbyte[] bytes = new sbyte[length];
        Array.Copy(data, pos, bytes, 0, length);
        pos += length;
        return bytes;
    }

    public override string ReadString()
    {
        ushort length = ReadUInt16();
        if (!CheckSize(length))
            return string.Empty;
        byte[] bytes = new byte[length];
        for (ushort i = 0; i < length; ++i)
            bytes[i] = data[pos + i];
        pos += length;
        return Encoding.UTF8.GetString(bytes);
    }

    public override int ReadToBuffer(ref sbyte[] buffer)
    {
        if (buffer == null)
            return 0;
        int bytesRead = 0;
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (sbyte)ReadUInt8();
            if (Position >= Length)
                return -1;
            bytesRead++;
        }
        return bytesRead;
    }

    public override void ReadToBufferOffset(ref sbyte[] buffer, int offset, int count)
    {
        if (buffer == null)
            return;
        for (int i = 0; i < count; i++)
        {
            buffer[i + offset] = (sbyte)ReadUInt8();
            if (Position >= Length)
                return;
        }
    }

    public override void ReadToBufferFull(ref sbyte[] buffer)
    {
        if (buffer == null || buffer.Length + Position > Length)
            return;
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = (sbyte)ReadUInt8();
    }

    public void MarkPosition() => posMark = pos;

    public void ResetToMarkedPosition() => pos = posMark;

    bool CheckSize(int size)
    {
        bool result = pos + size <= data.Length;
        if (!result)
            throw new InvalidOperationException($"Not enough data to read. Required: {size}, Available: {data.Length - pos}");
        return result;
    }
}

public class StreamedMessageRecv : MessageRecv
{
    Stream stream;
    long length;
    Func<byte, byte> applyEncryptionFunc;
    long position;

    public StreamedMessageRecv(byte cmd, Stream stream, long length, Func<byte, byte> applyEncryptionFunc) : base(cmd)
    {
        this.stream = stream;
        this.length = length;
        this.applyEncryptionFunc = applyEncryptionFunc;
        position = 0;
    }

    public override int Position => (int)position;
    public override int Length => (int)length;
    public override int Available => (int)(length - position);

    public override byte ReadUInt8() => CheckSize(1) ? ReadNext() : (byte)0;

    public override ushort ReadUInt16()
    {
        if (!CheckSize(2))
            return 0;
        byte high = ReadNext();
        byte low = ReadNext();
        return (ushort)((high << 8) | low);
    }

    public override uint ReadUInt32()
    {
        if (!CheckSize(4))
            return 0;
        uint value = 0;
        for (int i = 0; i < 4; ++i)
            value = (value << 8) | ReadNext();
        return value;
    }

    public override ulong ReadUInt64()
    {
        if (!CheckSize(8))
            return 0;
        ulong value = 0;
        for (int i = 0; i < 8; ++i)
            value = (value << 8) | ReadNext();
        return value;
    }

    public override byte[] ReadRawBytes(int length)
    {
        if (!CheckSize(length))
            return [];
        byte[] bytes = new byte[length];
        for (int i = 0; i < length; ++i)
            bytes[i] = ReadNext();
        return bytes;
    }

    public override sbyte[] ReadRawSBytes(int length)
    {
        if (!CheckSize(length))
            return [];
        sbyte[] bytes = new sbyte[length];
        for (int i = 0; i < length; ++i)
            bytes[i] = (sbyte)ReadNext();
        return bytes;
    }

    public override string ReadString()
    {
        ushort length = ReadUInt16();
        if (!CheckSize(length))
            return string.Empty;
        byte[] bytes = new byte[length];
        for (ushort i = 0; i < length; ++i)
            bytes[i] = ReadNext();
        return Encoding.UTF8.GetString(bytes);
    }

    public override int ReadToBuffer(ref sbyte[] buffer)
    {
        if (buffer == null)
            return 0;
        int bytesRead = 0;
        for (int i = 0; i < buffer.Length; i++)
        {
            buffer[i] = (sbyte)ReadUInt8();
            if (Position >= Length)
                return -1;
            bytesRead++;
        }
        return bytesRead;
    }

    public override void ReadToBufferOffset(ref sbyte[] buffer, int offset, int count)
    {
        if (buffer == null)
            return;
        for (int i = 0; i < count; i++)
        {
            buffer[i + offset] = (sbyte)ReadUInt8();
            if (Position >= Length)
                return;
        }
    }

    public override void ReadToBufferFull(ref sbyte[] buffer)
    {
        if (buffer == null || buffer.Length + Position > Length)
            return;
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = (sbyte)ReadUInt8();
    }

    public BufferedMessageRecv ToBufferedMessage()
    {
        long len = length - position;
        byte[] data = new byte[len];
        for (int i = 0; i < len; ++i)
            data[i] = ReadNext();
        return new BufferedMessageRecv(command, data);
    }

    bool CheckSize(int size)
    {
        bool result = position + size <= length;
        if (!result)
            throw new InvalidOperationException($"Not enough data to read. Required: {size}, Available: {length - position}");
        return result;
    }

    byte ReadNext()
    {
        if (position >= length)
            return 0;
        int data = stream.ReadByte();
        if (data == -1)
            return 0;
        position++;
        byte result = (byte)data;
        if (applyEncryptionFunc != null)
            result = applyEncryptionFunc(result);
        return result;
    }
}