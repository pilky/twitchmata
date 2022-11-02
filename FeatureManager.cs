using UnityEngine;
using System.Collections;
using TwitchLib.Unity;

namespace Twitchmata {

    public class FeatureManager : MonoBehaviour {
        public TwitchManager manager;
        public void InitializeWithAPIManager(TwitchManager manager) {
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