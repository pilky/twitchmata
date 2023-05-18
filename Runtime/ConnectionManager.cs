using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.PubSub.Events;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using System;

namespace Twitchmata {
    public class ConnectionManager {
        public PubSub PubSub { get; private set; }
        public Api API { get; private set; }
        public Client Client { get; private set; }

        public ConnectionConfig ConnectionConfig { get; private set; }

        public string ChannelID {
            get { return this.UserManager.BroadcasterID; }
        }

        public string BotID {
            get { return this.UserManager.BotID; }
        }

        /// <summary>
        /// Connect to PubSub and Chat Bot
        /// </summary>
        public void Connect() {
            if (this.ChannelID == null) {
                Logger.LogError("Channel ID not set, did you forget to call PerformSetup()?");
                return;
            }
            this.PubSub.Connect();
            this.ConnectClient();
        }

        /// <summary>
        /// Disconnect from PubSub and Chat Bot
        /// </summary>
        public void Disconnect() {
            this.PubSub.Disconnect();
            this.Client.Disconnect();
        }

        /// <summary>
        /// This must be called after initialising a connection manager but before calling any APIs or connecting
        /// </summary>
        /// <param name="callback">Action that is called when it is safe to use the ConnectionManager</param>
        public void PerformSetup(Action callback) {
            this.UserManager.PerformSetup(callback);
        }



        /**************************************************
         * INTERNAL CODE. NO NEED TO READ BELOW THIS LINE *
         **************************************************/

        internal UserManager UserManager { get; private set; }
        internal Persistence Secrets { get; private set; }

        internal ConnectionManager(ConnectionConfig connectionConfig, Persistence secrets) {
            this.ConnectionConfig = connectionConfig;
            this.Secrets = secrets;
            this.SetupAPIAndPubSub();
            this.SetupClient();
            this.UserManager = new UserManager(this);
        }

        private void SetupClient() {
            this.Client = new Client();
            this.Client.OnIncorrectLogin += Client_OnIncorrectLogin;
        }

        

        private void SetupAPIAndPubSub() {
            this.API = new Api();
            this.API.Settings.ClientId = this.ConnectionConfig.ClientID;
            this.API.Settings.AccessToken = this.Secrets.AccountAccessToken;

            this.PubSub = new PubSub();
            this.PubSub.OnListenResponse += PubSub_OnListenResponse;
            this.PubSub.OnPubSubServiceConnected += PubSub_OnPubSubServiceConnected;
            this.PubSub.OnPubSubServiceClosed += PubSub_OnPubSubServiceClosed;
            this.PubSub.OnPubSubServiceError += PubSub_OnPubSubServiceError;
        }


        #region Connection

        private void ConnectClient() {
            Logger.LogInfo("Connecting client: "+ this.ConnectionConfig.BotName);
            ConnectionCredentials credentials = new ConnectionCredentials(this.ConnectionConfig.BotName, this.Secrets.BotAccessToken);
            this.Client.Initialize(credentials, this.ConnectionConfig.ChannelName);
            foreach (FeatureManager manager in this.FeatureManagers) {
                manager.InitializeClient(this.Client);
            }
            this.Client.Connect();
        }

        #endregion


        #region Client Management

        private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs args) {
            Logger.LogError("Invalid bot login, need to re-authenticate");
        }

        #endregion


        #region PubSub Management
        private void PubSub_OnListenResponse(object sender, OnListenResponseArgs args) {
            if (args.Successful == false) {
                if (args.Response.Error == "ERR_BADAUTH") {
                    Debug.Log("Invalid account login, need to re-authenticate");
                } else {
                    Debug.LogError("PubSub Error: " + args.Response.Error);
                }
            }
        }

        private void PubSub_OnPubSubServiceConnected(object sender, System.EventArgs args) {
            Logger.LogInfo("PubSub Connected");
            this.PubSub.SendTopics(this.Secrets.AccountAccessToken);
        }

        private void PubSub_OnPubSubServiceClosed(object sender, System.EventArgs args) {
            Logger.LogInfo("PubSub Closed");
        }

        private void PubSub_OnPubSubServiceError(object sender, OnPubSubServiceErrorArgs args) {
            Logger.LogError("PubSub Error: " + args.Exception.Message);
        }

        internal void PubSub_SendTestMessage(string topicName, System.Object messageObject) {
            var jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(new {
                type = "MESSAGE",
                data = new {
                    topic = topicName,
                    message = messageObject
                }
            });

            this.PubSub.TestMessageParser(jsonString);
        }

        #endregion


        #region Feature Managers
        public List<FeatureManager> FeatureManagers { get; private set; } = new List<FeatureManager>();
        /// <summary>
        /// Register a feature manager with the connectino manager.
        /// </summary>
        /// <remarks>
        /// Usually this would be handled by TwitchManager but is provided in case you want to programtically register a feature manager
        /// </remarks>
        /// <param name="manager">The manager to register</param>
        public void RegisterFeatureManager(FeatureManager manager) {
            this.FeatureManagers.Add(manager);
            manager.InitializeWithAPIManager(this);
        }
        
        public void PerformPostDiscoverySetup() {
            var featureManagers = new List<FeatureManager>(this.FeatureManagers);
            foreach (var featureManager in featureManagers) {
                featureManager.PerformPostDiscoverySetup();
            }
        }
        #endregion
        
    }
}
