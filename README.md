#Twitchmata

This is a set of files to help simplify integrating Twitch with an Animata VIPO stream built in Unity. It is built upon [TwitchLib](https://github.com/TwitchLib) so you will first need to include that in your Unity project.

This is currently a work in progress so is missing documentation and functionality. The hope is that it can be built upon to provide all the functionality streamers need.


##Setup

Currently you need to create your authentication tokens, refresh tokens, and client secrets manually at first and save them to text files on disk (you can find the names of the files in `Secrets.cs`). You then need to add your channel name, id, and the path to the folder containing the secrets files to `Config.cs`


##Using

Most of the functionality you'll need are in the Feature Managers. Currently there are managers for Bits, Raids, Followers, Subscribers, Chat Commands, and Channel Points. 

If you look at the classes you'll see some `public virtual` methods. The intention is you subclass the Feature Managers you need and override these methods. The Feature Managers are `MonoBehaviours` so you should add these to a game object (ideally one that persists). This allows you to easily connect up other game objects and hook into the unity update cycle if needed.

At this point you should be able to instantiate a `TwitchManager`, usually in your main script. You then need to fetch the feature managers (usually by doing `var featureManager = gameObjectWithManagersOn.GetComponent<ManagerType>();`) and pass them to the manager by calling `AddFeatureManager(featureManager)` on the twitch manager. At this point you can now call `Connect()`.

For a longer example:

```
using Twitchmata;

//Note: In this example feature managers added to this object instance in scene
public class MyMainScript: MonoBehaviour {
	TwitchManager apiManager;

	void Start() {
		this.apiManager = new TwitchManager();
		
		this.apiManager.AddFeatureManager(GetComponent<BitsManager>());
		this.apiManager.AddFeatureManager(GetComponent<RaidManager>());
		this.apiManager.AddFeatureManager(GetComponent<SubscriberManager>());
		
		this.apiManager.Connect();
	}	
}
```
