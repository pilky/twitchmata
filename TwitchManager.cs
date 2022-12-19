using UnityEngine;
using System.Collections;

namespace Twitchmata {
    public class TwitchManager : MonoBehaviour {
        public ConnectionManager connectionManager;
        private void Start() {
            if (this.SecretsPath == null || this.SecretsPath == "") {
                this.SecretsPath = Application.persistentDataPath;
            }
            this.Reset();
        }

        public void Reset() {
            if (this.connectionManager != null) {
                this.connectionManager.Disconnect();
            }

            this.connectionManager = new ConnectionManager(this.ConnectionConfig, new Secrets(this.SecretsPath));
            this.DiscoverFeatureManagers();
            this.connectionManager.Connect();
        }

        private void DiscoverFeatureManagers() {
            foreach (var manager in this.GetComponents<FeatureManager>()) {
                this.connectionManager.RegisterFeatureManager(manager);
            }

            foreach (var manager in this.GetComponentsInChildren<FeatureManager>()) {
                this.connectionManager.RegisterFeatureManager(manager);
            }
        }

        #region Config
        [Tooltip("Name and ID of your channel, and name of your chat bot")]
        public ConnectionConfig ConnectionConfig = new ConnectionConfig() {
            ChannelName = "",
            ChannelID = "",
            BotName = "",
        };
        [Tooltip("Location of secrets files on disk. Leave blank to use default.")]
        public string SecretsPath;
        #endregion
    }

    
}

