using System;
using System.Collections.Generic;

namespace Twitchmata {
    public delegate void ChatCommandCallback(List<string> arguments, Models.User user);

    internal struct RegisteredChatCommand {
        internal Permissions Permissions;
        internal ChatCommandCallback Callback;
    }

    [Flags]
    public enum Permissions {
        None = 0,
        Mods = 1,
        VIPs = 2,
        Subscribers = 4,
        Chatters = 8
    }
}
