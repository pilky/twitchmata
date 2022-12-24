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

        #region Permissions
        internal bool IsPermitted(Permissions permissions) {
            if ((permissions & Permissions.Everyone) == Permissions.Everyone) {
                return true;
            }
            if (this.IsBroadcaster) {
                return true;
            }
            if ((permissions & Permissions.Mods) == Permissions.Mods && this.IsModerator) {
                return true;
            }
            if ((permissions & Permissions.VIPs) == Permissions.VIPs && this.IsVIP) {
                return true;
            }
            if ((permissions & Permissions.Subscribers) == Permissions.Subscribers && this.IsSubscriber) {
                return true;
            }
            return false;
        }
        #endregion
    }
}
