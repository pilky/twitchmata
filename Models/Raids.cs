using System;

namespace Twitchmata.Models {
    public struct IncomingRaid {
        public User Raider;
        public int ViewerCount;
    }

    public struct OutgoingRaid {
        public User RaidTarget;
        public DateTime CreatedAt;
        public bool IsMature;
    }
}
