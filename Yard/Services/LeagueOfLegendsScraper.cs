using HtmlAgilityPack;
using Yard.Interfaces;
using Yard.Models;

namespace Yard.Services
{
    public class LeagueOfLegendsScraper : ILeagueOfLegendsScraper
    {
        public async Task<LeagueOfLegendsPlayerStatistics> GetStatsAsync(string region, string username)
        {
            var safeUsername = username.Replace("#", "-");
            var encodedUsername = Uri.EscapeDataString(safeUsername);

            var web = new HtmlWeb
            {
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36"
            };

            var stats = new LeagueOfLegendsPlayerStatistics();
            try
            {
                var document = web.Load($"https://www.op.gg/summoners/{region}/{encodedUsername}");
                var parentNode = document.DocumentNode.SelectSingleNode("//div[@id='content-container']");
                if (parentNode == null)
                    await Console.Out.WriteLineAsync("Parent node not found.");

                stats.Tier = parentNode.SelectSingleNode(".//div[contains(@class, 'tier')]")?.InnerText.Trim() ?? "N/A";

                stats.LeaguePoints = parentNode.SelectSingleNode(".//div[contains(@class, 'lp')]")?.InnerText.Trim() ?? "N/A";

                stats.WinPercentage = parentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' win-lose ')]")?.InnerText.Trim() ?? "N/A";

                stats.WinsAndLosses = parentNode.SelectSingleNode(".//div[contains(@class, 'ratio')]")?.InnerText.Trim() ?? "N/A";

                var championBoxes = parentNode.SelectNodes(".//div[contains(@class, 'champion-box')]");
                if (championBoxes == null)
                    await Console.Out.WriteLineAsync("No champion box nodes found.");

                foreach (var box in championBoxes)
                {
                    var name = box.SelectSingleNode(".//div[contains(@class, 'name')]")?.InnerText.Trim() ?? "N/A";

                    var kda = box.SelectSingleNode(".//div[contains(@class, 'ere6j7v2')]")?.InnerText.Trim() ?? "N/A";

                    var gamesPlayed = box.SelectSingleNode(".//div[contains(@class, 'count')]")?.InnerText.Trim() ?? "N/A";

                    var winRate = box.SelectSingleNode(".//div[contains(@class, 'ere6j7v3')]")?.InnerText.Trim() ?? "N/A";

                    stats.Champions.Add($"{name} - {kda} - {gamesPlayed} - {winRate} WR");
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error fetching data: {ex.Message}");
            }
            return stats;
        }
    }
}
