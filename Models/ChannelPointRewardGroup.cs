using System;
using System.Collections.Generic;

namespace Twitchmata.Models { 
    public class ManagedRewardGroup {
        public List<ManagedReward> Rewards { get; private set; } = new List<ManagedReward>() { };

        public void AddReward(ManagedReward reward) {
            if (this.Rewards.Contains(reward)) {
                return;
            }
            this.Rewards.Add(reward);
        }


        public ManagedRewardGroup(ManagedReward[] rewards) {
            this.Rewards = new List<ManagedReward>(rewards);
        }
    }
}

