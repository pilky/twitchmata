using Newtonsoft.Json;
using System;
using TwitchLib.EventSub.Core.SubscriptionTypes.Channel;
using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.EventArgs.Channel;
using TwitchLib.EventSub.Websockets.Core.Handler;
using TwitchLib.EventSub.Websockets.Core.Models;
using Twitchmata.Adapters.Args;
using Twitchmata.Adapters.Models;

namespace Twitchmata.Adapters.Handlers
{
    public class PowerUpRedemptionHandler : INotificationHandler
    {
        public string SubscriptionType => "channel.custom_power_up_redemption.add";

        public void Handle(TwitchLib.EventSub.Websockets.EventSubWebsocketClient client, string jsonString)
        {
            EventSubNotification<PowerUpRedemption> eventSubNotification = JsonConvert.DeserializeObject<EventSubNotification<PowerUpRedemption>>(jsonString);
            if (eventSubNotification == null)
            {
                throw new InvalidOperationException("Parsed JSON cannot be null!");
            }
            (client as Twitchmata.Adapters.EventSubWebsocketClient).InvokePowerUpRedemption(new PowerUpRedemptionArgs { Notification = eventSubNotification });
        }
    }
}