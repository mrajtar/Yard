namespace Yard.Models
{
    public class ValorantPlayerStatistics
    {
        public string? Rank { get; set; } = "N/A";
        public string? KDRatio { get; set; } = "N/A";
        public string? AverageDamagePerRound { get; set; } = "N/A";
        public string? Wins { get; set; } = "N/A";
        public string? Losses { get; set; } = "N/A";
        public string? WinPercentage { get; set; } = "N/A";
        public string? HeadShotPercentage { get; set; } = "N/A";
        public string? KAST { get; set; } = "N/A";
        public List<string> Agents { get; set; } = [];
    }
}
