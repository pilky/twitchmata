using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace Twitchmata.Models {
    /// <summary>
    /// Result from a MessageMatcher, passed to the callback
    /// </summary>
    public struct MessageMatch {
        /// <summary>
        /// The user who sent the message
        /// </summary>
        public User User;

        /// <summary>
        /// The text of the message
        /// </summary>
        public string Message;

        /// <summary>
        /// The matching text
        /// </summary>
        public string Match;

        /// <summary>
        /// If the matcher used a regular expression, the Match object
        /// </summary>
        public Match RegexMatch;

        /// <summary>
        /// The ChatMessage that backs the match
        /// </summary>
        public ChatMessage BackingChatMessage;
    }
}