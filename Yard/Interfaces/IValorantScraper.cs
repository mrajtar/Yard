using Yard.Models;

namespace Yard.Interfaces
{
    public interface IValorantScraper
    {
        public Task<ValorantPlayerStatistics> GetStatsAsync(string username);
    }
}
