namespace Twitchmata.Adapters.Models
{
    public sealed class CustomPowerUp
    {
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public int Bits { get; set; } = 0;

        public string Prompt { get; set; } = string.Empty;

    }
}