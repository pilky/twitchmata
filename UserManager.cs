using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.Channels.GetChannelVIPs;
using TwitchLib.Api.Helix.Models.Moderation.GetModerators;
using TwitchLib.Api.Helix.Models.Subscriptions;
using TwitchLib.Client.Models;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses.Messages;
using Twitchmata.Models;
using UnityEngine;

namespace Twitchmata {
    class UserManager {
        internal ConnectionManager ConnectionManager;
        internal UserManager(ConnectionManager connectionManager){
            this.ConnectionManager = connectionManager;
        }

        private Dictionary<string, Models.User> UsersByID { get; set; } = new Dictionary<string, Models.User> {};

        public Models.User? UserWithID(string userID) {
            if (UsersByID.ContainsKey(userID)) { 
                return UsersByID[userID];
            }
            return null;
        }

        public Models.User? UserWithUserName(string userName) {
            foreach (var user in UsersByID.Values) {
                if (user.UserName == userName) {
                    return user;
                }
            }
            return null;
        }

        private Models.User ExistingOrNewUser(string userID, string userName, string displayName) {
            var user = this.UserWithID(userID);
            if (user == null) {
                user = new Models.User(userID, userName, displayName);
                UsersByID[user.UserId] = user;
            }
            return user;
        }

        internal Models.User? UserForBitsRedeem(OnBitsReceivedV2Args bitsRedeem) {
            if (bitsRedeem.IsAnonymous) {
                return null;
            }
            return this.ExistingOrNewUser(bitsRedeem.UserId, bitsRedeem.UserName, bitsRedeem.UserName);
        }

        internal Models.User UserForRaidNotification(RaidNotification raidNotification) {
            var user = this.ExistingOrNewUser(raidNotification.UserId, raidNotification.Login, raidNotification.DisplayName);
            user.IsModerator = raidNotification.Moderator;
            user.IsSubscriber = raidNotification.Subscriber;
            return user;
        }

        internal Models.User UserForFollowNotification(OnFollowArgs followNotification) {
            return this.ExistingOrNewUser(followNotification.UserId, followNotification.Username, followNotification.DisplayName);
        }

        internal Models.User UserForSubscriptionNotification(ChannelSubscription subscriptionNotification) {
            var subscription = new Models.Subscription();
            subscription.SubscribedMonthCount = subscriptionNotification.CumulativeMonths ?? 1;
            subscription.StreakMonths = subscriptionNotification.StreakMonths ?? 1;
            subscription.Tier = (SubscriptionTier)subscriptionNotification.SubscriptionPlan;
            subscription.PlanName = subscriptionNotification.SubscriptionPlanName;


            Models.User subscriber = null;
            if (subscriptionNotification.IsGift == true) {
                subscription.IsGift = true;
                subscription.Gifter = this.ExistingOrNewUser(subscriptionNotification.UserId, subscriptionNotification.Username, subscriptionNotification.DisplayName);
                subscriber = this.ExistingOrNewUser(subscriptionNotification.RecipientId, subscriptionNotification.RecipientName, subscriptionNotification.RecipientDisplayName);
            } else {
                subscription.IsGift = false;
                subscriber = this.ExistingOrNewUser(subscriptionNotification.UserId, subscriptionNotification.Username, subscriptionNotification.DisplayName);
            }

            subscriber.IsSubscriber = true;
            subscriber.Subscription = subscription;

            return subscriber;
        }

        internal Models.User UserForChannelPointsRedeem(RewardRedemption rewardRedemption) {
            return this.ExistingOrNewUser(rewardRedemption.UserId, rewardRedemption.UserLogin, rewardRedemption.UserName); ;
        }

        internal Models.User UserForChatMessage(ChatMessage chatMessage) {
            var user = this.ExistingOrNewUser(chatMessage.UserId, chatMessage.Username, chatMessage.DisplayName);
            user.IsBroadcaster = chatMessage.IsBroadcaster;
            user.IsSubscriber = chatMessage.IsSubscriber;
            user.IsVIP = chatMessage.IsVip;
            user.IsModerator = chatMessage.IsModerator;
            if (user.Subscription != null) {
                user.Subscription.SubscribedMonthCount = chatMessage.SubscribedMonthCount;
            }
            return user;
        }

        internal Models.User UserForAPISubscription(TwitchLib.Api.Helix.Models.Subscriptions.Subscription subscriber) {
            var user = this.ExistingOrNewUser(subscriber.UserId, subscriber.UserLogin, subscriber.UserName);

            user.IsSubscriber = true;

            var sub = new Models.Subscription();
            sub.Tier = Models.Subscription.TierForString(subscriber.Tier);
            sub.PlanName = subscriber.PlanName;
            sub.IsGift = subscriber.IsGift;

            var gifter = ExistingOrNewUser(subscriber.GiftertId, subscriber.GifterLogin, subscriber.GifterName);

            user.Subscription = sub;

            return user;
        }

        internal Models.User UserForAPIVIP(ChannelVIPsResponseModel vip) {
            var user = this.ExistingOrNewUser(vip.UserId, vip.UserLogin, vip.UserName);

            user.IsVIP = true;
            
            return user;
        }

        internal Models.User UserForAPIModerator(Moderator moderator) {
            var user = this.ExistingOrNewUser(moderator.UserId, moderator.UserLogin, moderator.UserName);

            user.IsModerator = true;
            
            return user;
        }


        internal void FetchUserInfo() {
            this.FetchNextSubscribers();
            this.FetchNextVIPs();
            this.FetchNextModerators();
        }

        private string ChannelID {
            get { return this.ConnectionManager.ConnectionConfig.ChannelID; }
        }

        private static int FetchSize = 100;

        private void FetchNextSubscribers(string pagination = null) {
            var task = this.ConnectionManager.API.Helix.Subscriptions.GetBroadcasterSubscriptionsAsync(this.ChannelID, UserManager.FetchSize, pagination);
            this.ConnectionManager.API.Invoke(task, obj => {
                var subscribers = obj.Data;
                if (subscribers.Length == UserManager.FetchSize && obj.Pagination != null) {
                    this.FetchNextSubscribers(obj.Pagination.Cursor);
                }

                foreach (var subscriber in subscribers) {
                    this.UserForAPISubscription(subscriber);
                }
            });
        }

        private void FetchNextVIPs(string pagination = null) {
            var task = this.ConnectionManager.API.Helix.Channels.GetVIPsAsync(this.ChannelID, null, UserManager.FetchSize, pagination);
            this.ConnectionManager.API.Invoke(task, obj => {
                var vips = obj.Data;
                if (vips.Length == UserManager.FetchSize && obj.Pagination != null) {
                    this.FetchNextVIPs(obj.Pagination.Cursor);
                }

                foreach (var vip in vips) {
                    this.UserForAPIVIP(vip);
                }
            });
        }

        private void FetchNextModerators(string pagination = null)
        {
            var task = this.ConnectionManager.API.Helix.Moderation.GetModeratorsAsync(this.ChannelID, null, UserManager.FetchSize, pagination);
            this.ConnectionManager.API.Invoke(task, obj => {
                var moderators = obj.Data;
                if (moderators.Length == UserManager.FetchSize && obj.Pagination != null) {
                    this.FetchNextModerators(obj.Pagination.Cursor);
                }

                foreach (var moderator in moderators) {
                    this.UserForAPIModerator(moderator);
                }
            });
        }
    }
}

