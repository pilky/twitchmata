using System;

namespace Twitchmata.Models {
    class ChannelPointReward {
        string? Id;
        string Title;

        RewardDetails? Details;
    }

    struct RewardDetails {
        int Cost;
        bool IsEnabled;
        int? MaxPerStream;
        int? MaxPerUserPerStream;
        int? GlobalCooldownSeconds;
        bool IsUserInputRequired;
    }
}

