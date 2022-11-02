using System.Collections;
using System.Collections.Generic;
using F10.StreamDeckIntegration;
using F10.StreamDeckIntegration.Attributes;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses.Messages.Redemption;
using TwitchLib.Unity;
using UnityEngine;

namespace Twitchmata {

    public class FollowManager : FeatureManager {
        override public void InitializePubSub(PubSub pubSub) {
            pubSub.OnFollow -= PubSub_OnFollow;
            pubSub.OnFollow += PubSub_OnFollow;
            pubSub.ListenToFollows(Twitchmata.Config.channelID);
        }

        private void PubSub_OnFollow(object sender, OnFollowArgs args) {
            this.NewFollower(args);
        }

        public virtual void NewFollower(OnFollowArgs follower) {
            Debug.Log("User followed");
        }
    }
}
