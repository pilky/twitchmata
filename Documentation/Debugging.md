# Debugging

Twitchmata offers several tools to aid in debugging your overlay.

## Logging

Most parts of Twitchmata log out various bits of information. These logs are split into 3 types:

- Errors: Logged when something went wrong
- Warnings: Logged when something is used incorrectly, even though it may seem to work
- Info: Logged when something happened, usually when some event fires

By default Twichmata only logs out errors, but you can customise this by changing the log level. There are 3 levels that match the 3 types of logs. Setting the logging level to a value will show any logs at that level, as well as any more severe logs (e.g. the Warning level logs out both Warning and Error messages).

You can change the log level by selecting your `TwitchManager` game object in Unity and changing the Log Level field in the inspector. Alternatively you can change this in code by updating the `.LogLevel` property of `TwitchManager`


## Feature Manager

While it is easy to fire off a chat message or redeem a reward for testing purposes, some feature of Twitch are harder to simulate without help. Twitchmata offers various methods to aid you.

You can call any of these methods without any arguments to get a standard test event. Alternatively, you can override argument to customise the event. See the documentation for each method to see what arguments are available

### Bits

`BitsManager` offers the `Debug_SendBits()` method to simulate receiving bits.

### Follows

`FollowManager` offers the `Debug_NewFollow()` method to simulate receiving a new follower

### Subscribers

`SubscriberManager` offers the `Debug_NewSubscription()` and `Debug_NewGiftSubscription()` methods to simulate receiving new subscription and gift subscription events respectively

### Raids

`RaidManager` offers the `Debug_IncomingRaid()` method to simulate receiving a raid