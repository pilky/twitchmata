using TwitchLib.EventSub.Websockets.Core.EventArgs;
using TwitchLib.EventSub.Websockets.Core.Models;
using Twitchmata.Adapters.Models;


namespace Twitchmata.Adapters.Args
{
    public class PowerUpRedemptionArgs : TwitchLibEventSubEventArgs<EventSubNotification<PowerUpRedemption>>
    {
    }
}