namespace DataNRO.Interfaces
{
    /// <summary>
    /// Interface đại diện cho các class tạo và gửi gói tin tới máy chủ.
    /// </summary>
    public interface IMessageWriter
    {
        /// <summary>
        /// Gửi yêu cầu cập nhật thông tin map
        /// </summary>
        void UpdateMap();
        /// <summary>
        /// Gửi yêu cầu cập nhật thông tin item
        /// </summary>
        void UpdateItem();
        /// <summary>
        /// Gửi yêu cầu cập nhật thông tin skill
        /// </summary>
        void UpdateSkill();
        /// <summary>
        /// Gửi yêu cầu cập nhật thông tin dữ liệu chung
        /// </summary>
        void UpdateData();
        /// <summary>
        /// Gửi thông báo ứng dụng khách đã sẵn sàng
        /// </summary>
        void ClientOk();
        /// <summary>
        /// Gửi thông tin về ứng dụng khách (thông tin phiên bản, kích thước màn hình, ...)
        /// </summary>
        void SetClientType();
        /// <summary>
        /// Gửi yêu cầu dữ liệu ảnh
        /// </summary>
        void ImageSource();
        /// <summary>
        /// Gửi thông tin đăng nhập
        /// </summary>
        /// <param name="username">Tài khoản hoặc tên người dùng</param>
        /// <param name="pass">Mật khẩu</param>
        /// <param name="type">Loại đăng nhập, 0 là đăng nhập bằng tài khoản và mật khẩu, 1 là đăng nhập bằng tên người dùng</param>
        void Login(string username, string pass, sbyte type);
        /// <summary>
        /// Gửi thông báo đã cập nhật xong
        /// </summary>
        void FinishUpdate();
        /// <summary>
        /// Gửi thông báo đã tải map xong
        /// </summary>
        void FinishLoadMap();
        /// <summary>
        /// Gửi lệnh chat
        /// </summary>
        /// <param name="text">Nội dung</param>
        void Chat(string text);
        /// <summary>
        /// Gửi lệnh di chuyển nhân vật
        /// </summary>
        /// <param name="x">Toạ độ X</param>
        /// <param name="y">Toạ độ Y</param>
        void CharMove(int x, int y);
        /// <summary>
        /// Gửi lệnh chuyển map
        /// </summary>
        void RequestChangeMap();
        /// <summary>
        /// Gửi lệnh chuyển map khi map hiện tại hoặc map tiếp theo là map offline
        /// </summary>
        void GetMapOffline();
    }
}
