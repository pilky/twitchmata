using UnityEngine;
using System.Collections;
using TwitchLib.Unity;

namespace Twitchmata {

    public class FeatureManager : MonoBehaviour {
        public ConnectionManager manager;
        public void InitializeWithAPIManager(ConnectionManager manager) {
            this.manager = manager;

            this.InitializePubSub(manager.pubSub);
            this.InitializeClient(manager.client);
        }

        public virtual void InitializePubSub(PubSub pubSub) {

        }

        public virtual void InitializeClient(Client client) {

        }
    }

}