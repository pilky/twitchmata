# RaidManager

A `RaidManager` allows you to start and cancel raids, and respond to raid events

## Respond to Incoming Raid

```
public override void RaidReceived(Models.IncomingRaid raid) {
	Debug.Log($"{raid.Raider.DisplayName} raided with {raid.ViewerCount}");
}
```

## Respond to Outgoing Raid

```
public override void RaidUpdated(Models.OutgoingRaidUpdate raid) {
    Debug.Log($"Raiding {raid.RaidTarget.DisplayName} with {raid.ViewerCount} viewers");
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

