using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using System.Threading.Tasks;

namespace Twitchmata {
    public class RaidManager : FeatureManager {
        //TODO
        //1. Response to raid notifications
        //2. Create and display raid message
        //3. Add incoming message alert to comms screen
        //4. On interaction, show raider's avatar and name and cancel alert
        //5. FINALLY do texturing and lighting and makesure lights turn red on raid
        override public void InitializeClient(Client client) {
            Debug.Log("Setting Up Raid Notifications");
            client.OnRaidNotification -= Client_OnRaidNotification;
            client.OnRaidNotification += Client_OnRaidNotification;
        }

        private void Client_OnRaidNotification(object sender, OnRaidNotificationArgs args) {
            this.RaidStarted(args.RaidNotification);
        }

        public virtual void RaidStarted(RaidNotification raidNotification) {
            Debug.Log($"Raid received from {raidNotification.DisplayName}");
        }   
    }
}
