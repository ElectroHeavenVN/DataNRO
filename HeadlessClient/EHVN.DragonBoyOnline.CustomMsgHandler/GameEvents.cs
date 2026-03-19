using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Text;

namespace EHVN.DragonBoyOnline.CustomMsgHandler
{
    /// <summary>
    /// Chứa các sự kiện lắng nghe từ game.
    /// </summary>
    public class GameEvents
    {
        /// <summary>
        /// Command 0xE3 (-29)
        /// </summary>
        public event Action<string>? IPAddressListReceived;

        /// <summary>
        /// Command 0xE6 (-26)
        /// </summary>
        public event Action<string>? DialogMessageReceived;

        /// <summary>
        /// Command 0xE7 (-25)
        /// </summary>
        public event Action<string>? ServerMessageReceived;

        /// <summary>
        /// Command 0x5E (94)
        /// </summary>
        public event Action<string>? ServerAlertReceived;

        /// <summary>
        /// Command 0x5C (92), name empty
        /// </summary>
        public event Action<string>? GameNotificationReceived;

        /// <summary>
        /// Command 0x5C (92), isChatServer true
        /// </summary>
        public event Action<string, string>? ServerChatReceived;

        /// <summary>
        /// Command 0x5C (92), isChatServer false
        /// </summary>
        public event Action<string, string>? PrivateChatReceived;

        /// <summary>
        /// Command 0x5D (93)
        /// </summary>
        public event Action<string>? ServerNotificationReceived;

        /// <summary>
        /// Command 0x23 (35)
        /// </summary>
        public event Action<string>? UnknownMessageReceived;

        internal void OnIPAddressListReceived(string ipList) => IPAddressListReceived?.Invoke(ipList);
        internal void OnDialogMessageReceived(string message) => DialogMessageReceived?.Invoke(message);
        internal void OnServerMessageReceived(string message) => ServerMessageReceived?.Invoke(message);
        internal void OnServerAlertReceived(string alert) => ServerAlertReceived?.Invoke(alert);
        internal void OnGameNotificationReceived(string notification) => GameNotificationReceived?.Invoke(notification);
        internal void OnServerChatReceived(string sender, string message) => ServerChatReceived?.Invoke(sender, message);
        internal void OnPrivateChatReceived(string sender, string message) => PrivateChatReceived?.Invoke(sender, message);
        internal void OnServerNotificationReceived(string notification) => ServerNotificationReceived?.Invoke(notification);
        internal void OnUnknownMessageReceived(string message) => UnknownMessageReceived?.Invoke(message);
    }
}
