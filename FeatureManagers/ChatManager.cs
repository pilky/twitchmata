using System;
using System.Collections;
using System.Collections.Generic;
using TwitchLib.Api.Core;
using TwitchLib.Api.Core.Models.Undocumented.Chatters;
using TwitchLib.Api.Helix.Models.Chat;
using TwitchLib.Api.Helix.Models.Chat.GetChatters;
using TwitchLib.Client.Events;
using TwitchLib.Unity;
using UnityEngine;

namespace Twitchmata {
    /// <summary>
    /// Used to keep track of users in Chat
    /// </summary>
    public class ChatManager : FeatureManager {
        #region Chatters
        /// <summary>
        /// A list of users currently in chat (this is not a list of viewers)
        /// </summary>
        public Dictionary<string, Models.User> Chatters { get; private set; } = new Dictionary<string, Models.User>() { };

        /// <summary>
        /// Fired when a user sends their first message after joining the channel
        /// </summary>
        public virtual void ChatterJoined(Models.User chatter) {
            Logger.LogInfo("Chatter joined: "+ chatter.DisplayName);
        }

        /// <summary>
        /// Fired when a user leaves the chat
        /// </summary>
        /// <param name="chatter"></param>
        public virtual void ChatterLeft(Models.User chatter) {
            Logger.LogInfo("Chatter left: " + chatter.DisplayName);
        }
        #endregion

        #region Moderator
        /// <summary>
        /// A list of moderators currently viewing the stream
        /// </summary>
        public Dictionary<string, Models.User> Moderators { get; private set; } = new Dictionary<string, Models.User>() { };

        /// <summary>
        /// Fired when a moderator joins the stream
        /// </summary>
        public virtual void ModeratorJoined(Models.User moderator) {
            Logger.LogInfo("Mod joined: " + moderator.DisplayName);
        }

        /// <summary>
        /// Fired when a moderator leaves the stream
        /// </summary>
        public virtual void ModeratorLeft(Models.User moderator) {
            Logger.LogInfo("Mod left: " + moderator.DisplayName);
        }
        #endregion

        #region VIPs
        /// <summary>
        /// A list of VIPs currently viewing the stream
        /// </summary>
        public Dictionary<string, Models.User> VIPs { get; private set; } = new Dictionary<string, Models.User>() { };

        /// <summary>
        /// Fired when a VIP joins the stream
        /// </summary>
        public virtual void VIPJoined(Models.User vip) {
            Logger.LogInfo("VIP joined: " + vip.DisplayName);
        }

        /// <summary>
        /// Fired when a VIP leaves the stream
        /// </summary>
        public virtual void VIPLeft(Models.User vip) {
            Logger.LogInfo("VIP left: " + vip.DisplayName);
        }
        #endregion


        #region Sending Messages
        /// <summary>
        /// Send an announcement to the chat
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="colour">The announcement colour to use (default: null, i.e. the channel colour)</param>
        /// <param name="preferredAccount">The preferredAccount to post with (falls back to broadcaster if bot is not available)</param>
        public void SendAnnouncement(string message, AnnouncementColors colour = null, PosterAccount preferredAccount = PosterAccount.Broadcaster) {
            var account = this.Connection.ChannelID;
            var accessToken = this.Connection.Secrets.AccountAccessToken;
            if (preferredAccount == PosterAccount.Bot && this.Connection.BotID != null) {
                account = this.Connection.BotID;
                accessToken = this.Connection.Secrets.BotAccessToken;
            }

            var task = this.Connection.API.Helix.Chat.SendChatAnnouncementAsync(this.Connection.ChannelID, account, message, colour, accessToken);
            TwitchManager.RunTask(task, () => {
                Logger.LogInfo("Announcement sent");
            });
        }

        //Implement in TwitchLib
        //Add rate limit (2min per shoutout, 60m per streamer)
        //Sender (broadcaster or chat bot)
        //Type (native, text, auto [uses native but falls back to text if necessary])
        public void ShoutOut(string streamerName) {
            
        }
        #endregion



        /**************************************************
         * INTERNAL CODE. NO NEED TO READ BELOW THIS LINE *
         **************************************************/

        #region Internal
        internal override void InitializeClient(Client client) {
            Logger.LogInfo("Initializing Chat Manager");
            client.OnMessageReceived += Client_OnMessageReceived;
            client.OnUserJoined += Client_OnUserJoined;
            client.OnUserLeft += Client_OnUserLeft;
            client.OnModeratorJoined += Client_OnModeratorJoined;
            client.OnModeratorLeft += Client_OnModeratorLeft;
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e) {
            var user = this.UserManager.UserForChatMessage(e.ChatMessage);
            if (this.Chatters.ContainsKey(user.UserName) == false) {
                this.Chatters[user.UserName] = user;
                this.ChatterJoined(user);
            }
            
        }

        private void Client_OnUserJoined(object sender, OnUserJoinedArgs e) {
            var user = this.UserManager.UserWithUserName(e.Username);
            if (user != null && user.IsVIP && this.VIPs.ContainsKey(user.UserName) == false) {
                this.VIPs[user.UserName] = user;
                this.VIPJoined(user);
            }
        }

        private void Client_OnUserLeft(object sender, OnUserLeftArgs e) {
            if (this.Chatters.ContainsKey(e.Username)) {
                var user = this.Chatters[e.Username];
                this.Chatters.Remove(e.Username);
                this.ChatterLeft(user);
            }
            if (this.VIPs.ContainsKey(e.Username)) {
                var user = this.VIPs[e.Username];
                this.VIPs.Remove(e.Username);
                this.VIPLeft(user);
            }
        }

        private void Client_OnModeratorJoined(object sender, OnModeratorJoinedArgs e) {
            var user = this.UserManager.UserWithUserName(e.Username);
            if (user != null && this.Moderators.ContainsKey(user.UserName) == false) {
                this.Moderators[user.UserName] = user;
                this.ModeratorJoined(user);
            }
        }

        private void Client_OnModeratorLeft(object sender, OnModeratorLeftArgs e) {
            if (this.Moderators.ContainsKey(e.Username)) {
                var user = this.Chatters[e.Username];
                this.Moderators.Remove(e.Username);
                this.ModeratorLeft(user);
            }
        }
        #endregion
    }

    /// <summary>
    /// The account to use for Chat calls
    /// </summary>
    public enum PosterAccount {
        Broadcaster,
        Bot,
    }
}