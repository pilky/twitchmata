using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TwitchLib.Unity;
using TwitchLib.PubSub.Events;
using TwitchLib.Api.Helix.Models.ChannelPoints.UpdateCustomRewardRedemptionStatus;
using TwitchLib.Api.Core.Enums;
using TwitchLib.Api;
using TwitchLib.Client.Models;
using TwitchLib.Client.Events;
using System.Threading.Tasks;
using TwitchLib.PubSub.Models.Responses;
using TwitchLib.Api.Auth;

namespace Twitchmata {
    public class ConnectionManager {
        public PubSub PubSub { get; private set; }
        public Api API { get; private set; }
        public Client Client { get; private set; }

        public ConnectionConfig ConnectionConfig { get; private set; }

        /// <summary>
        /// Connect to PubSub and Chat Bot
        /// </summary>
        public void Connect() {
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




        /**************************************************
         * INTERNAL CODE. NO NEED TO READ BELOW THIS LINE *
         **************************************************/

        internal UserManager UserManager { get; private set; }
        internal Secrets Secrets { get; private set; }

        internal ConnectionManager(ConnectionConfig connectionConfig, Secrets secrets) {
            this.ConnectionConfig = connectionConfig;
            this.Secrets = secrets;
            this.SetupAPIAndPubSub();
            this.SetupClient();
            this.UserManager = new UserManager(this);
            this.UserManager.FetchUserInfo();
        }

        private void SetupClient() {
            this.Client = new Client();
            this.Client.OnIncorrectLogin += Client_OnIncorrectLogin;
        }

        private void SetupAPIAndPubSub() {
            this.API = new Api();
            this.API.Settings.ClientId = this.ConnectionConfig.ClientID;
            this.API.Settings.AccessToken = this.Secrets.AccountAccessToken();

            this.PubSub = new PubSub();
            this.PubSub.OnListenResponse += PubSub_OnListenResponse;
            this.PubSub.OnPubSubServiceConnected += PubSub_OnPubSubServiceConnected;
            this.PubSub.OnPubSubServiceClosed += PubSub_OnPubSubServiceClosed;
            this.PubSub.OnPubSubServiceError += PubSub_OnPubSubServiceError;
        }


        #region Connection

        private void ConnectClient() {
            ConnectionCredentials credentials = new ConnectionCredentials(this.ConnectionConfig.BotName, this.Secrets.BotAccessToken());
            this.Client.Initialize(credentials, this.ConnectionConfig.ChannelName);
            foreach (FeatureManager manager in this.FeatureManagers) {
                manager.InitializeClient(this.Client);
            }
            this.Client.Connect();
        }

        #endregion

        #region Access Tokens
        private void RefreshBotAccessToken() {
            var refreshToken = this.Secrets.BotRefreshToken();
            var clientSecret = this.Secrets.ClientSecret();
            var task = Task.Run(() => API.Auth.RefreshAuthTokenAsync(refreshToken, clientSecret));
            task.Wait();
            var response = task.Result;
            this.Secrets.SetBotAccessToken(response.AccessToken);
            this.Secrets.SetBotRefreshToken(response.RefreshToken);
            this.SetupClient();
            this.ConnectClient();
        }

        private async Task<string> RefreshAccountAccessToken() {
            var refreshToken = this.Secrets.AccountRefreshToken();
            var clientSecret = this.Secrets.ClientSecret();
            var response = await API.Auth.RefreshAuthTokenAsync(refreshToken, clientSecret);
            this.Secrets.SetAccountAccessToken(response.AccessToken);
            this.Secrets.SetAccountRefreshToken(response.RefreshToken);
            return response.AccessToken;
        }
        #endregion


        #region Client Management

        private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs args) {
            Debug.Log("Updating Bot Token");
            this.RefreshBotAccessToken();
        }

        #endregion


        #region PubSub Management
        private void PubSub_OnListenResponse(object sender, OnListenResponseArgs args) {
            if (args.Successful == false) {
                Debug.Log("PubSub Error: " + args.Response.Error);
                if (args.Response.Error == "ERR_BADAUTH") {
                    if (this.IsResettingPubSub == false) {
                        this.ResetPubSub();
                    }
                }
            }
        }

        private bool IsResettingPubSub = false;

        private void ResetPubSub() {
            this.IsResettingPubSub = true;
            var completionSource = new TaskCompletionSource<string>();
            Task.Run(async () => {
                completionSource.SetResult(await this.RefreshAccountAccessToken());
            });

            completionSource.Task.ConfigureAwait(true).GetAwaiter().OnCompleted(() => {
                this.API.Settings.AccessToken = completionSource.Task.Result;
                this.PubSub.Disconnect();
                this.PubSub.Connect();
                foreach (FeatureManager manager in this.FeatureManagers) {
                    manager.InitializeWithAPIManager(this);
                }
                this.IsResettingPubSub = false;
            });
        }

        private void PubSub_OnPubSubServiceConnected(object sender, System.EventArgs args) {
            Debug.Log("Connected");
            this.PubSub.SendTopics(this.Secrets.AccountAccessToken());
        }

        private void PubSub_OnPubSubServiceClosed(object sender, System.EventArgs args) {
            Debug.Log("Closed");
        }

        private void PubSub_OnPubSubServiceError(object sender, OnPubSubServiceErrorArgs args) {
            Debug.Log("Pub Sub Error: " + args.Exception.Message);
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
        #endregion


        //TODO: move out of connection manager
        public async Task<string> GetAvatarURLForUser(string userID) {
            var user = new List<string> { userID };
            var users = await API.Helix.Users.GetUsersAsync(user);

            if (users == null || users.Users.Length == 0) {
                return "";
            }
            return users.Users[0].ProfileImageUrl;
        }
    }
}
