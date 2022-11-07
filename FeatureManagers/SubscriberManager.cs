using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses.Messages;
using System.Threading.Tasks;
using TwitchLib.Unity;

namespace Twitchmata {

    public class SubscriberManager : FeatureManager {
        override public void InitializePubSub(PubSub pubSub) {
            pubSub.OnChannelSubscription -= PubSub_OnChannelSubscription;
            pubSub.OnChannelSubscription += PubSub_OnChannelSubscription;
            pubSub.ListenToSubscriptions(Config.channelID);
        }

        private void PubSub_OnChannelSubscription(object sender, OnChannelSubscriptionArgs arg) {
            var subscription = arg.Subscription;
            if (subscription.IsGift == true) {
                this.manager.MarkAsSubscribed(subscription.RecipientId);
                this.SubscriptionReceived(subscription);
            } else {
                this.manager.MarkAsSubscribed(subscription.UserId);
                this.GiftSubscriptionReceived(subscription);
            }
        }

        public virtual void SubscriptionReceived(ChannelSubscription sub) {
            Debug.Log($"User Subscribed {sub.Username}");
        }

        public virtual void GiftSubscriptionReceived(ChannelSubscription sub) {
            Debug.Log($"User received gift sub {sub.RecipientName}");
        }
    }
}
