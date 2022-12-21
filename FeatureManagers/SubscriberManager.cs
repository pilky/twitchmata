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
    public class SubscriberManager : FeatureManager {
        override internal void InitializePubSub(PubSub pubSub) {
            pubSub.OnChannelSubscription -= PubSub_OnChannelSubscription;
            pubSub.OnChannelSubscription += PubSub_OnChannelSubscription;
            pubSub.ListenToSubscriptions(this.ChannelID);
        }

        private void PubSub_OnChannelSubscription(object sender, OnChannelSubscriptionArgs arg) {
            var user = this.UserManager.UserForSubscriptionNotification(arg.Subscription);
            this.SubscribersThisStream.Add(user);
            if (arg.Subscription.IsGift == true) {
                this.UserGiftedSubscription(user);
            } else {
                this.UserSubscribed(user);
            }
        }

        #region Notifications
        public virtual void UserSubscribed(Models.User subscriber) {
            Debug.Log($"{subscriber.DisplayName} subscribed");
        }

        public virtual void UserGiftedSubscription(Models.User subscriber) {
            Debug.Log($"{subscriber.DisplayName} received gift sub from {subscriber.Subscription.Gifter.DisplayName}");
        }
        #endregion

        #region Stats
        public List<Models.User> SubscribersThisStream { get; private set; } = new List<Models.User>() { };
        #endregion
    }
}
