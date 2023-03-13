using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Twitchmata {
    public class Utilities {
        internal ConnectionManager ConnectionManager;

        public void FetchAvatarForUserAsTexture(Models.User user, Action<Texture2D> callback){
            var task = this.ConnectionManager.API.Helix.Users.GetUsersAsync(new List<string> { user.UserId });
            TwitchManager.RunTask(task, (response) => {
                var imageURL = response.Users[0].ProfileImageUrl;

                var textureRequest = UnityWebRequestTexture.GetTexture(imageURL);
                var asyncOp = textureRequest.SendWebRequest();
                asyncOp.completed += (op) => {
                    var texture = DownloadHandlerTexture.GetContent(textureRequest);
                    callback.Invoke(texture);
                };
            });
        }
    }
}
