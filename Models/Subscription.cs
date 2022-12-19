using System;

namespace Twitchmata.Models { 
    public class Subscription {
        User User;
        bool SubscribedMonthCount; //Chat + PubSub
        bool StreakMonths; //PubSub
        string SubscriptionPlan; //PubSub + API
        string PlanName; //PubSub + API
        bool IsGift; //PubSub + API
        User? Gifter;
    }
}

