using UnityEngine;
using System.Collections;
using TwitchLib.Unity;

namespace Twitchmata {

    public class FeatureManager : MonoBehaviour {
        public ConnectionManager Manager;

        public virtual void InitializeFeatureManager() {

        }


        #region Convenience Properties
        public string ChannelID {
            get { return this.Manager.ConnectionConfig.ChannelID; }
        }

        public TwitchLib.Api.Helix.Helix HelixAPI {
            get { return this.Manager.API.Helix; }
        }
        #endregion


        #region Internal Initialization
        internal void InitializeWithAPIManager(ConnectionManager manager)
        {
            this.Manager = manager;

            this.InitializeFeatureManager();
            this.InitializePubSub(manager.PubSub);
            this.InitializeClient(manager.Client);
        }

        internal virtual void InitializePubSub(PubSub pubSub)
        {

        }

        internal virtual void InitializeClient(Client client)
        {

        }
        #endregion
    }

}