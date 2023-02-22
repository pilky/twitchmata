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
        #region Reward Registration
        /// <summary>
        /// List of managed rewards keyed by their ID
        /// </summary>
        public Dictionary<string, ManagedReward> ManagedRewardsByID { get; private set; } = new Dictionary<string, ManagedReward>() {};

        /// <summary>
        /// List of managed rewards keyed by their title
        /// </summary>
        public Dictionary<string, ManagedReward> ManagedRewardsByTitle { get; private set; } = new Dictionary<string, ManagedReward>() { };

        /// <summary>
        /// Register a reward to be managed by Twitchmata.
        /// </summary>
        /// <remarks>
        /// Managed rewards are created by Twitchmata and so can be updated. See ManagedReward for more detail
        /// </remarks>
        /// <param name="reward">The Managed Reward to set up</param>
        /// <param name="callback">The delegate method to call when the reward is successfully redeemed</param>
        public void RegisterReward(ManagedReward reward, RewardRedemptionCallback callback) {
            reward.Callback = callback;
            this.ManagedRewardsByTitle[reward.Title] = reward;
            if (reward.Id != null) {
                this.ManagedRewardsByID[reward.Id] = reward;
            }
        }

        /// <summary>
        /// Register an reward that is not managed by Twitchmata
        /// </summary>
        /// <remarks>
        /// Unmanaged Rewards are rewards created elsewhere (e.g. on Twitch's website) but which
        /// you wish to respond to. Twitchmata (and thus your overlay) is not able to perform any
        /// updates to these rewards, just respond to them.
        ///
        /// If you wish to have more control over a reward consider converting it to a Managed Reward
        /// </remarks>
        /// <param name="title">The title of the reward</param>
        /// <param name="callback">The delegate method to call when the reward is redeemed</param>
        public void RegisterUnmanagedReward(string title, RewardRedemptionCallback callback) {
            this.UnmanagedRewards[title] = callback;
        }
        #endregion

        #region Update Rewards
        /// <summary>
        /// Enables a ManagedReward if it was previously disabled
        /// </summary>
        /// <param name="reward">The reward to enable</param>
        public void EnableReward(ManagedReward reward) {
            if (reward.IsEnabled == true || reward.Id == null) {
                return;
            }
            var request = new UpdateCustomRewardRequest();
            request.IsEnabled = true;
            var task = this.HelixAPI.ChannelPoints.UpdateCustomRewardAsync(this.ChannelID, reward.Id, request);
            TwitchManager.RunTask(task, (response) => {
                reward.IsEnabled = true;
                Logger.LogInfo("Enabled reward '" + reward.Title + "'");
            });
        }

        /// <summary>
        /// Disables a ManagedReward if it was previously enabled
        /// </summary>
        /// <param name="reward">The reward to disable</param>
        public void DisableReward(ManagedReward reward) {
            if (reward.IsEnabled == false || reward.Id == null) {
                return;
            }
            var request = new UpdateCustomRewardRequest();
            request.IsEnabled = false;
            var task = this.HelixAPI.ChannelPoints.UpdateCustomRewardAsync(this.ChannelID, reward.Id, request);
            TwitchManager.RunTask(task, (response) => {
                reward.IsEnabled = false;
                Logger.LogInfo("Disabled reward '" + reward.Title + "'");
            });
        }

        /// <summary>
        /// Updates the cost of a reward
        /// </summary>
        /// <param name="reward">The reward to update the cost of</param>
        /// <param name="newCost">The new cost for the reward</param>
        public void UpdateRewardCost(ManagedReward reward, int newCost) {
            if (reward.Cost == newCost || reward.Id == null) {
                return;
            }
            var request = new UpdateCustomRewardRequest();
            request.Cost = newCost;
            var task = this.HelixAPI.ChannelPoints.UpdateCustomRewardAsync(this.ChannelID, reward.Id, request);
            TwitchManager.RunTask(task, (response) => {
                reward.Cost = newCost;
                Logger.LogInfo("Updated cost of reward '" + reward.Title + "' to " + newCost);
            });
        }
        #endregion



        /**************************************************
         * INTERNAL CODE. NO NEED TO READ BELOW THIS LINE *
         **************************************************/

        #region Internal
        override internal void InitializePubSub(PubSub pubSub) {
            Logger.LogInfo("Setting up Channel Points");
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

            if (this.ManagedRewardsByID.ContainsKey(apiRedemption.Reward.Id) == false) {
                return;
            }

            var reward = this.ManagedRewardsByID[apiRedemption.Reward.Id];

            if (redemption.User.IsPermitted(reward.Permissions) == false) {
                Logger.LogInfo("User not permitted");
                this.UpdateRedemptionStatus(apiRedemption, CustomRewardRedemptionStatus.CANCELED);
                return;
            }

            if (reward.RequiresUserInput && reward.ValidInputs.Count > 0 && reward.ValidInputs.Contains(redemption.UserInput.ToLower()) == false) {
                Logger.LogInfo("Invalid input entered: " + redemption.UserInput);
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
        #endregion

        #region Reward Management
        private Dictionary<string, RewardRedemptionCallback> UnmanagedRewards = new Dictionary<string, RewardRedemptionCallback>() { };

        private void CreateReward(ManagedReward reward) {
            var request = reward.CreateRewardRequest();
            var task = this.HelixAPI.ChannelPoints.CreateCustomRewardsAsync(this.ChannelID, request);
            TwitchManager.RunTask(task, (response) => {
                var id = response.Data[0].Id;
                reward.Id = id;
                this.ManagedRewardsByID[id] = reward;
                Logger.LogInfo("Created reward '" + reward.Title + "'");
            }, (error) => {
                Logger.LogError("Could not create managed reward. Make sure a reward with this name doesn't already exist.\nIf you wish to convert an existing reward to a managed reward you must first delete the reward in Twitch's dashboard.");
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
                Logger.LogInfo("Updated custom reward: " + response);
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
        
        private void UpdateRedemptionStatus(Redemption redemption, CustomRewardRedemptionStatus newStatus) {
            var statusRequest = new UpdateCustomRewardRedemptionStatusRequest() { Status = newStatus };
            var task = this.HelixAPI.ChannelPoints.UpdateRedemptionStatusAsync(this.ChannelID, redemption.Reward.Id, new List<string>() { redemption.Id}, statusRequest);
            TwitchManager.RunTask(task, (obj) => {
                Logger.LogInfo("Updated to status: "+ obj.Data[0].Status);
            });
        }
        #endregion
    }
}

