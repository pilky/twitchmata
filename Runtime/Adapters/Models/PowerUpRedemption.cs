using Newtonsoft.Json;

namespace Twitchmata.Adapters.Models
{
    public sealed class PowerUpRedemption
    {
        public string Id { get; set; } = string.Empty;

        [JsonProperty("broadcaster_user_id")]
        public string BroadcasterUserId { get; set; } = string.Empty;

        [JsonProperty("broadcaster_user_name")]
        public string BroadcasterUserName { get; set; } = string.Empty;

        [JsonProperty("user_id")]
        public string UserId { get; set; } = string.Empty;

        [JsonProperty("user_login")]
        public string UserLogin { get; set; } = string.Empty;

        [JsonProperty("user_name")]
        public string UserName { get; set; } = string.Empty;

        [JsonProperty("user_input")]
        public string UserInput { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;

        [JsonProperty("custom_power_up")]
        public CustomPowerUp CustomPowerUp { get; set; } = null;
    }
}