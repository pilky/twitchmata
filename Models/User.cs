using System;

namespace Twitchmata.Models {
    public class User {
        public string UserId { get; }
        public string UserName { get; }
        public string DisplayName { get; }

        internal User(string userID, string userName, string displayName) {
            this.UserId = userID;
            this.UserName = userName;
            this.DisplayName = displayName;
        }

        public bool IsBroadcaster { get; internal set; } = false;
        public bool IsModerator { get; internal set; } = false;
        public bool IsVIP { get; internal set; } = false;
        public bool IsSubscriber { get; internal set; } = false;

        public Subscription? Subscription { get; internal set; } = null;
    }
}
