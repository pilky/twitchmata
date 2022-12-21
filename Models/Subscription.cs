using System;

namespace Twitchmata.Models { 
    public class Subscription {
        public int SubscribedMonthCount { get; internal set; } //Chat + PubSub
        public int StreakMonths { get; internal set; } //PubSub
        public SubscriptionTier Tier { get; internal set; } //PubSub + API
        public string PlanName { get; internal set; } //PubSub + API
        public bool IsGift { get; internal set; } //PubSub + API
        public User Gifter { get; internal set; }

        static public SubscriptionTier TierForString(string tierString) {
            if (tierString == "1000") {
                return SubscriptionTier.Tier1;
            } else if (tierString == "2000") {
                return SubscriptionTier.Tier2;
            } else if (tierString == "3000") {
                return SubscriptionTier.Tier3;
            } else if (tierString == "Prime") {
                return SubscriptionTier.Prime;
            }
            return SubscriptionTier.NotSet;
        }
    }

    public enum SubscriptionTier {
        NotSet,
        Prime,
        Tier1,
        Tier2,
        Tier3
    }
}

