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
    /// <summary>
    /// Used to respond to chat commands in your overlay
    /// </summary>
    /// <remarks>
    /// To utilise ChatCommandManager create a subclass and add to a GameObject (either the)
    /// Game object holding TwitchManager or a child GameObject).
    ///
    /// See <code>RegisterChatCommand()</code> for details on how to set up commands
    /// </remarks>
    public class ChatCommandManager : FeatureManager {
        #region Command Registration
        /// <summary>
        /// Register a chat command that viewers can invoke
        /// </summary>
        /// <param name="command">The name of the command (excluding any command prefix such as !)</param>
        /// <param name="permissions">Permissions for who can invoke the command</param>
        /// <param name="callback">The delegate method to call when this command is invoked</param>
        public void RegisterChatCommand(string command, Permissions permissions, ChatCommandCallback callback) {
            this.RegisteredCommands[command] = new RegisteredChatCommand() {
                Permissions = permissions,
                Callback = callback,
            };
        }
        #endregion



        /**************************************************
         * INTERNAL CODE. NO NEED TO READ BELOW THIS LINE *
         **************************************************/

        #region Internal
        private Dictionary<string, RegisteredChatCommand> RegisteredCommands = new Dictionary<string, RegisteredChatCommand>();

        override internal void InitializeClient(Client client) {
            Logger.LogInfo("Setting up Chat Command Manager");
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
                this.Manager.Client.SendMessage(this.Manager.ConnectionConfig.ChannelName, "You don't have permission to use this command");
                return;
            }

            registeredCommand.Callback.Invoke(command.ArgumentsAsList, user);
        }
        #endregion

    }
}
