using System;
using TwitchLib.Api.Helix.Models.ChannelPoints;
using TwitchLib.Api.Helix.Models.ChannelPoints.CreateCustomReward;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomReward;

namespace Twitchmata.Models {
    public delegate void RewardRedemptionCallback(ChannelPointRedemption redemption);

    public class ManagedReward {
        public string? Id { get; internal set; }

        public string Title { get; internal set; }
        public int Cost { get; internal set; }
        public bool IsEnabled { get; internal set; }
        public Permissions Permissions { get; private set; }

        public ManagedReward(string title, int cost, Permissions permissions = Permissions.Everyone, bool isEnabled = true) {
            this.Title = title;
            this.Cost = cost;
            this.IsEnabled = isEnabled;
            this.Permissions = permissions;
        }

        internal ManagedReward(CustomReward remoteReward) {
            this.Id = remoteReward.Id;
            this.Title = remoteReward.Title;
            this.Cost = remoteReward.Cost;
            this.IsEnabled = remoteReward.IsEnabled;
            this.Description = remoteReward.Prompt;
            this.RequiresUserInput = remoteReward.IsUserInputRequired;

            if (remoteReward.MaxPerStreamSetting.IsEnabled) {
                this.MaxPerStream = remoteReward.MaxPerStreamSetting.MaxPerStream;
            } else {
                this.MaxPerStream = null;
            }

            if (remoteReward.MaxPerUserPerStreamSetting.IsEnabled) {
                this.MaxPerUserPerStream = remoteReward.MaxPerUserPerStreamSetting.MaxPerUserPerStream;
            } else {
                this.MaxPerUserPerStream = null;
            }

            if (remoteReward.GlobalCooldownSetting.IsEnabled) {
                this.GlobalCooldownSeconds = remoteReward.GlobalCooldownSetting.GlobalCooldownSeconds;
            } else {
                this.GlobalCooldownSeconds = null;
            }
        }

        public int? MaxPerStream { get; set; }
        public int? MaxPerUserPerStream { get; set; }
        public int? GlobalCooldownSeconds { get; set; }
        public string Description { get; set; } = "";
        public bool RequiresUserInput { get; set; } = false;

        internal RewardRedemptionCallback Callback { get; set; }

        internal UpdateCustomRewardRequest? UpdateRequestForDifferencesFrom(ManagedReward otherReward) {
            var hasUpdated = false;
            var updateRequest = new UpdateCustomRewardRequest();
            if (this.Cost != otherReward.Cost) {
                updateRequest.Cost = this.Cost;
                hasUpdated = true;
            }
            if (this.IsEnabled != otherReward.IsEnabled) {
                updateRequest.IsEnabled = this.IsEnabled;
                hasUpdated = true;
            }
            if (this.RequiresUserInput != otherReward.RequiresUserInput) {
                updateRequest.IsUserInputRequired = this.RequiresUserInput;
                hasUpdated = true;
            }
            if (this.Description != otherReward.Description) {
                updateRequest.Prompt = this.Description;
                hasUpdated = true;
            }
            if (this.MaxPerStream != otherReward.MaxPerStream) {
                updateRequest.IsMaxPerStreamEnabled = (this.MaxPerStream != null);
                if (this.MaxPerStream != null) { 
                    updateRequest.MaxPerStream = (int)this.MaxPerStream;
                }
                hasUpdated = true;
            }
            if (this.MaxPerUserPerStream != otherReward.MaxPerUserPerStream) {
                updateRequest.IsMaxPerUserPerStreamEnabled = (this.MaxPerUserPerStream != null);
                if (this.MaxPerUserPerStream != null) {
                    updateRequest.MaxPerUserPerStream = (int)this.MaxPerUserPerStream;
                }
                hasUpdated = true;
            }
            if (this.GlobalCooldownSeconds != otherReward.GlobalCooldownSeconds) {
                updateRequest.IsGlobalCooldownEnabled = (this.GlobalCooldownSeconds != null);
                if (this.GlobalCooldownSeconds != null)  {
                    updateRequest.GlobalCooldownSeconds = (int)this.GlobalCooldownSeconds;
                }
                hasUpdated = true;
            }
            if (this.Description != otherReward.Description) {
                updateRequest.Prompt = this.Description;
                hasUpdated = true;
            }

            if (hasUpdated == false) {
                return null;
            }
            return updateRequest;
        }

        internal CreateCustomRewardsRequest CreateRewardRequest() {
            var request = new CreateCustomRewardsRequest();
            request.Title = this.Title;
            request.Prompt = this.Description;
            request.Cost = this.Cost;
            request.IsEnabled = this.IsEnabled;
            request.IsUserInputRequired = this.RequiresUserInput;
            request.IsMaxPerStreamEnabled = (this.MaxPerStream != null);
            if (this.MaxPerStream != null) { 
                request.MaxPerStream = (int)this.MaxPerStream;
            }
            request.IsMaxPerUserPerStreamEnabled = (this.MaxPerUserPerStream != null);
            if (this.MaxPerUserPerStream != null) {
                request.MaxPerUserPerStream = (int)this.MaxPerUserPerStream;
            }
            request.IsGlobalCooldownEnabled = (this.GlobalCooldownSeconds != null);
            if (this.GlobalCooldownSeconds != null) {
                request.GlobalCooldownSeconds = (int)this.GlobalCooldownSeconds;
            }
            return request;
        }
    }
}

