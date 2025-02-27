namespace Yard.Models
{
    public class LeagueOfLegendsPlayerStatistics
    {
        public string? Tier { get; set; } = "N/A";
        public string? LeaguePoints { get; set; } = "N/A";
        public string? WinsAndLosses { get; set; } = "N/A";
        public string? WinPercentage { get; set; } = "N/A";
        public List<string> Champions { get; set; } = [];
    }
}
