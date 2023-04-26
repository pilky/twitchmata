using System;

namespace Twitchmata {
    /// <summary>
    /// A flag enum for specifying permissions for various callbacks. In all cases the broadcaster has permission to all parts of the API
    /// </summary>
    /// <remarks>
    /// You can combine multiple permissions using the | character, e.g. <code>Permissions.VIPs | Permissions.Subscribers</code>.
    /// </remarks>
    [Flags]
    public enum Permissions {
        /// <summary>
        /// Only the broadcaster is permitted to use the command. This is implied by all other permissions so never needs to be passed in with them
        /// </summary>
        Broadcaster = 0,
        /// <summary>
        /// Channel Moderators are permitted to use the command
        /// </summary>
        Mods = 1,
        /// <summary>
        /// Channel VIPs are permitted to use the command
        /// </summary>
        VIPs = 2,
        /// <summary>
        /// Channel Subscribers are permitted to use the command
        /// </summary>
        Subscribers = 4,
        /// <summary>
        /// Anyone viewing the stream is permitted to use the command. This obviously overrides all other permissions
        /// </summary>
        Everyone = 8
    }
}
