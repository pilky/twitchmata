# Channel Points/Rewards

A `ChannelPointManager` helps you manage rewards that users can spend channel points on. The `ChannelPointManager` class supports 2 types of reward: Managed and Unmanaged.

## Unmanaged Rewards
Unmanaged rewards are rewards that were created by something other than your overlay (usually the Twitch dashboard). Twitchmata has less control over these so you are limited to simply being notified when one is redeemed.

You can register an unmanaged reward using the `RegisterUnmanagedReward()` function, passing in the title of the reward and a delegate method to call when the reward is redeemed.

## Managed Rewards
Managed rewards offer a lot more power and flexibility as they are created and handled by Twitchmata. Additional functionality includes:

- Enabling/disabling rewards
- Updating reward cost
- Reward permissions
- Input validation
- Automatic refunding of channel points if not redeemed

See the examples below for how to use managed rewards

### Registering Managed Reward

You need to register any managed rewards with the `ChannelPointManager` each time your overlay starts. Twitchmata will then make sure the reward exists and is up-to-date.

If you wish to convert an existing reward to a managed reward, you will need to first remove it from Twitch and then set it up in your overlay.

```
public class TwitchChannelPointManager : Twitchmata.ChannelPointManager {
    public ManagedReward ChangeThemeReward;

    public override void InitializeFeatureManager() {
        this.ChangeThemeReward = new ManagedReward("Change Theme", 200, Permissions.Subscribers | Permissions.Mods);
        this.ChangeThemeReward.Description = "Change the theme of Pilkybot's screen. Options include: default, terminal, pixels, bsod";
        this.ChangeThemeReward.RequiresUserInput = true;
        this.ChangeThemeReward.ValidInputs = new List<string>() { "default", "terminal", "pixels", "bsod" };
        this.ChangeThemeReward.GlobalCooldownSeconds = 600;
        this.RegisterReward(this.ChangeThemeReward, ChangeThemeRedeemed);
    }

    private void ChangeThemeRedeemed(ChannelPointRedemption redemption) {
        Debug.Log("User redeemed with input: " + redemption.UserInput);
    }
}
```

You need to keep a reference to the created `ManagedReward` objects if you wish to update them in future. It's recommended you store them in properties in your `ChannelPointManager` subclass

### Updating Managed Reward

You can disable and enable rewards like so:

```
this.DisableReward(this.ChangeThemeReward);

this.EnableReward(this.ChangeThemeReward);
```

You can also update the cost of a reward like so:

```
this.UpdateRewardCost(this.ChangeThemeReward, 100);
```