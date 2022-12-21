using System.Collections;
using System.Collections.Generic;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.Unity;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Twitchmata {
    public class BitsManager : FeatureManager {
        override internal void InitializePubSub(PubSub pubSub) {
            pubSub.OnBitsReceivedV2 -= OnBitsReceived;
            pubSub.OnBitsReceivedV2 += OnBitsReceived;
            pubSub.ListenToBitsEventsV2(this.ChannelID);
        }

        private void OnBitsReceived(object sender, OnBitsReceivedV2Args bitsInfo) {
            Models.User user = null;
            if ((bitsInfo.IsAnonymous == false) && (bitsInfo.UserId != null)) {
                user = this.UserManager.UserForBitsRedeem(bitsInfo);
            }

            var redemption = new Models.BitsRedemption() {
                BitsUsed = bitsInfo.BitsUsed,
                TotalBitsUsed = bitsInfo.TotalBitsUsed,
                User = user,
                RedeemedAt = bitsInfo.Time,
                Message = bitsInfo.ChatMessage,
            };

            this.RedemptionsThisStream.Add(redemption);
            this.ReceivedBits(redemption);
        }

        #region Notifications

        /// <summary>
        /// Fired when a user gives you bits. 
        /// </summary>
        /// <param name="bitsInfo">Info on the bits received</param>
        public virtual void ReceivedBits(Models.BitsRedemption bitsRedemption) {
            Debug.Log($"Received {bitsRedemption.BitsUsed} Bits from {bitsRedemption.User.DisplayName}");
        }

        #endregion


        #region Stats
        public List<Models.BitsRedemption> RedemptionsThisStream { get; private set; } = new List<Models.BitsRedemption>() {};
        #endregion
    }
}