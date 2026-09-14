using System;
using System.Collections.Generic;
using System.Reflection;
using TwitchLib.EventSub.Core;
using Twitchmata.Adapters.Args;
using Twitchmata.Adapters.Handlers;

namespace Twitchmata.Adapters
{
    public class EventSubWebsocketClient : TwitchLib.EventSub.Websockets.EventSubWebsocketClient
    {
        public event AsyncEventHandler<PowerUpRedemptionArgs> PowerUpRedemption;

        public EventSubWebsocketClient(string websocketUrl = "wss://eventsub.wss.twitch.tv/ws") : base(websocketUrl)
        {
            // 1. Grab the private field metadata
            FieldInfo field = typeof(TwitchLib.EventSub.Websockets.EventSubWebsocketClient).GetField("_handlers", BindingFlags.NonPublic | BindingFlags.Instance);

            // 2. Extract the actual list instance from your object
            var list = (Dictionary<string, Action<TwitchLib.EventSub.Websockets.EventSubWebsocketClient, string>>)field.GetValue(this);
            var handler = new PowerUpRedemptionHandler();
            // 3. Directly modify the list
            list.Add(handler.SubscriptionType, handler.Handle);
        }
        public void InvokePowerUpRedemption(PowerUpRedemptionArgs args)
        {
            PowerUpRedemption?.Invoke(this, args);
        }
    }
}