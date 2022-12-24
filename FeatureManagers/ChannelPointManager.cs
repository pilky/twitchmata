using System.Collections;
using System.Collections.Generic;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses.Messages.Redemption;
using TwitchLib.Unity;
using UnityEngine;
using System.Threading;
using UnityEngine.Events;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;
using Twitchmata.Models;
using TwitchLib.Api.Helix.Models.ChannelPoints.GetCustomReward;
using System;
using System.Linq;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomReward;

namespace Twitchmata {
    public class ChannelPointManager : FeatureManager {
        override internal void InitializePubSub(PubSub pubSub) {
            Debug.Log("Setting up Channel Points");
            pubSub.OnChannelPointsRewardRedeemed -= PubSub_OnChannelPointsRewardRedeemed;
            pubSub.OnChannelPointsRewardRedeemed += PubSub_OnChannelPointsRewardRedeemed;
            pubSub.ListenToChannelPoints(this.ChannelID);
        }

        internal override void FinalizeInitialization() {
            base.FinalizeInitialization();
            this.FetchRemoteManagedRewards();
        }

        private void PubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e) {
            var apiRedemption = e.RewardRedeemed.Redemption;
            var redemption = this.RedemptionFromAPIRedemption(apiRedemption);
            if (this.UnmanagedRewards.ContainsKey(apiRedemption.Reward.Title)) {
                this.UnmanagedRewards[apiRedemption.Reward.Title](redemption);
                return;
            }

            Debug.Log("Managed reward found: " + apiRedemption.Reward.Title);
            if (this.ManagedRewardsByID.ContainsKey(apiRedemption.Reward.Id) == false) {
                return;
            }

            var reward = this.ManagedRewardsByID[apiRedemption.Reward.Id];

            if (redemption.User.IsPermitted(reward.Permissions) == false) {
                Debug.Log("User not permitted");
                this.UpdateRedemptionStatus(apiRedemption, CustomRewardRedemptionStatus.CANCELED);
                return;
            }

            //Just make extra sure we don't redeem
            if (apiRedemption.Status == "CANCELED") {
                return;
            }

            if (apiRedemption.Status == "UNFULFILLED") { 
                this.UpdateRedemptionStatus(apiRedemption, CustomRewardRedemptionStatus.FULFILLED);
            }
            reward.Callback(redemption);
        }

        private ChannelPointRedemption RedemptionFromAPIRedemption(Redemption apiRedemption) {
            return new ChannelPointRedemption() {
                RedeemedAt = apiRedemption.RedeemedAt,
                UserInput = apiRedemption.UserInput,
                User = this.UserManager.UserForChannelPointsRedeem(apiRedemption)
            };
        }

        #region Reward Registration
        public Dictionary<string, ManagedReward> ManagedRewardsByID { get; private set; } = new Dictionary<string, ManagedReward>() {};
        public Dictionary<string, ManagedReward> ManagedRewardsByTitle { get; private set; } = new Dictionary<string, ManagedReward>() { };
        public void RegisterReward(ManagedReward reward, RewardRedemptionCallback callback) {
            reward.Callback = callback;
            this.ManagedRewardsByTitle[reward.Title] = reward;
            if (reward.Id != null) {
                this.ManagedRewardsByID[reward.Id] = reward;
            }
        }

        private Dictionary<string, RewardRedemptionCallback> UnmanagedRewards = new Dictionary<string, RewardRedemptionCallback>() { };
        public void RegisterUnmanagedReward(string title, RewardRedemptionCallback callback) {
            this.UnmanagedRewards[title] = callback;
        }
        #endregion

        #region Reward Management
        private void CreateReward(ManagedReward reward) {
            var request = reward.CreateRewardRequest();
            var task = this.HelixAPI.ChannelPoints.CreateCustomRewardsAsync(this.ChannelID, request);
            TwitchManager.RunTask(task, (response) => {
                var id = response.Data[0].Id;
                reward.Id = id;
                this.ManagedRewardsByID[id] = reward;
            });
        }

        private void CheckRewardForUpdates(ManagedReward localReward, CustomReward remoteReward) {
            var remoteManagedReward = new ManagedReward(remoteReward);
            var updateRequest = localReward.UpdateRequestForDifferencesFrom(remoteManagedReward);
            if (updateRequest == null) {
                return;
            }

            var task = this.HelixAPI.ChannelPoints.UpdateCustomRewardAsync(this.ChannelID, localReward.Id, updateRequest);
            TwitchManager.RunTask(task, (response) => {
                Debug.Log("Updated custom reward: " + response);
            });
        }

        private void FetchRemoteManagedRewards() {
            if (this.ManagedRewardsByTitle.Count == 0) {
                return;
            }
            var task = this.HelixAPI.ChannelPoints.GetCustomRewardAsync(this.ChannelID, null, true);
            TwitchManager.RunTask(task, UpdateManagedRewards);
        }

        private void UpdateManagedRewards(GetCustomRewardsResponse obj) {
            var rewardsNeedingCreation = this.ManagedRewardsByTitle.Keys.ToList();
            foreach (var reward in obj.Data) {
                if (this.ManagedRewardsByTitle.ContainsKey(reward.Title) == false) {
                    continue;
                }
                var managedReward = this.ManagedRewardsByTitle[reward.Title];
                managedReward.Id = reward.Id;
                this.ManagedRewardsByID[reward.Id] = managedReward;
                rewardsNeedingCreation.Remove(reward.Title);

                this.CheckRewardForUpdates(managedReward, reward);
            }

            foreach (var title in rewardsNeedingCreation) {
                this.CreateReward(this.ManagedRewardsByTitle[title]);
            }
        }
        #endregion

        #region Update Rewards
        private void UpdateRedemptionStatus(Redemption redemption, CustomRewardRedemptionStatus newStatus) {
            var statusRequest = new UpdateCustomRewardRedemptionStatusRequest() { Status = newStatus };
            var task = this.HelixAPI.ChannelPoints.UpdateRedemptionStatusAsync(this.ChannelID, redemption.Reward.Id, new List<string>() { redemption.Id}, statusRequest);
            TwitchManager.RunTask(task, (obj) => {
                Debug.Log("Updated to status: "+ obj.Data[0].Status);
            });
        }

        public void EnableReward(ManagedReward reward) {
            if (reward.IsEnabled == true || reward.Id == null) {
                return;
            }
            var request = new UpdateCustomRewardRequest();
            request.IsEnabled = true;
            var task = this.HelixAPI.ChannelPoints.UpdateCustomRewardAsync(this.ChannelID, reward.Id, request);
            TwitchManager.RunTask(task, (response) => {
                reward.IsEnabled = true;
            });
        }

        public void DisableReward(ManagedReward reward) {
            if (reward.IsEnabled == false || reward.Id == null) {
                return;
            }
            var request = new UpdateCustomRewardRequest();
            request.IsEnabled = false;
            var task = this.HelixAPI.ChannelPoints.UpdateCustomRewardAsync(this.ChannelID, reward.Id, request);
            TwitchManager.RunTask(task, (response) => {
                reward.IsEnabled = false;
            });
        }

        public void UpdateRewardCost(ManagedReward reward, int newCost) {
            if (reward.Cost == newCost || reward.Id == null) {
                return;
            }
            var request = new UpdateCustomRewardRequest();
            request.Cost = newCost;
            var task = this.HelixAPI.ChannelPoints.UpdateCustomRewardAsync(this.ChannelID, reward.Id, request);
            TwitchManager.RunTask(task, (response) => {
                reward.Cost = newCost;
            });
        }

        #endregion
    }
}

