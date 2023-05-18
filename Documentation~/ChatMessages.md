# ChatMessageManager

A `ChatMessageManager` allows you to create and respond to messages in chat.

## Chat Commands

Chat commands are messages typed in chat with a prefix of !. Below is an example `ChatCommandManager` subclass which adds a command for mods and the broadcaster to print a message in chat containing an input

```
using Twitchmata;

class MyChatCommandManager : ChatCommandManager {
	override void InitializeFeatureManager() {
		this.RegisterChatCommand("message", Permissions.Mods, MessageCommandReceived);
	}

	private void MessageCommandReceived(List<string> arguments, Models.User user) {
		if (arguments.Count != 1) {
			return;
		}
		this.Connection.Client.SendMessage(this.Connection.ConnectionConfig.ChannelName, $"{user.DisplayName} said {arguments[0]}");
	}
}
```

A mod or the broadcaster can now type `!message hello` in chat and it will post a message "User X said hello".


## Message Matchers

Sometimes you want to respond to something in a chat message, for example trigging an action or sound in your overlay when someone says "hello". Message Matchers help you set up these triggers.

Message Matchers come in two varieties: String and Regex.

### String Matchers

These perform basic matches against a message, checking if the message is either an exact match to the text, starts with the text, or simply contains the text. You can create a string message matcher using `AddStringMessageMatcher()` on `ChatMessageManager`, passing in the text to match, the kind of match, and a callback.

String matchers have two optional arguments:
- `permissions` takes a standard permissions flag set, similar to Managed Rewards or Chat Commands
- `options` allows you to customise the string matcher. It currently supports two options: 
  - `CaseInsensitive`: ignores case when matching text
  - `FullWordsOnly`: only matches against full words. For example, if the match text is "bar", it will match "the bar keeper" but not "the barrel" 

### Regular Expression Matchers

These provide far more power by matching against a C# `Regex`. As with String Matchers they can also take a `Permissions` flag set.

### Matches

Message Matcher callbacks are sent a `MessageMatch` when a valid match is found. These contain the user who sent the message, the matching text, and (if relevant) the regex `Match` object.

## Announcements
You can send announcements to chat using the `SendAnnouncement()` method. This only requires a string containing the message, however you can optionally pass a colour or a preferred account to send from (either the broadcasting account or the bot account).

## Shout Outs
You can also send shout outs using the `ShoutOut()` method. There are two types of shout out supported by Twitchmata:

- Native shout outs, which will use Twitch's built in shout out command, which shows a follow link and notifies the streamer they were shouted out.
- Text shout outs, which are simply chat messages that link to the streamer

To use the `ShoutOut()` function just pass in the streamer's name. Twitchmata will default to trying a native shout out, but if you want to force it to use a text shout out pass `false` as the second argument.

Twitch has cooldowns on native shout outs (2 minutes per shoutout and 60 minutes for each streamer shouted out). Twitchmata will try to manage these cooldowns for you. If you run `ShoutOut()` again during a timeout then it will fall back to using a text shoutout.

### Customising Text Shoutouts

You can customise the textual shoutouts by changing the `.ShoutOutTemplate` property on your `ChatManager`. This can be done either in code or through the Unity inspector.

Textual shoutouts have 3 tokens that can be replaced:

- `{{user}}`: the display name of the user being shouted out
- `{{category}}`: the category they were last streaming
- `{{url}}`: the Twitch url for the streamer

Twitchmata will replace all instances of these tokens in the template string with the appropritate value. While it's recommended you use all 3 tokens, it is not required.