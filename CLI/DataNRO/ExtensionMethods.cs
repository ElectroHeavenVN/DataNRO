using System;
using System.IO;
using System.Linq;

namespace DataNRO
{
    /// <summary>
    /// Class chứa các phương thức mở rộng.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Đảo ngược mảng byte
        /// </summary>
        /// <param name="b">Mảng byte cần đảo ngược</param>
        /// <returns>Mảng byte đã đảo ngược</returns>
        public static byte[] Reverse(this byte[] b) => b.Reverse<byte>().ToArray();

        /// <summary>
        /// Đọc giá trị UInt16 từ BinaryReader theo định dạng Big-Endian
        /// </summary>
        /// <param name="binRdr">BinaryReader để đọc dữ liệu</param>
        /// <returns>Giá trị UInt16</returns>
        public static ushort ReadUInt16BE(this BinaryReader binRdr) => BitConverter.ToUInt16(binRdr.ReadBytesRequired(sizeof(ushort)).Reverse(), 0);

        /// <summary>
        /// Đọc giá trị Int16 từ BinaryReader theo định dạng Big-Endian
        /// </summary>
        /// <param name="binRdr">BinaryReader để đọc dữ liệu</param>
        /// <returns>Giá trị Int16</returns>
        public static short ReadInt16BE(this BinaryReader binRdr) => BitConverter.ToInt16(binRdr.ReadBytesRequired(sizeof(short)).Reverse(), 0);

        /// <summary>
        /// Đọc giá trị UInt32 từ BinaryReader theo định dạng Big-Endian
        /// </summary>
        /// <param name="binRdr">BinaryReader để đọc dữ liệu</param>
        /// <returns>Giá trị UInt32</returns>
        public static uint ReadUInt32BE(this BinaryReader binRdr) => BitConverter.ToUInt32(binRdr.ReadBytesRequired(sizeof(uint)).Reverse(), 0);

        /// <summary>
        /// Đọc giá trị Int32 từ BinaryReader theo định dạng Big-Endian
        /// </summary>
        /// <param name="binRdr">BinaryReader để đọc dữ liệu</param>
        /// <returns>Giá trị Int32</returns>
        public static int ReadInt32BE(this BinaryReader binRdr) => BitConverter.ToInt32(binRdr.ReadBytesRequired(sizeof(int)).Reverse(), 0);

        /// <summary>
        /// Đọc giá trị UInt64 từ BinaryReader theo định dạng Big-Endian
        /// </summary>
        /// <param name="binRdr">BinaryReader để đọc dữ liệu</param>
        /// <returns>Giá trị UInt64</returns>
        public static ulong ReadUInt64BE(this BinaryReader binRdr) => BitConverter.ToUInt64(binRdr.ReadBytesRequired(sizeof(ulong)).Reverse(), 0);

        /// <summary>
        /// Đọc giá trị Int64 từ BinaryReader theo định dạng Big-Endian
        /// </summary>
        /// <param name="binRdr">BinaryReader để đọc dữ liệu</param>
        /// <returns>Giá trị Int64</returns>
        public static long ReadInt64BE(this BinaryReader binRdr) => BitConverter.ToInt64(binRdr.ReadBytesRequired(sizeof(long)).Reverse(), 0);

        internal static byte[] ReadBytesRequired(this BinaryReader reader, int byteCount)
        {
            var result = reader.ReadBytes(byteCount);

            if (result.Length != byteCount)
                throw new EndOfStreamException(string.Format("{0} byte cần thiết từ luồng, nhưng chỉ trả về {1} byte.", byteCount, result.Length));

            return result;
        }
    }
}
