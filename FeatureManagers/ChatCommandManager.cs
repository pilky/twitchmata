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
            if (user.IsPermitted(registeredCommand.Permissions) == false) {
                this.Manager.Client.SendRaw(":tmi.twitch.tv NOTICE #pilkycrc :Here is a notice");
                this.Manager.Client.SendMessage(this.Manager.ConnectionConfig.ChannelName, "You don't have permission to use this command");
                return;
            }

            registeredCommand.Callback.Invoke(command.ArgumentsAsList, user);
        }

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
