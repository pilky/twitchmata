# Chat

A `ChatManager` gives you control over the chat, including being notified when chatters enter or leave, as well as performing various chat functions such as announcements and shout outs.

## Chatter Notifications

`ChatManager` provides various methods to allow you to know when a chatter, moderator, or VIP enters or leaves your chat. These methods are:

- `ChatterJoined`/`Left`
- `ModeratorJoined`/`Left`
- `VIPJoined`/`Left`

While you are notified when moderators and VIPs start watching the stream, for general chatters you only get notified when they first type in chat.

You can also find a list of current chatters, moderators, and VIPs that the `ChatManager` knows about by using `.Chatters`, `.Moderators`, and `.VIPs` respectively.

## Announcements
You can send announcements to chat using the `SendAnnouncement()` method. This only requires a string containing the message, however you can optionally pass a colour or a preferred account to send from (either the broadcasting account or the bot account).

## Shout Outs
You can also send shout outs using the `ShoutOut()` method. There are two types of shout out supported by Twitchmata:

- Native shout outs, which will use Twitch's built in shout out command, which shows a follow link and notifies the streamer they were shouted out.
- Text shout outs, which are simply chat messages that link to the streamer

To use the `ShoutOut()` function just pass in the streamer's name. Twitchmata will default to trying a native shout out, but if you want to force it to use a text shout out pass `false` as the second argument.

Twitch has cooldowns on native shout outs (2 minutes per shoutout and 60 minutes for each streamer shouted out). Twitchmata will try to manage these cooldowns for you. If you run `ShoutOut()` again during a timeout then it will fall back to using a text shoutout.

###Customising Text Shoutouts

You can customise the textual shoutouts by changing the `.ShoutOutTemplate` property on your `ChatManager`. This can be done either in code or through the Unity inspector.

Textual shoutouts have 3 tokens that can be replaced:

- `{{user}}`: the display name of the user being shouted out
- `{{category}}`: the category they were last streaming
- `{{url}}`: the Twitch url for the streamer

Twitchmata will replace all instances of thes tokens in the template string with the appropritate value. While it's recommended you use all 3 tokens, it is not required.