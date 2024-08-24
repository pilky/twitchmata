using System.Collections.Generic;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses.Messages.Redemption;
using TwitchLib.Unity;
using Twitchmata.Models;
using TwitchLib.Api.Helix.Models.ChannelPoints.GetCustomReward;
using System.Linq;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomReward;
using UnityEngine;

namespace Twitchmata {
    public class ChannelPointManager : FeatureManager {
        #region Managed Rewards
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
        /// Convenience for adding a managed reward
        /// </summary>
        public ManagedReward RegisterReward(string title, string description, int cost, RewardRedemptionCallback callback, ManagedRewardGroup group) {
            return this.RegisterReward(title, description, cost, callback, Permissions.Everyone, true, group);
        }
        
        /// <summary>
        /// Convenience for adding a managed reward
        /// </summary>
        public ManagedReward RegisterReward(string title, int cost, RewardRedemptionCallback callback, ManagedRewardGroup group) {
            return this.RegisterReward(title, "", cost, callback, Permissions.Everyone, true, group);
        }

        /// <summary>
        /// Convenience for adding a managed reward
        /// </summary>
        public ManagedReward RegisterReward(string title, string description, int cost, RewardRedemptionCallback callback, bool enabled) {
            return this.RegisterReward(title, description, cost, callback, Permissions.Everyone, enabled);
        }
        
        /// <summary>
        /// Convenience for adding a managed reward
        /// </summary>
        public ManagedReward RegisterReward(string title, int cost, RewardRedemptionCallback callback, bool enabled) {
            return this.RegisterReward(title, "", cost, callback, Permissions.Everyone, enabled);
        }

        /// <summary>
        /// Convenience for adding a managed reward
        /// </summary>
        /// <param name="title">The reward title (must be unique)</param>
        /// <param name="description">the reward Description. Max 200 Characters</param>
        /// <param name="cost">The channel point cost of the reward</param>
        /// <param name="callback">A method to call when the reward is invoked</param>
        /// <param name="permissions">The permissions of the reward (defaults to Everyone)</param>
        /// <param name="enabled">The enabled state of the reward (defaults to true)</param>
        /// <param name="group">The group to place the reward in (defaults to null)</param>
        /// <returns>A newly created and registered reward</returns>
        public ManagedReward RegisterReward(string title, string description, int cost, RewardRedemptionCallback callback, Permissions permissions = Permissions.Everyone, bool enabled = true, ManagedRewardGroup group = null) {
            var reward = new ManagedReward(title, description, cost, permissions, enabled);
            if (group != null) {
                group.AddReward(reward);
            }
            this.RegisterReward(reward, callback);
            return reward;
        }
        #endregion

        #region Unmanaged Rewards
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
            this.UnmanagedRewardRedemptionsThisStream[title] = new List<ChannelPointRedemption>();
        }

        /// <summary>
        /// Fulfilled Redemptions for any registered unmanaged rewards. The dictionary is keyed by reward title
        /// </summary>
        public Dictionary<string, List<ChannelPointRedemption>> UnmanagedRewardRedemptionsThisStream { get; private set; } = new Dictionary<string, List<ChannelPointRedemption>>();
        #endregion

        #region Reward Groups
        /// <summary>
        /// Enables all rewards in the supplied group
        /// </summary>
        /// <param name="group">The group to enable rewards in</param>
        public void EnableGroup(ManagedRewardGroup group) {
            foreach (var reward in group.Rewards) {
                this.EnableReward(reward);
            }
        }

