using System;

namespace Twitchmata.Models {
    /// <summary>
    /// Details on an incoming raid
    /// </summary>
    public struct IncomingRaid {
        /// <summary>
        /// A User representing the raider
        /// </summary>
        public User Raider;
        /// <summary>
        /// The number of viewers that came in with the raid
        /// </summary>
        public int ViewerCount;
    }

    /// <summary>
    /// Details for an outgoing raid, returned after starting a raid
    /// </summary>
    public struct OutgoingRaid {
        /// <summary>
        /// A user representing the target of the raid
        /// </summary>
        public User RaidTarget;

        /// <summary>
        /// The time the raid was created
        /// </summary>
        public DateTime CreatedAt;

        /// <summary>
        /// Whether the raid target has marked their stream as mature
        /// </summary>
        public bool IsMature;
    }
}
