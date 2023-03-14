using System;
using System.Text.RegularExpressions;
using TwitchLib.Client.Models;
using Twitchmata;
using Twitchmata.Models;

namespace External.Twitchmata.Models {
    internal class MessageMatcher {
        internal ChatMessageManager.MessageMatcherCallback Callback { get; }
        
        internal Permissions Permissions { get; }
        internal MessageMatcher(ChatMessageManager.MessageMatcherCallback callback, Permissions permissions = Permissions.Everyone) {
            this.Callback = callback;
            this.Permissions = permissions;
        }

        internal void HandleMessage(ChatMessage message, UserManager userManager) {
            var user = userManager.UserForChatMessage(message);
            if (user.IsPermitted(this.Permissions) == false) {
                return;
            }

            var result = this.MatchInMessage(message.Message);
            if (result == null) {
                return;
            }

            var matchResult = (MatchResult)result;
            
            var messageMatch = new MessageMatch() {
                User = user,
                Message = message.Message,
                BackingChatMessage = message,
                Match = matchResult.MatchText,
                RegexMatch = matchResult.RegexMatch
            };
            this.Callback(messageMatch);
        }

        internal virtual MatchResult? MatchInMessage(string message) {
            return null;
        }
    }

    internal struct MatchResult {
        internal string MatchText;
        internal Match RegexMatch;
    }

    internal class StringMessageMatcher : MessageMatcher {
        internal string MatchString { get; }
        internal ChatMessageManager.StringMatchKind Kind { get; }
        internal bool CaseInsensitive { get; }
        internal StringMessageMatcher(string matchString, ChatMessageManager.StringMatchKind kind, ChatMessageManager.MessageMatcherCallback callback, bool caseInsensitive = true, Permissions permissions = Permissions.Everyone) : base(callback, permissions) {
            this.CaseInsensitive = caseInsensitive;
            this.MatchString = matchString;
            this.Kind = kind;
        }

        internal override MatchResult? MatchInMessage(string message) {
            var stringOptions = (this.CaseInsensitive ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture);
            switch (this.Kind) {
                case ChatMessageManager.StringMatchKind.Equals:
                    if (message.Equals(this.MatchString, stringOptions)) {
                        return new MatchResult() { MatchText = this.MatchString };
                    }

                    break;
                case ChatMessageManager.StringMatchKind.StartsWith:
                    if (message.StartsWith(this.MatchString, stringOptions)) {
                        return new MatchResult() { MatchText = this.MatchString };
                    }

                    break;
                case ChatMessageManager.StringMatchKind.Contains:
                    if (message.Contains(this.MatchString, stringOptions)) {
                        return new MatchResult() { MatchText = this.MatchString };
                    }

                    break;
            }

            return null;
        }
    }

    internal class RegexMessageMatcher : MessageMatcher {
        internal Regex Regex;
        internal RegexMessageMatcher(Regex regex, ChatMessageManager.MessageMatcherCallback callback, Permissions permissions = Permissions.Everyone) : base(callback, permissions) {
            this.Regex = regex;
        }

        internal override MatchResult? MatchInMessage(string message) {
            var match = this.Regex.Match(message);
            if (match.Success == false) {
                return null;
            }

            return new MatchResult() { MatchText = match.Value, RegexMatch = match };
        }
    }
}