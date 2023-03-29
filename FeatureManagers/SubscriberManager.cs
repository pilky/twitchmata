using System.Collections.Generic;
using TwitchLib.PubSub.Events;
using TwitchLib.Unity;
using Twitchmata.Models;
using System;
using TwitchLib.Api.Core.Extensions.System;

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
                Logger.LogInfo($"{subscriber.DisplayName} received gift sub from {subscriber.Subscription.Gifter?.DisplayName ?? "an anonymous gifter"}");
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

        /// <summary>
        /// List of all users who gifted a sub this stream
        /// </summary>
        public List<Models.User> GiftersThisStream { get; private set; } = new List<Models.User>() { };
        #endregion


        #region Debug

        /// <summary>
        /// Simulates a subscription event
        /// </summary>
        /// <param name="displayName">The display name of the subscriber</param>
        /// <param name="userName">The username of the subscriber</param>
        /// <param name="userID">The user ID of the subscriber</param>
        /// <param name="plan">The subscription plan</param>
        /// <param name="planName">The name of the subscription plan</param>
        /// <param name="cumulativeMonths">The total number of months subscribed</param>
        /// <param name="streakMonths">The number of months in the current subscription streak</param>
        /// <param name="isResub">True if the user is re-subscribing, false if they're subscribing for the first time</param>
        /// <param name="message">The message sent with the subscription</param>
        public void Debug_NewSubscription(
            string displayName = "TWW2",
            string userName = "tww2",
            string userID = "13405587",
            SubscriptionTier plan = SubscriptionTier.Tier1,
            string planName = "Channel Subscription",
            int cumulativeMonths = 1,
            int streakMonths = 1,
            bool isResub = false,
            string message = "I just subscribed!"
            ) {
            this.Connection.PubSub_SendTestMessage("channel-subscribe-events-v1.44322889", new {
                user_name = userName,
                display_name = displayName,
                user_id = userID,
                channel_name = this.Connection.ConnectionConfig.ChannelName,
                channel_id = this.Connection.ChannelID,
                time = DateTime.Now.ToRfc3339String(),
                sub_plan = Subscription.StringForTier(plan),
                sub_plan_name = planName,
                cumulative_months = cumulativeMonths,
                streak_months = streakMonths,
                context = isResub ? "resub" : "sub",
                is_gift = false,
                sub_message = new {
                    message = message,
                    emotes = new List<System.Object>() { }
                }
            });
        }

        /// <summary>
        /// Simulates a gift subscription
        /// </summary>
        /// <param name="gifterDisplayName">The display name of the user gifting the sub</param>
        /// <param name="gifterUserName">The username of the user gifting the sub</param>
        /// <param name="gifterUserID">The user ID of the user gifting the sub</param>
        /// <param name="recipientDisplayName">The display name of the user receiving the sub</param>
        /// <param name="recipientUserName">The username of the user receiving the sub</param>
        /// <param name="recipientUserID">The user ID of the user receiving the sub</param>
        /// <param name="plan">The subscription plan</param>
        /// <param name="planName">The name of the subscription plan</param>
        /// <param name="months">The number of months gifted</param>
        /// <param name="message">The message associated with the subscription</param>
        public void Debug_NewGiftSubscription(
            string gifterDisplayName = "TWW2",
            string gifterUserName = "tww2",
            string gifterUserID = "13405587",
            string recipientDisplayName = "ForstyCup",
            string recipientUserName = "forstycup",
            string recipientUserID = "19571752",
            SubscriptionTier plan = SubscriptionTier.Tier1,
            string planName = "Channel Subscription",
            int months = 1,
            string message = "I just gifted a sub!"
            ) {
            this.Connection.PubSub_SendTestMessage("channel-subscribe-events-v1.44322889", new {
                user_name = gifterUserName,
                display_name = gifterDisplayName,
                user_id = gifterUserID,
                channel_name = this.Connection.ConnectionConfig.ChannelName,
                channel_id = this.Connection.ChannelID,
                time = DateTime.Now.ToRfc3339String(),
                sub_plan = Subscription.StringForTier(plan),
                sub_plan_name = planName,
                months = months,
                context = "subgift",
                is_gift = true,
                sub_message = new {
                    message = message,
                    emotes = new List<System.Object>() { }
                },
                recipient_id = recipientUserID,
                recipientUserName = recipientUserName,
                recipientDisplayName = recipientDisplayName
            });
        }
        
        /// <summary>
        /// Simulates an anonymous gift subscription
        /// </summary>
        /// <param name="recipientDisplayName">The display name of the user receiving the sub</param>
        /// <param name="recipientUserName">The username of the user receiving the sub</param>
        /// <param name="recipientUserID">The user ID of the user receiving the sub</param>
        /// <param name="plan">The subscription plan</param>
        /// <param name="planName">The name of the subscription plan</param>
        /// <param name="months">The number of months gifted</param>
        /// <param name="message">The message associated with the subscription</param>
        public void Debug_NewAnonymousGiftSubscription(
            string recipientDisplayName = "ForstyCup",
            string recipientUserName = "forstycup",
            string recipientUserID = "19571752",
            SubscriptionTier plan = SubscriptionTier.Tier1,
            string planName = "Channel Subscription",
            int months = 1,
            string message = "I just gifted a sub!"
            ) {
            this.Connection.PubSub_SendTestMessage("channel-subscribe-events-v1.44322889", new {
                channel_name = this.Connection.ConnectionConfig.ChannelName,
                channel_id = this.Connection.ChannelID,
                time = DateTime.Now.ToRfc3339String(),
                sub_plan = Subscription.StringForTier(plan),
                sub_plan_name = planName,
                months = months,
                context = "anonsubgift",
                is_gift = true,
                sub_message = new {
                    message = message,
                    emotes = new List<System.Object>() { }
                },
                recipient_id = recipientUserID,
                recipientUserName = recipientUserName,
                recipientDisplayName = recipientDisplayName
            });
        }

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
            if (user.Subscription?.IsGift == true) {
                var gifter = user.Subscription.Gifter;
                if (this.GiftersThisStream.Contains(gifter) == false) {
                    this.GiftersThisStream.Add(gifter);
                }
            }
            this.UserSubscribed(user);
        }
        #endregion
    }
}
