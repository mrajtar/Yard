namespace Yard.Models
{
    public class LeagueOfLegendsPlayerStatistics
    {
        public string? Tier { get; set; }
        public string? LeaguePoints { get; set; }
        public string? WinsAndLosses { get; set; }
        public string? WinPercentage { get; set; }
        public List<string> Champions { get; set; } = [];
    }
}
