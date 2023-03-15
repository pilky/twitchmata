# Statistics

Many parts of Twitchmata collect statistics as your stream runs. Currently these stats only last as long as your overlay is running, but future versions will persist these.

## Available Statistics

### Follows
`FollowManager` tracks the following:

- `FollowsThisStream`: a list of `User` objects for new follower during this stream

### Subscription
`SubscriberManager` tracks the following:

- `SubscribersThisStream`: a list of `User` objects for every subscription event this stream (both new subs and re-subs)
- `GiftersThisStream`: a list of `User` objects for everyone who has gifted one or more subscriptions this stream

### Raids
`RaidManager` tracks the following:

- `RaidsThisStream`: a list of `IncomingRaid` objects for every raid that occurred during the stream

### Bit Redemptions
`BitsManager` tracks the following:

- `RedemptionsThisStream`: a list of `BitRedemption` objects for every bit redemption that occured during the stream

### Channel Reward Redemptions
`ChannelPointManager` tracks the following:

- `UnmanagedRewardsThisStream`: a dictionary keyed by the title of any unmanaged rewards registered with the manager. The values are lists containing the `ChannelPointRedemption` for any fulfilled redemptions that occured during the stream

To track redemptions of managed rewards each `ManagedReward` has a `RedemptionsThisStream` property that stores a list of all fulfilled redemptions that occured for that reward during the stream.