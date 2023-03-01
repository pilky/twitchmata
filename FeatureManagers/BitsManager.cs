using System;
using System.Collections;
using System.Collections.Generic;
using TwitchLib.Api.Core.Extensions.System;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.Unity;
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
            Logger.LogInfo($"Received {bitsRedemption.BitsUsed} Bits from {bitsRedemption.User?.DisplayName}");
        }
        #endregion


        #region Stats
        /// <summary>
        /// List of bit redemptions that have occurred while the overlay has been open
        /// </summary>
        public List<Models.BitsRedemption> RedemptionsThisStream { get; private set; } = new List<Models.BitsRedemption>() {};
        #endregion


        #region Debug
        /// <summary>
        /// Trigger a bits event from a named user
        /// </summary>
        /// <param name="bitsUsed">The number of bits to send</param>
        /// <param name="userName">The name of the user</param>
        /// <param name="userID">The id of the user</param>
        /// <param name="chatMessage">The message the user sent with the bits</param>
        public void Debug_SendBits(int bitsUsed = 100, string userName = "jwp", string userID = "95546976", string chatMessage = "Have some test bits") {
            this.Connection.PubSub_SendTestMessage("channel-bits-events-v2.46024993", new {
                data = new {
                    user_name = userName,
                    user_id = userID,
                    channel_name = this.Connection.ConnectionConfig.ChannelName,
                    channel_id = this.Connection.ChannelID,
                    is_anonymous = false,
                    time = DateTime.Now.ToRfc3339String(),
                    bits_used = bitsUsed,
                    chat_message = chatMessage,
                    context = "cheer",
                    message_id = Guid.NewGuid().ToString(),
                    message_type = "bits_event",
                    version = "1.0"
                }
            });
        }

        /// <summary>
        /// Trigger a bits event from an anonymous user
        /// </summary>
        /// <param name="bitsUsed">The number of bits to send</param>
        /// <param name="chatMessage">The message the user sent with the bits</param>
        public void Debug_SendAnonymousBits(int bitsUsed = 100, string chatMessage = "Have some test bits") {
            this.Connection.PubSub_SendTestMessage("channel-bits-events-v2.46024993", new {
                data = new {
                    channel_name = this.Connection.ConnectionConfig.ChannelName,
                    channel_id = this.Connection.ChannelID,
                    is_anonymous = true,
                    time = DateTime.Now.ToRfc3339String(),
                    bits_used = bitsUsed,
                    chat_message = chatMessage,
                    context = "cheer",
                    message_id = Guid.NewGuid().ToString(),
                    message_type = "bits_event",
                    version = "1.0"
                }
            });
        }

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