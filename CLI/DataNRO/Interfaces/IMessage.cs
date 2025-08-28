using System;

namespace EHVN.DataNRO.Interfaces
{
    /// <summary>
    /// Interface đại diện cho các gói tin gửi đi và nhận được từ máy chủ.
    /// </summary>
    public interface IMessage : IDisposable
    {
        /// <summary>
        /// Lệnh của gói tin
        /// </summary>
        sbyte Command { get; }
        /// <summary>
        /// Dữ liệu của gói tin
        /// </summary>
        byte[] Buffer { get; }
        /// <summary>
        /// Độ dài của dữ liệu
        /// </summary>
        long DataLength { get; }
        /// <summary>
        /// Vị trí hiện tại của con trỏ trong dữ liệu
        /// </summary>
        long CurrentPosition { get; }
    }
}
