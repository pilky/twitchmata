using System.Collections;
using System.Collections.Generic;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Unity;
using UnityEngine;

namespace Twitchmata {
    public class ChatCommandManager : FeatureManager {
        override public void InitializeClient(Client client) {
            Debug.Log("Setting up Chat Command Manager");
            client.OnChatCommandReceived -= Client_OnChatCommandReceived;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs args) {
            this.ReceivedCommand(args.Command);
        }

        public virtual void ReceivedCommand(ChatCommand command) {
            Debug.Log("Chat command received: " + command.CommandText);
        }
    }
}