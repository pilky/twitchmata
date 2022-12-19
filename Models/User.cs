using System;

namespace Twitchmata.Models {
    class User {
        string UserId; //Chat + Follower + ChannelPoint + PubSub Sub + API + Raid
        string Username; //Chat + Follower + ChannelPoint + PubSub Sub + API + Raid
        string DisplayName; //Chat + Follower + ChannelPoint + PubSub Sub + API + Raid

        bool IsBroadcaster; //Chat
        bool IsModerator; //Chat + Raid
        bool IsVIP; //Chat
        bool IsSubscriber; //Chat + Raid + PubSub + API

        Subscription? subscription;
    }
}
