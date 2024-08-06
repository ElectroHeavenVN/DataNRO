namespace DataNRO.Interfaces
{
    /// <summary>
    /// Interface đại diện cho các class nhận và đọc gói tin từ máy chủ.
    /// </summary>
    public interface IMessageReceiver
    {
        /// <summary>
        /// Hàm xử lý dữ liệu khi nhận được gói tin từ máy chủ
        /// </summary>
        /// <param name="message">Gói tin nhận được từ máy chủ</param>
        void OnMessageReceived(MessageReceive message);
    }
}
