# ChatCommandManager

A `ChatCommandManager` allows you to create and respond to chat commands. Chat commands are messages typed in chat with a prefix of !

## Register a Chat Command

Below is an example ChatCommandManager subclass which adds a command for mods and the broadcaster to print a message in chat containing an input

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
