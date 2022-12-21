using UnityEngine;
using System.Collections;

namespace Twitchmata {
    public class TwitchManager : MonoBehaviour {
        public ConnectionManager ConnectionManager;
        private void Start() {
            if (this.SecretsPath == null || this.SecretsPath == "") {
                this.SecretsPath = Application.persistentDataPath;
            }
            this.Reset();
        }

        public void Reset() {
            if (this.ConnectionManager != null) {
                this.ConnectionManager.Disconnect();
            }

            this.ConnectionManager = new ConnectionManager(this.ConnectionConfig, new Secrets(this.SecretsPath));
            this.DiscoverFeatureManagers();
            this.ConnectionManager.Connect();
        }

        private void DiscoverFeatureManagers() {
            foreach (var manager in this.GetComponents<FeatureManager>()) {
                this.ConnectionManager.RegisterFeatureManager(manager);
            }

            foreach (var manager in this.GetComponentsInChildren<FeatureManager>()) {
                this.ConnectionManager.RegisterFeatureManager(manager);
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

