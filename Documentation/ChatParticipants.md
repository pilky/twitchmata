# ChatParticipantManager

A `ChatParticipantManager` gives you control over the chat, including being notified when chatters enter or leave.

## Chatter Notifications

`ChatManager` provides various methods to allow you to know when a chatter, moderator, or VIP enters or leaves your chat. These methods are:

- `ChatterJoined`/`Left`
- `ModeratorJoined`/`Left`
- `VIPJoined`/`Left`

While you are notified when moderators and VIPs start watching the stream, for general chatters you only get notified when they first type in chat.

You can also find a list of current chatters, moderators, and VIPs that the `ChatManager` knows about by using `.Chatters`, `.Moderators`, and `.VIPs` respectively.

## Lurkers

Twitchmata can keep track of viewers who choose to actively lurk in your chat. Streams commonly have `!lurk` and `!unlurk` commands that viewers can use when they want to lurk, but let the streamer know they are lurking.

By default Twitchmata does not respond to these commands (to avoid conflicts with existing commands). To enable Twitchmata's lurk support you need to set the `LurkersEnabled` property of a `ChatParticipantManager` to true. Twitchmata will now track when a user is actively lurking.

You can override the `UserLurked` and `UserUnlurked` methods to be notified when a user changes their lurk status. You can also check if a particular `User` is lurking by checking its `IsLurking` property

### Lurk Messages

When enabled, Twitchmata will post a message when the viewer lurks or unlurks. These messages are set in the `LurkMessage` and `UnlurkMessage` properties. They have two available tokens:

- `{{user}}` will be replaced with the user's display name
- `{{lurker-count}}` will be replaced with the current number of lurkers

If you don't want these messages to be sent you can simply set them to an empty string, either in code or in the Unity inspector.