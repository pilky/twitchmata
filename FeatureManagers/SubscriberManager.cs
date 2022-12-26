using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses.Messages;
using System.Threading.Tasks;
using TwitchLib.Unity;
using Twitchmata;
using Twitchmata.Models;

namespace Twitchmata {
    /// <summary>
    /// Used to hook into Subscriber events in your overlay
    /// </summary>
    /// <remarks>
    /// To utilise SubscriberManager create a subclass and add to a GameObject (either the
    /// GameObject holding TwitchManager or a child GameObject).
    ///
    /// Then override <code>UserSubscribed()</code> and add your sub-handling code.
    /// </remarks>
    public class SubscriberManager : FeatureManager {
        #region Notifications
        /// <summary>
        /// Fired when a user subscribes or is gifted a sub.
        /// </summary>
        /// <param name="subscriber"></param>
        public virtual void UserSubscribed(Models.User subscriber) {
            if (subscriber.Subscription.IsGift == true) {
                Logger.LogInfo($"{subscriber.DisplayName} received gift sub from {subscriber.Subscription.Gifter.DisplayName}");
            } else {
                Logger.LogInfo($"{subscriber.DisplayName} subscribed");
            }
        }
        #endregion

        #region Stats
        /// <summary>
        /// List of users who subscribed or received a gift sub while the overlay has been open
        /// </summary>
        public List<Models.User> SubscribersThisStream { get; private set; } = new List<Models.User>() { };
        #endregion



        /**************************************************
         * INTERNAL CODE. NO NEED TO READ BELOW THIS LINE *
         **************************************************/

        #region Internal
        override internal void InitializePubSub(PubSub pubSub) {
            Logger.LogInfo("Setting up Subscriber Manager");
            pubSub.OnChannelSubscription -= PubSub_OnChannelSubscription;
            pubSub.OnChannelSubscription += PubSub_OnChannelSubscription;
            pubSub.ListenToSubscriptions(this.ChannelID);
        }

        private void PubSub_OnChannelSubscription(object sender, OnChannelSubscriptionArgs arg) {
            var user = this.UserManager.UserForSubscriptionNotification(arg.Subscription);
            this.SubscribersThisStream.Add(user);
            this.UserSubscribed(user);
        }
        #endregion
    }
}
