using UnityEngine;
using System.Collections;

namespace Twitchmata {
    public class TwitchManager : MonoBehaviour {
        public ConnectionManager connectionManager;
        private void Start() {
            this.Reset();
        }

        public void Reset() {
            if (this.connectionManager != null) {
                this.connectionManager.Disconnect();
            }

            this.connectionManager = new ConnectionManager();
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
    }
}

