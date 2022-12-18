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

        public PubSub pubSub { get; private set; }
        public Api api { get; private set; }
        public Client client { get; private set; }

        public ConnectionManager() {
            this.SetupAPIAndPubSub();
            this.SetupClient();
        }

        private void SetupClient() {
            this.client = new Client();
            this.client.OnIncorrectLogin += Client_OnIncorrectLogin;
        }

        private void SetupAPIAndPubSub() {
            this.api = new Api();
            this.api.Settings.ClientId = Secrets.ClientID();
            this.api.Settings.AccessToken = Secrets.AccountAccessToken();

            this.pubSub = new PubSub();
            this.pubSub.OnListenResponse += PubSub_OnListenResponse;
            this.pubSub.OnPubSubServiceConnected += PubSub_OnPubSubServiceConnected;
            this.pubSub.OnPubSubServiceClosed += PubSub_OnPubSubServiceClosed;
            this.pubSub.OnPubSubServiceError += PubSub_OnPubSubServiceError;
        }


        //MARK: - Connection
        /// <summary>
        /// Connect to PubSub and Chat Bot
        /// </summary>
        public void Connect() {
            this.pubSub.Connect();
            this.ConnectClient();
        }

        /// <summary>
        /// Disconnect from PubSub and Chat Bot
        /// </summary>
        public void Disconnect() {
            this.pubSub.Disconnect();
            this.client.Disconnect();
        }

        private void ConnectClient() {
            ConnectionCredentials credentials = new ConnectionCredentials("pilkybot", Secrets.BotAccessToken());
            this.client.Initialize(credentials, Config.channelName);
            foreach (FeatureManager manager in this.featureManagers) {
                manager.InitializeClient(this.client);
            }
            this.client.Connect();
        }

        //MARK: - Access Tokens
        public bool isAccessTokenValid = false;
        private void RefreshBotAccessToken() {
            var refreshToken = Secrets.BotRefreshToken();
            var clientSecret = Secrets.ClientSecret();
            var task = Task.Run(() => api.Auth.RefreshAuthTokenAsync(refreshToken, clientSecret));
            task.Wait();
            var response = task.Result;
            Secrets.SetBotAccessToken(response.AccessToken);
            Secrets.SetBotRefreshToken(response.RefreshToken);
            this.SetupClient();
            this.ConnectClient();
        }

        private async Task<string> RefreshAccountAccessToken() {
            var refreshToken = Secrets.AccountRefreshToken();
            var clientSecret = Secrets.ClientSecret();
            var response = await api.Auth.RefreshAuthTokenAsync(refreshToken, clientSecret);
            Secrets.SetAccountAccessToken(response.AccessToken);
            Secrets.SetAccountRefreshToken(response.RefreshToken);
            return response.AccessToken;
        }

        //MARK: - Client Basics

        private void Client_OnIncorrectLogin(object sender, OnIncorrectLoginArgs args) {
            Debug.Log("Updating Bot Token");
            this.RefreshBotAccessToken();
        }

        //MARK: - PubSub Basics
        private void PubSub_OnListenResponse(object sender, OnListenResponseArgs args) {
            if (args.Successful == false) {
                Debug.Log("PubSub Error: " + args.Response.Error);
                if (args.Response.Error == "ERR_BADAUTH") {
                    if (this.isResettingPubSub == false) {
                        this.ResetPubSub();
                    }
                }
            }
        }

        private bool isResettingPubSub = false;

        private void ResetPubSub() {
            this.isResettingPubSub = true;
            var completionSource = new TaskCompletionSource<string>();
            Task.Run(async () => {
                completionSource.SetResult(await this.RefreshAccountAccessToken());
            });

            completionSource.Task.ConfigureAwait(true).GetAwaiter().OnCompleted(() => {
                this.api.Settings.AccessToken = completionSource.Task.Result;
                this.pubSub.Disconnect();
                this.pubSub.Connect();
                foreach (FeatureManager manager in this.featureManagers) {
                    manager.InitializeWithAPIManager(this);
                }
                this.isResettingPubSub = false;
            });
        }

        private void PubSub_OnPubSubServiceConnected(object sender, System.EventArgs args) {
            Debug.Log("Connected");
            this.pubSub.SendTopics(Secrets.AccountAccessToken());
        }

        private void PubSub_OnPubSubServiceClosed(object sender, System.EventArgs args) {
            Debug.Log("Closed");
        }

        private void PubSub_OnPubSubServiceError(object sender, OnPubSubServiceErrorArgs args) {
            Debug.Log("Pub Sub Error: " + args.Exception.Message);
        }

        public List<FeatureManager> featureManagers { get; private set; } = new List<FeatureManager>();
        public void RegisterFeatureManager(FeatureManager manager) {
            this.featureManagers.Add(manager);
            manager.InitializeWithAPIManager(this);
        }


        //MARK: - Cached Info

        public async Task<string> GetAvatarURLForUser(string userID) {
            var user = new List<string> { userID };
            var users = await api.Helix.Users.GetUsersAsync(user);

            if (users == null || users.Users.Length == 0) {
                return "";
            }
            return users.Users[0].ProfileImageUrl;
        }
    }
}
