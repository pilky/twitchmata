using System.Collections;
using System.Collections.Generic;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses.Messages.Redemption;
using TwitchLib.Unity;
using UnityEngine;

namespace Twitchmata {

    public class FollowManager : FeatureManager {
        override internal void InitializePubSub(PubSub pubSub) {
            pubSub.OnFollow -= PubSub_OnFollow;
            pubSub.OnFollow += PubSub_OnFollow;
            pubSub.ListenToFollows(this.ChannelID);
        }

        private void PubSub_OnFollow(object sender, OnFollowArgs args) {
            var user = this.UserManager.UserForFollowNotification(args);
            this.FollowsThisStream.Add(user);
            this.UserFollowed(user);
        }

        #region Notifications
        public virtual void UserFollowed(Models.User follower) {
            Debug.Log($"User followed: {follower.DisplayName}");
        }
        #endregion


        #region Stats
        public List<Models.User> FollowsThisStream { get; private set; } = new List<Models.User>() { };
        #endregion
    }
}
