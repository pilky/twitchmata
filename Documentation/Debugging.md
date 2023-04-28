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

## Debug Chat Commands

Twitchmata also offers the ability to trigger a simplified version of the feature manager debug functionality through chat commands. To enable these simply set `TwitchManager.EnableDebugCommands` to `true` or click the checkbox in the Unity inspector.

Once enabled you get access to the following commands:

- `!debug-bits` simulates user sending in bits. You can pass in a bit amount and a twitch username to customise the amount and sender (e.g. `!debug-bits 200 pilkycrc`)
- `!debug-follow` simulates a user following. You can pass in a twitch username simulate that user following (e.g. `!debug-follow doigswift`)
- `!debug-sub` simulates a user subscribing. You can pass in the tier (either `tier1`, `tier2`, `tier3`, or `prime`) and a twitch username to simulate that user subscribing at that tier (e.g. `!debug-sub tier3 spacepiratefenrir`)
- `!debug-gift-sub` simulates a user gifting a sub to another user. You can pass in the tier, the recipient's twitch username, and the gifter's twitcher username (or `anon` for an anonymous gift sub) to simulate a sub from gifter to recipient at that tier (e.g. `!debug-gift-sub tier1 renerighthere dinocrys`)
- `!debug-raid` simulates a user raiding the channel. You can pass in a viewer count and a twitch username for the raider to simulate that user raiding with the number of viewers (e.g. `!sub-raid 26 lanmana`)