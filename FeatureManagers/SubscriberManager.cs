using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses.Messages;
using System.Threading.Tasks;
using TwitchLib.Unity;

namespace Twitchmata {
    public class SubscriberManager : FeatureManager {
        override internal void InitializePubSub(PubSub pubSub) {
            pubSub.OnChannelSubscription -= PubSub_OnChannelSubscription;
            pubSub.OnChannelSubscription += PubSub_OnChannelSubscription;
            pubSub.ListenToSubscriptions(this.ChannelID);
        }

        override public void InitializeFeatureManager() {
            this.FetchSubscribers();
        }

        private void PubSub_OnChannelSubscription(object sender, OnChannelSubscriptionArgs arg) {
            var subscription = arg.Subscription;
            if (subscription.IsGift == true) {
                this.MarkAsSubscribed(subscription.RecipientId);
                this.GiftSubscriptionReceived(subscription);
            } else {
                this.SubscriptionReceived(subscription);
                this.MarkAsSubscribed(subscription.UserId);
            }
        }

        #region Notifications
        public virtual void SubscriptionReceived(ChannelSubscription sub) {
            Debug.Log($"User Subscribed {sub.Username}");
        }

        public virtual void GiftSubscriptionReceived(ChannelSubscription sub) {
            Debug.Log($"User received gift sub {sub.RecipientName}");
        }
        #endregion

        #region Subscriptions
        private List<string> Subscribers = new List<string> { };

        private void FetchSubscribers() {
            var task = Task.Run(() => GetSubscribers());
            try {
                task.Wait();
                this.Subscribers = task.Result;
            } catch {
                Debug.Log("Failed!");
            }
        }

        private async Task<List<string>> GetSubscribers() {
            var tasks = new List<string> { };
            var subscribers = await this.HelixAPI.Subscriptions.GetBroadcasterSubscriptionsAsync(this.ChannelID);

            foreach (var subscriber in subscribers.Data) {
                tasks.Add(subscriber.UserId);
            }
            return tasks;
        }

        //MARK: - API Helpers
        public bool CheckIfSubscribed(string userID) {
            return this.Subscribers.Contains(userID);
        }

        public void MarkAsSubscribed(string userID) {
            this.Subscribers.Add(userID);
        }

        #endregion
    }
}
