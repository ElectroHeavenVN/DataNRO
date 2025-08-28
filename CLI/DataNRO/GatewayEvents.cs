using System;

namespace EHVN.DataNRO
{
    /// <summary>
    /// Chứa các sự kiện lắng nghe từ game.
    /// </summary>
    public class GatewayEvents
    {
        /// <summary>
        /// Command -29
        /// </summary>
        public event Action<string>? IPAddressListReceived;

        /// <summary>
        /// Command 026
        /// </summary>
        public event Action<string>? DialogMessageReceived;

        /// <summary>
        /// Command -25
        /// </summary>
        public event Action<string>? ServerMessageReceived;

        /// <summary>
        /// Command 94
        /// </summary>
        public event Action<string>? ServerAlertReceived;

        /// <summary>
        /// Command 92, name empty
        /// </summary>
        public event Action<string>? GameNotificationReceived;

        /// <summary>
        /// Command 92, isChatServer true
        /// </summary>
        public event Action<string, string>? ServerChatReceived;

        /// <summary>
        /// Command 92, isChatServer false
        /// </summary>
        public event Action<string, string>? PrivateChatReceived;

        /// <summary>
        /// Command 93
        /// </summary>
        public event Action<string>? ServerNotificationReceived;

        /// <summary>
        /// Command 35
        /// </summary>
        public event Action<string>? UnknownMessageReceived;

        public GatewayEvents() { }

        public void OnIPAddressListReceived(string ipList) => IPAddressListReceived?.Invoke(ipList);

        public void OnDialogMessageReceived(string message) => DialogMessageReceived?.Invoke(message);

        public void OnServerMessageReceived(string message) => ServerMessageReceived?.Invoke(message);

        public void OnServerAlertReceived(string alert) => ServerAlertReceived?.Invoke(alert);

        public void OnGameNotificationReceived(string notification) => GameNotificationReceived?.Invoke(notification);

        public void OnServerChatReceived(string sender, string message) => ServerChatReceived?.Invoke(sender, message);

        public void OnPrivateChatReceived(string sender, string message) => PrivateChatReceived?.Invoke(sender, message);

        public void OnServerNotificationReceived(string notification) => ServerNotificationReceived?.Invoke(notification);

        public void OnUnknownMessageReceived(string message) => UnknownMessageReceived?.Invoke(message);
    }
}
