using System;

namespace Twitchmata.Models {
    /// <summary>
    /// A viewer's redemption of bits
    /// </summary>
    public struct BitsRedemption {
        /// <summary>
        /// The number of bits redeemed
        /// </summary>
        public int BitsUsed;

        /// <summary>
        /// The total bits this user has redeemed on the channel
        /// </summary>
        public int TotalBitsUsed;

        /// <summary>
        /// The user who redeemed the bits (can be null if the donation was anonymous)
        /// </summary>
        public User User;

        /// <summary>
        /// The time the bits were redeemed at
        /// </summary>
        public DateTime RedeemedAt;

        /// <summary>
        /// The message sent with the bits
        /// </summary>
        public string Message;
    }
}

