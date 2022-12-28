# FollowersManager

A `FollowersManager` allows you to manage follow events

## Respond to Incoming Raid

```
public override void RaidReceived(Models.IncomingRaid raid) {
	Debug.Log($"{raid.Raider.DisplayName} raided with {raid.ViewerCount}");
}
```

## Set Up an Outgoing Raid

```
public SetupRaid(string username) {
	//Fetch the user based on the username
	this.Connection.UserManager.FetchUserWithUserName(username, (user) => {
		//Start the raid
		this.RaidManager.StartRaid(user, (outgoingRaid) => {
			Debug.Log($"Starting to raid {outgoingRaid.RaidTarget.DisplayName}. Is a mature stream? {outgoingRaid.IsMature}");
		});
	});
}
```

## Cancel a Pending Raid

```
this.RaidManager.CancelRaid();
```