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
        this.ChangeThemeReward = this.RegisterReward("Change Theme", 200, ChangeThemeRedeemed, Permissions.Subscribers | Permissions.Mods);
        this.ChangeThemeReward.Description = "Change the theme of Pilkybot's screen. Options include: default, terminal, pixels, bsod";
        this.ChangeThemeReward.RequiresUserInput = true;
        this.ChangeThemeReward.ValidInputs = new List<string>() { "default", "terminal", "pixels", "bsod" };
        this.ChangeThemeReward.GlobalCooldownSeconds = 600;
    }

    private void ChangeThemeRedeemed(ChannelPointRedemption redemption, CustomRewardRedemptionStatus status) {
        if (status == CustomRewardRedemptionStatus.FULFILLED) {
            Debug.Log("User redeemed with input: " + redemption.UserInput);
        }
    }
}
```

You need to keep a reference to the created `ManagedReward` objects if you wish to update them in future. It's recommended you store them in properties in your `ChannelPointManager` subclass.

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

### Reward Groups

`ManagedRewardGroup` provides an easy way to group multiple rewards so they can be enabled or disabled at once. When you have a group you can enable and disable them like so:

```
this.DisableGroup(this.TheatreRewards);

this.EnableGroup(this.TheatreRewards);
```

### Manual Reward Fulfillment

By default `ManagedReward`s that are valid will automatically fulfil themselves, removing them from the reward queue in the Twitch dashboard. You will only receive the callback when the redemption is fulfilled 

However, you sometimes want more control over when a reward is fulfilled, including the ability to cancel it later. In these cases you can disable automatic fulfillment of a particular `ManagedReward` by setting its `AutoFulfills` property to `false`.

When a `ManagedReward` is configured for manual fulfillment the callback will first be called with an `Unfulfilled` status. You can then fulfil at a later time by passing the redemption to the `FulfillReward()` method of `ChannelPointManager`. Alternatively you can cancel the reward (refunding the user's channel points) by calling the `CancelRedemption()` method. Once the redemption has been successfully fulfilled or cancelled you will receive another callback with the updated status.

Manually fulfilled `ManagedReward`s can also be fulfilled from the Twitch dashboard. In these cases Twitchmata will invoke the reward's callback with the updated state. This allows your overlay to respond to changes in the dashboard (for example, a mod rejecting a reward).

### Reward Cancellation

Any invalid managed rewards will be automatically cancelled. Invalid rewards include those where the user doesn't have permission or have entered invalid input. This occurs regardless of whether the `ManagedReward` is set up for automatic or manual fulfillment.

Usually you don't care about responding to a redemption being cancelled, so the callback is not invoked. In cases where you do wish to be notified when a redemption is cancelled you can set the `InvokesCallbackIfCancelled` on the `ManagedReward` to `true`.