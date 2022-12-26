using System.Collections;
using System.Collections.Generic;
using TwitchLib.PubSub.Events;
using TwitchLib.PubSub.Models.Responses.Messages.Redemption;
using TwitchLib.Unity;
using UnityEngine;

namespace Twitchmata {
    /// <summary>
    /// Used to hook into follower events in your overlay
    /// </summary>
    /// <remarks>
    /// To utilise FollowManager create a subclass and add to a GameObject (either the
    /// GameObject holding TwitchManager or a child GameObject).
    ///
    /// Then override <code>UserFollowed()</code> and add your follow-handling code.
    /// </remarks>
    public class FollowManager : FeatureManager {
        #region Notifications
        /// <summary>
        /// Fired when a user follows the channel
        /// </summary>
        /// <param name="follower">A User object with details on the follower</param>
        public virtual void UserFollowed(Models.User follower) {
            Debug.Log($"User followed: {follower.DisplayName}");
        }
        #endregion

        #region Stats
        /// <summary>
        /// List of users who have followed while the overlay has been open
        /// </summary>
        public List<Models.User> FollowsThisStream { get; private set; } = new List<Models.User>() { };
        #endregion



        /**************************************************
         * INTERNAL CODE. NO NEED TO READ BELOW THIS LINE *
         **************************************************/

        #region Internal
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
        #endregion
    }
}
