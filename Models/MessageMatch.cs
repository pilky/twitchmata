using System.Text.RegularExpressions;
using TwitchLib.Client.Models;

namespace Twitchmata.Models {
    public struct MessageMatch {
        public User User;

        public string Message;

        public string Match;

        public Match RegexMatch;

        public ChatMessage BackingChatMessage;
    }
}