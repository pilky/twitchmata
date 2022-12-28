# Twitchmata

Twitchmata is a library to help you integrate an Animata/VIPO stream overlay with Unity. 

Twitchmata has been tested on Unity 2022.1+


## What is an Animata/VIPO Stream Overlay?

Animata are a type of VTuber, virtual stream avatars common on Twitch. Animata are more akin to puppets, often controlled with a game controller, and they exist in a VIPO: **Virtual Interactive Puppet Overlay**. A VIPO is a virtual environment, often built in a game engine, which an Animata can move around in and interact with. They also provide interactivity to viewers, responding to various events on Twitch, which is where Twitchmata comes in.


## What Does Twitchmata Do?

Twitch has various APIs to request information and get notified of events. There already exists an excellent implementation of this API in C# called [TwitchLib](https://github.com/twitchlib/twitchlib). However, this API is designed to be generic and work for many different use-cases.

Twitchmata is a wrapper around TwitchLib to optimise the API for VIPOs and provide better Unity integration. It provides help for authenticating accounts and handles a lot of the boilerplate code so that you can focus on implementing what makes your stream unique.

Twitchmata currently has support for dealing with Follows, Subscribers, Raids, Bits, Chat (including Chat Commands), and Channel Points.


## Setup

You can find out more about setting up Twitchmata in your overlay [here](https://github.com/pilky/twitchmata/tree/main/Documentation/Setup.md)


## Usage Overview

Twitchmata has a few key classes:

- `TwitchManager` is the base class which acts as the root of the Twitch integration and is where you do most of the configuration
- `ConnectionManager` handles the connection to Twitch through various API endpoint. You mostly don't need to touch this, but it lets you easily drop down to using TwitchLib directly if you need some functionality Twitchmata does not provide.
- `UserManager` holds a list of users known to Twitchmata. Most APIs will give you a `User` type with information about who invoked it
- `FeatureManagers` are where most of your custom code will go. See below on how to use them.
- `Utilities` provides additional functionality that doesn't fit in a FeatureManager (e.g. downloading a user's avatar as a `Texture2D`). [(documentation)](https://github.com/pilky/twitchmata/tree/main/Documentation/Utilities.md)

### Feature Managers

Twitchmata provides access to various bits of Twitch functionality through Feature Managers. Feature Managers are intended to be subclassed by you to allow your overlay to respond to notifications or invoke various features. Below gives an example on how to set up a Feature Manager for handling follower:

1. Create a new C# Script in Unity called "MyFollowerManager" (or whatever name you desire) and open in your code editor of choice

2. Add `using Twitchmata;` to the includes at the top of the file

3. Change the super class from `MonoBehaviour` to `FollowerManager`

4. Add an empty `GameObject` as a child of the `TwitchManager` object you added during setup

5. Add the "MyFollowerManager" class as a component


Your MyFollowerManager class will now be notified when a user follows your channel and keep a list of everyone who followed while your overlay was open. Twitchmata provides Feature Managers for the following:
- Followers [(documentation)](https://github.com/pilky/twitchmata/tree/main/Documentation/Followers.md)
- Subscribers [(documentation)](https://github.com/pilky/twitchmata/tree/main/Documentation/Subscribers.md)
- Raids [(documentation)](https://github.com/pilky/twitchmata/tree/main/Documentation/Raids.md)
- Channel Points/Rewards [(documentation)](https://github.com/pilky/twitchmata/tree/main/Documentation/ChannelPoints.md)
- Chat Commands [(documentation)](https://github.com/pilky/twitchmata/tree/main/Documentation/ChatCommands.md)
- Chatters [(documentation)](https://github.com/pilky/twitchmata/tree/main/Documentation/Chatters.md)
- Bits [(documentation)](https://github.com/pilky/twitchmata/tree/main/Documentation/Bits.md)