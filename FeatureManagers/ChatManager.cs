using System;
using System.Collections;
using System.Collections.Generic;
using TwitchLib.Api.Core.Models.Undocumented.Chatters;
using TwitchLib.Api.Helix.Models.Chat.GetChatters;
using TwitchLib.Client.Events;
using TwitchLib.Unity;
using UnityEngine;

namespace Twitchmata {
    public class ChatManager : FeatureManager {
        #region Client Handling
        internal override void InitializeClient(Client client) {
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

        #region Chatters
        public Dictionary<string, Models.User> Chatters { get; private set; } = new Dictionary<string, Models.User>() { };
        public virtual void ChatterJoined(Models.User chatter) {
            Debug.Log("Chatter joined: "+ chatter.DisplayName);
        }

        public virtual void ChatterLeft(Models.User chatter) {
            Debug.Log("Chatter left: " + chatter.DisplayName);
        }
        #endregion

        #region Moderator
        public Dictionary<string, Models.User> Moderators { get; private set; } = new Dictionary<string, Models.User>() { };
        public virtual void ModeratorJoined(Models.User moderator) {
            Debug.Log("Mod joined: " + moderator.DisplayName);
        }

        public virtual void ModeratorLeft(Models.User moderator) {
            Debug.Log("Mod left: " + moderator.DisplayName);
        }
        #endregion

        #region VIPs
        public Dictionary<string, Models.User> VIPs { get; private set; } = new Dictionary<string, Models.User>() { };
        public virtual void VIPJoined(Models.User vip) {
            Debug.Log("VIPs joined: " + vip.DisplayName);
        }

        public virtual void VIPLeft(Models.User vip) {
            Debug.Log("VIPs joined: " + vip.DisplayName);
        }
        #endregion
    }
}
