using System;
using Starksoft.Net.Proxy;

namespace DataNRO.Interfaces
{
    /// <summary>
    /// Interface đại diện cho một phiên kết nối tới máy chủ.
    /// </summary>
    public interface ISession : IDisposable
    {
        /// <summary>
        /// Đối tượng nhận và đọc tin nhắn từ máy chủ
        /// </summary>
        IMessageReceiver MessageReceiver { get; }

        /// <summary>
        /// Đối tượng gửi tin nhắn tới máy chủ
        /// </summary>
        IMessageWriter MessageWriter { get; }

        /// <summary>
        /// Địa chỉ máy chủ
        /// </summary>
        string Host { get; }

        /// <summary>
        /// Cổng kết nối của máy chủ
        /// </summary>
        ushort Port { get; }

        /// <summary>
        /// Trạng thái kết nối thành công tới máy chủ của phiên hiện tại
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Kết nối tới máy chủ
        /// </summary>
        void Connect();

        /// <summary>
        /// Kết nối tới máy chủ sử dụng proxy
        /// </summary>
        /// <param name="proxyHost">Địa chỉ proxy</param>
        /// <param name="proxyPort">Cổng proxy</param>
        /// <param name="proxyUsername">Tên đăng nhập proxy</param>
        /// <param name="proxyPassword">Mật khẩu proxy</param>
        /// <param name="proxyType">Loại proxy</param>
        void Connect(string proxyHost, ushort proxyPort, string proxyUsername, string proxyPassword, ProxyType proxyType);

        /// <summary>
        /// Gửi gói tin tới máy chủ
        /// </summary>
        /// <param name="message">Gói tin cần gửi</param>
        void SendMessage(MessageSend message);

        /// <summary>
        /// Ngắt kết nối khỏi máy chủ
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Dữ liệu Game
        /// </summary>
        GameData Data { get; }

        /// <summary>
        /// Nhân vật của phiên hiện tại
        /// </summary>
        Player Player { get; }

        /// <summary>
        /// Đối tượng quản lý việc ghi dữ liệu vào file
        /// </summary>
        FileWriter FileWriter { get; }
    }
}
