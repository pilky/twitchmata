using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Events;
using TwitchLib.Unity;
using UnityEngine;

namespace Twitchmata {
    /// <summary>
    /// Used to respond to and send chat messages and notification in your overlay
    /// </summary>
    /// <remarks>
    /// To utilise ChatMessageManager create a subclass and add to a GameObject (either the)
    /// Game object holding TwitchManager or a child GameObject).
    ///
    /// See <code>RegisterChatCommand()</code> for details on how to set up commands
    /// </remarks>
    public class ChatMessageManager : FeatureManager {
        #region Command Registration
        /// <summary>
        /// Register a chat command that viewers can invoke
        /// </summary>
        /// <param name="command">The name of the command (excluding any command prefix such as !)</param>
        /// <param name="permissions">Permissions for who can invoke the command</param>
        /// <param name="callback">The delegate method to call when this command is invoked</param>
        public void RegisterChatCommand(string command, Permissions permissions, ChatCommandCallback callback) {
            this.RegisteredCommands[command] = new RegisteredChatCommand() {
                Permissions = permissions,
                Callback = callback,
            };
        }
        #endregion

        
        #region Announcements
        /// <summary>
        /// Send an announcement to the chat
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="colour">The announcement colour to use (default: null, i.e. the channel colour)</param>
        /// <param name="preferredAccount">The preferredAccount to post with (falls back to broadcaster if bot is not available)</param>
        public void SendAnnouncement(string message, AnnouncementColors colour = null, PosterAccount preferredAccount = PosterAccount.Broadcaster) {
            var account = this.Connection.ChannelID;
            var accessToken = this.Connection.Secrets.AccountAccessToken;
            if (preferredAccount == PosterAccount.Bot && this.Connection.BotID != null) {
                account = this.Connection.BotID;
                accessToken = this.Connection.Secrets.BotAccessToken;
            }

            var task = this.Connection.API.Helix.Chat.SendChatAnnouncementAsync(this.Connection.ChannelID, account, message, colour, accessToken);
            TwitchManager.RunTask(task, () => {
                Logger.LogInfo("Announcement sent");
            });
        }
        #endregion


        #region Shoutouts
        //Implement in TwitchLib
        //Add rate limit (2min per shoutout, 60m per streamer)
        //Sender (broadcaster or chat bot)
        //Type (native, text, auto [uses native but falls back to text if necessary])
        [Tooltip("The template to use for textual shoutouts. {{user}} will be replaced by streamer's display name, {{category}} by the category they last streamed in, and {{url}} with a link to their stream")]
        public string ShoutOutTemplate = "Check out {{user}}, they are streaming {{category}} at {{url}}";

        private int ShoutOutLimit = 120;
        private int PerStreamerShoutoutLimit = 3600;

        private DateTime? LastNativeShoutout = null;
        private Dictionary<string, DateTime> PastNativeShoutouts = new Dictionary<string, DateTime>() {};

        private Regex UserRegex = new Regex(Regex.Escape("{{user}}"));
        private Regex CategoryRegex = new Regex(Regex.Escape("{{category}}"));
        private Regex UrlRegex = new Regex(Regex.Escape("{{url}}"));
        public void ShoutOut(string streamerName, bool prefersNative = true) {
            this.UserManager.FetchUserWithUserName(streamerName, (user) => {
                if (user == null) {
                    Logger.LogError($"Could not shout out {streamerName}");
                    return;
                }

                bool useNative = true;
                if (prefersNative == false) {
                    useNative = false;
                } else if ((this.LastNativeShoutout != null) && DateTime.Now.Subtract((DateTime)this.LastNativeShoutout).TotalSeconds < this.ShoutOutLimit) {
                    useNative = false;
                } else if (this.PastNativeShoutouts.ContainsKey(streamerName)) {
                    var pastShoutoutTime = this.PastNativeShoutouts[streamerName];
                    if (DateTime.Now.Subtract(pastShoutoutTime).TotalSeconds < this.PerStreamerShoutoutLimit) {
                        useNative = false;
                    }
                }

                if (useNative) {
                    var task = this.HelixAPI.Chat.SendShoutoutAsync(this.Connection.ChannelID, user.UserId, this.Connection.ChannelID);
                    TwitchManager.RunTask(task, () => {
                        Logger.LogInfo("Shout out sent");
                        this.LastNativeShoutout = DateTime.Now;
                        this.PastNativeShoutouts[streamerName] = DateTime.Now;
                    });
                } else {
                    var task = this.HelixAPI.Channels.GetChannelInformationAsync(user.UserId);
                    TwitchManager.RunTask(task, (channelInfo) => {
                        var userReplacedMsg = this.UserRegex.Replace(this.ShoutOutTemplate, user.DisplayName);
                        var categoryReplacedMsg = this.CategoryRegex.Replace(userReplacedMsg, channelInfo.Data[0].GameName);
                        var urlReplacedMsg = this.UrlRegex.Replace(categoryReplacedMsg, "https://twitch.tv/" + user.UserName);
                        this.SendChatMessage(urlReplacedMsg);
                    });
                    
                }
            });
        }
        #endregion


        /**************************************************
         * INTERNAL CODE. NO NEED TO READ BELOW THIS LINE *
         **************************************************/

        #region Internal
        private Dictionary<string, RegisteredChatCommand> RegisteredCommands = new Dictionary<string, RegisteredChatCommand>();

        override internal void InitializeClient(Client client) {
            Logger.LogInfo("Setting up Chat Command Manager");
            client.OnChatCommandReceived -= Client_OnChatCommandReceived;
            client.OnChatCommandReceived += Client_OnChatCommandReceived;
        }

        private void Client_OnChatCommandReceived(object sender, OnChatCommandReceivedArgs args) {
            var command = args.Command;
            if (this.RegisteredCommands.ContainsKey(command.CommandText) == false) {
                return;
            }

            var user = this.UserManager.UserForChatMessage(command.ChatMessage);

            var registeredCommand = this.RegisteredCommands[command.CommandText];
            if (user.IsPermitted(registeredCommand.Permissions) == false) {
                this.SendChatMessage("You don't have permission to use this command");
                return;
            }

            registeredCommand.Callback.Invoke(command.ArgumentsAsList, user);
        }
        #endregion

    }
}
