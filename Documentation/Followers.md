# FollowersManager

A `FollowersManager` allows you to manage follow events

## Respond to Follow

```
public override void UserFollowed(Models.User follower) {
	Debug.Log($"User followed: {follower.DisplayName}");
}
```