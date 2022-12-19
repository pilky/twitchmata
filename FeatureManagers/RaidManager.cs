using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using System.Threading.Tasks;

namespace Twitchmata {
    public class RaidManager : FeatureManager {
        override internal void InitializeClient(Client client) {
            Debug.Log("Setting Up Raid Notifications");
            client.OnRaidNotification -= Client_OnRaidNotification;
            client.OnRaidNotification += Client_OnRaidNotification;
        }

        private void Client_OnRaidNotification(object sender, OnRaidNotificationArgs args) {
            this.IncomingRaid(args.RaidNotification);
        }


        #region Notifications
        public virtual void IncomingRaid(RaidNotification raidNotification) {
            Debug.Log($"Raid received from {raidNotification.DisplayName}");
        }
        #endregion
    }
}
