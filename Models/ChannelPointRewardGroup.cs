using System.Collections.Generic;

namespace Twitchmata.Models {
    /// <summary>
    /// Class to hold multiple rewards to make it easier to mass enable/disable rewards
    /// </summary>
    public class ManagedRewardGroup {
        /// <summary>
        /// The rewards in the group
        /// </summary>
        public List<ManagedReward> Rewards { get; private set; } = new List<ManagedReward>() { };

        /// <summary>
        /// Add a reward to the group
        /// </summary>
        /// <param name="reward">The reward to add</param>
        public void AddReward(ManagedReward reward) {
            if (this.Rewards.Contains(reward)) {
                return;
            }
            this.Rewards.Add(reward);
        }

        /// <summary>
        /// Create a new group from an array of rewards
        /// </summary>
        /// <param name="rewards">The array of rewards to create the group with</param>
        public ManagedRewardGroup(ManagedReward[] rewards) {
            this.Rewards = new List<ManagedReward>(rewards);
        }
    }
}

