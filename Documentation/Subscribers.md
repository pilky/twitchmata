# FollowersManager

A `FollowersManager` allows you to manage follow events

## Respond to Subscription

```
public override void UserSubscribed(Models.User subscriber) {
	if (subscriber.Subscription.IsGift == true) {
		Debug.Log($"{subscriber.DisplayName} received gift sub from {subscriber.Subscription.Gifter.DisplayName}");
	} else {
		Debug.Log($"{subscriber.DisplayName} subscribed. They have subscribed for {subscriber.Subscription.SubscribedMonthCount}");
	}
}
```
