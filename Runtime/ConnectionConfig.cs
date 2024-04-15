using System;
using UnityEngine;

namespace Twitchmata {
    /// <summary>
    /// Holds the current public configuration info
    /// </summary>
    [Serializable]
    public struct ConnectionConfig {
        /// <summary>
        /// The clientID of your app, found after registering your app on Twitch's website.
        /// </summary>
        [Tooltip("The clientID of your app, found after registering your app on Twitch's website.")]
        public string ClientID;

        /// <summary>
        /// The name of the channel you're broadcasting from
        /// </summary>
        [Tooltip("The name of the channel you're broadcasting from")]
        public string ChannelName;

        /// <summary>
        /// The name of the bot to use when communicating with chat
        /// </summary>
        /// <remarks>
        /// This can be the same as your broadcasting account, but it's recommended to be a separate account.
        /// </remarks>
        [Tooltip("The name of the bot to use when communicating with chat")]
        public string BotName;
        
        /// <summary>
        /// Post a Message in the Channel after the connection was successful
        /// </summary>
        [Tooltip("Post a Message in the Channel after the connection was successful")]
        public bool PostConnectMessage;

        /// <summary>
        /// The Message that gets send on successful connection
        /// </summary>
        ///
        [Tooltip("The Message that gets posted in the Channel after the connection was successful")]
        public string ConnectMessage;
    }
}

