#Utilities

`Utilities` contains various extra methods that can assist with use in Unity

## `FetchAvatarForUserAsTexture()`

Passing in a `User` object will fetch a `Texture2D` containing that user's profile picture

```
this.Manager.Utilities.FetchAvatarForUserAsTexture(user, (texture) => {
    //do something with the texture
});
```
