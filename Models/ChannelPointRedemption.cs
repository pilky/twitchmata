using System;

namespace Twitchmata.Models {
    /// <summary>
    /// Information about redemption of channel points
    /// </summary>
    public struct ChannelPointRedemption {
        /// <summary>
        /// The user who redeemed the channel points
        /// </summary>
        public User User;
        /// <summary>
        /// The time the channel points were redeemed
        /// </summary>
        public DateTime RedeemedAt;
        /// <summary>
        /// Any user input required by the channel points
        /// </summary>
        public string UserInput;
        /// <summary>
        /// The reward that was redeemed
        /// </summary>
        public ManagedReward Reward;
    }
}

