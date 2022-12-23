using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using TwitchLib.Unity;
using UnityEngine;

namespace Twitchmata {
    public class ChatCommandManager : FeatureManager {
        override internal void InitializeClient(Client client) {
            Debug.Log("Setting up Chat Command Manager");
            client.OnChatCommandReceived -= Client_OnChatCommandReceived;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs args) {
            var command = args.Command;
            if (this.RegisteredCommands.ContainsKey(command.CommandText) == false) {
                return;
            }

            var user = this.UserManager.UserForChatMessage(command.ChatMessage);

            var registeredCommand = this.RegisteredCommands[command.CommandText];
            if (this.IsUserPermitted(user, registeredCommand.Permissions) == false) {
                this.Manager.Client.SendRaw(":tmi.twitch.tv NOTICE #pilkycrc :Here is a notice");
                this.Manager.Client.SendMessage(this.Manager.ConnectionConfig.ChannelName, "You don't have permission to use this command");
                return;
            }

            registeredCommand.Callback.Invoke(command.ArgumentsAsList, user);
        }


        #region Permissions
        private bool IsUserPermitted(Models.User user, Permissions permissions) {
            if ((permissions & Permissions.Chatters) == Permissions.Chatters) {
                return true;
            }
            if (user.IsBroadcaster) {
                return false;
            }
            if ((permissions & Permissions.Mods) == Permissions.Mods && user.IsModerator) {
                return true;
            }
            if ((permissions & Permissions.VIPs) == Permissions.VIPs && user.IsVIP) {
                return true;
            }
            if ((permissions & Permissions.Subscribers) == Permissions.Subscribers && user.IsSubscriber) {
                return true;
            }
            return false;
        }
        #endregion



        #region Command Registration
        private Dictionary<string, RegisteredChatCommand> RegisteredCommands = new Dictionary<string, RegisteredChatCommand>();
        public void RegisterChatCommand(string command, Permissions permissions, ChatCommandCallback callback) {
            this.RegisteredCommands[command] = new RegisteredChatCommand() {
                Permissions = permissions,
                Callback = callback,
            };
        }
        #endregion
    }
}
