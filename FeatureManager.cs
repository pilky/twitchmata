using UnityEngine;
using System.Collections;
using TwitchLib.Unity;

namespace Twitchmata {

    public class FeatureManager : MonoBehaviour {
        public ConnectionManager manager;
        internal void InitializeWithAPIManager(ConnectionManager manager) {
            this.manager = manager;

            this.InitializeFeatureManager();
            this.InitializePubSub(manager.pubSub);
            this.InitializeClient(manager.client);
        }

        internal virtual void InitializePubSub(PubSub pubSub) {

        }

        internal virtual void InitializeClient(Client client) {

        }

        public virtual void InitializeFeatureManager() {

        }

        public string ChannelID {
            get { return this.manager.ConnectionConfig.ChannelID; }
        }

        public TwitchLib.Api.Helix.Helix HelixAPI {
            get { return this.manager.api.Helix; }
        }
    }

}