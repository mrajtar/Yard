using Yard.Models;

namespace Yard.Interfaces
{
    public interface ILeagueOfLegendsScraper
    {
        Task<LeagueOfLegendsPlayerStatistics> GetStatsAsync(string region, string username);
    }
}