        /// <summary>
        /// Disables all rewards in the supplied group
        /// </summary>
        /// <param name="group">The group to disable rewards in</param>
        public void DisableGroup(ManagedRewardGroup group) {
            foreach (var reward in group.Rewards) {
                this.DisableReward(reward);
            }
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


        #region Complete Redemptions
        /// <summary>
        /// Fulfill the supplied redemption (will fail if not unfulfilled)
        /// </summary>
        /// <param name="redemption">The redemption to fulfill</param>
        public void FulfillRedemption(ChannelPointRedemption redemption) {
            this.UpdateRedemptionStatus(redemption, CustomRewardRedemptionStatus.FULFILLED);
        }

        /// <summary>
        /// Cancel the supplied redemption, refunding channel points to the user (will fail if not unfulfilled)
        /// </summary>
        /// <param name="redemption">The redemption to cancel</param>
        public void CancelRedemption(ChannelPointRedemption redemption) {
            this.UpdateRedemptionStatus(redemption, CustomRewardRedemptionStatus.CANCELED);
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
            //This is deprecated but is the only way to get notified of status updates with PubSub
            pubSub.OnRewardRedeemed -= PubSub_RewardUpdated;
            pubSub.OnRewardRedeemed += PubSub_RewardUpdated;
            pubSub.ListenToChannelPoints(this.ChannelID);
            pubSub.ListenToRewards(this.ChannelID);
        }

        internal override void PerformPostDiscoverySetup() {
            base.PerformPostDiscoverySetup();
            this.FetchRemoteManagedRewards();
        }

        private void PubSub_OnChannelPointsRewardRedeemed(object sender, OnChannelPointsRewardRedeemedArgs e) {
            var apiRedemption = e.RewardRedeemed.Redemption;
            var redemption = this.RedemptionFromAPIRedemption(apiRedemption);
            if (this.UnmanagedRewards.ContainsKey(apiRedemption.Reward.Title)) {
                this.UnmanagedRewards[apiRedemption.Reward.Title](redemption, this.CustomRewardRedemptionStatusFromString(apiRedemption.Status));
                if (apiRedemption.Status == "FULFILLED") {
                    this.UnmanagedRewardRedemptionsThisStream[apiRedemption.Reward.Title].Add(redemption);
                }
                return;
            }

            if (this.ManagedRewardsByID.ContainsKey(apiRedemption.Reward.Id) == false) {
                return;
            }

            var reward = this.ManagedRewardsByID[apiRedemption.Reward.Id];
            redemption.Reward = reward;

            if (redemption.User.IsPermitted(reward.Permissions) == false) {
                Logger.LogInfo("User not permitted");
                this.CancelRedemption(redemption);
                return;
            }

            var lowercaseInputs = reward.ValidInputs.Select(input => input.ToLowerInvariant());
            if (reward.RequiresUserInput && reward.ValidInputs.Count > 0 &&
                lowercaseInputs.Contains(redemption.UserInput.ToLowerInvariant()) == false) {
                Logger.LogInfo("Invalid input entered: " + redemption.UserInput);
                this.CancelRedemption(redemption);
                return;
            }

            //Just make extra sure we don't redeem
            if (apiRedemption.Status == "CANCELED") {
                reward.HandleRedemption(redemption, CustomRewardRedemptionStatus.CANCELED);
                return;
            }

            if (apiRedemption.Status == "UNFULFILLED") {
                if (reward.AutoFulfills == true) {
                    this.FulfillRedemption(redemption);
                    return;
                }
                reward.HandleRedemption(redemption, CustomRewardRedemptionStatus.UNFULFILLED);
                return;
            }
            
            reward.HandleRedemption(redemption, CustomRewardRedemptionStatus.FULFILLED);
        }

        private CustomRewardRedemptionStatus CustomRewardRedemptionStatusFromString(string status) {
            if (status == "CANCELED") {
                return CustomRewardRedemptionStatus.CANCELED;
            } else if (status == "FULFILLED") {
                return CustomRewardRedemptionStatus.FULFILLED;
            }

            return CustomRewardRedemptionStatus.UNFULFILLED;
        }

        private ChannelPointRedemption RedemptionFromAPIRedemption(Redemption apiRedemption) {
            return new ChannelPointRedemption() {
                RedeemedAt = apiRedemption.RedeemedAt,
                UserInput = apiRedemption.UserInput,
                User = this.UserManager.UserForChannelPointsRedeem(apiRedemption),
                RedemptionID = apiRedemption.Id,
            };
        }
        
        private void PubSub_RewardUpdated(object sender, OnRewardRedeemedArgs e) {
            if ((e.Status != "ACTION_TAKEN") || (this.ManagedRewardsByID.ContainsKey(e.RewardId.ToString()) == false)) {
                return;
            }
            
            var reward = this.ManagedRewardsByID[e.RewardId.ToString()];

            var task = this.HelixAPI.ChannelPoints.GetCustomRewardRedemptionAsync(this.ChannelID, e.RewardId.ToString(), new List<string>() { e.RedemptionId.ToString() });
            TwitchManager.RunTask(task, (obj) => {
                var responseRedemption = obj.Data[0];
                var redemption = this.RedemptionFromGetRedemptionResponse(responseRedemption);
                reward.HandleRedemption(redemption, responseRedemption.Status);
            });
        }

        private ChannelPointRedemption RedemptionFromGetRedemptionResponse(RewardRedemption redemption) {
            return new ChannelPointRedemption() {
                RedeemedAt = redemption.RedeemedAt,
                UserInput = redemption.UserInput,
                User = this.UserManager.UserForChannelPointsRedemptionResponse(redemption),
                RedemptionID = redemption.Id
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

        private void UpdateRedemptionStatus(ChannelPointRedemption redemption, CustomRewardRedemptionStatus newStatus) {
            var statusRequest = new UpdateCustomRewardRedemptionStatusRequest() { Status = newStatus };
            var task = this.HelixAPI.ChannelPoints.UpdateRedemptionStatusAsync(this.ChannelID, redemption.Reward.Id, new List<string>() { redemption.RedemptionID }, statusRequest);
            TwitchManager.RunTask(task, (obj) => {
                Logger.LogInfo("Updated to status: "+ obj.Data[0].Status);
            });
        }
        #endregion
    }
}

