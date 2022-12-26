using System.Collections;
using System.Collections.Generic;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.Unity;
using UnityEditor.PackageManager;
using UnityEngine;

namespace Twitchmata {
    /// <summary>
    /// Used to hook into bits events in your overlay
    /// </summary>
    /// <remarks>
    /// To utilise BitsManager create a subclass and add to a GameObject (either the
    /// GameObject holding TwitchManager or a child GameObject).
    ///
    /// Then override <code>ReceivedBits()</code> and add your bit-handling code.
    /// </remarks>
    public class BitsManager : FeatureManager {
        #region Notifications
        /// <summary>
        /// Fired when a user sends the broadcaster bits. 
        /// </summary>
        /// <param name="bitsInfo">Info on the bits received</param>
        public virtual void ReceivedBits(Models.BitsRedemption bitsRedemption) {
            Logger.LogInfo($"Received {bitsRedemption.BitsUsed} Bits from {bitsRedemption.User.DisplayName}");
        }
        #endregion


        #region Stats
        /// <summary>
        /// List of bit redemptions that have occurred while the overlay has been open
        /// </summary>
        public List<Models.BitsRedemption> RedemptionsThisStream { get; private set; } = new List<Models.BitsRedemption>() {};
        #endregion



        /**************************************************
         * INTERNAL CODE. NO NEED TO READ BELOW THIS LINE *
         **************************************************/

        #region Internal
        override internal void InitializePubSub(PubSub pubSub) {
            Logger.LogInfo("Initialising BitsManager");
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
        #endregion

    }
}