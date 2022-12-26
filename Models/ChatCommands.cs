using System;
using System.Collections.Generic;

namespace Twitchmata {
    /// <summary>
    /// Delegate callback for a chat command, called when a valid chat command is invoked
    /// </summary>
    /// <param name="arguments">Any arguments for the command</param>
    /// <param name="user">The user who invoked the command</param>
    public delegate void ChatCommandCallback(List<string> arguments, Models.User user);

    internal struct RegisteredChatCommand {
        internal Permissions Permissions;
        internal ChatCommandCallback Callback;
    }
}
