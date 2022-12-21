using System;
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
            var user = this.UserManager.UserForRaidNotification(args.RaidNotification);
            var raid = new Models.IncomingRaid() {
                Raider = user,
                ViewerCount = Int32.Parse(args.RaidNotification.MsgParamViewerCount),
            };
            this.RaidsThisStream.Add(raid);
            this.RaidReceived(raid);
        }


        #region Notifications
        public virtual void RaidReceived(Models.IncomingRaid raid) {
            Debug.Log($"{raid.Raider.DisplayName} raided with {raid.ViewerCount} viewers");
        }
        #endregion

        #region Stats
        public List<Models.IncomingRaid> RaidsThisStream { get; private set; } = new List<Models.IncomingRaid>() { };
        #endregion
    }
}
