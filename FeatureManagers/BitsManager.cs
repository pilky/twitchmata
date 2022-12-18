using System.Collections;
using System.Collections.Generic;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;
using TwitchLib.Unity;
using UnityEngine;

namespace Twitchmata {
    public class BitsManager : FeatureManager {
        public override void InitializePubSub(PubSub pubSub) {
            pubSub.OnBitsReceivedV2 -= OnBitsReceived;
            pubSub.OnBitsReceivedV2 += OnBitsReceived;
            pubSub.ListenToBitsEventsV2(Config.channelID);
        }

        private void OnBitsReceived(object sender, OnBitsReceivedV2Args args) {
            this.ReceivedBits(args);
        }

        #region Notifications

        /// <summary>
        /// Fired when a user gives you bits. 
        /// </summary>
        /// <param name="bitsInfo">Info on the bits received</param>
        public virtual void ReceivedBits(OnBitsReceivedV2Args bitsInfo) {
            Debug.Log($"Received {bitsInfo.BitsUsed} from {bitsInfo.UserName}");
        }

        #endregion
    }
}