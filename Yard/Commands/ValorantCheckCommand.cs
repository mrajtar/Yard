using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OpenQA.Selenium;
using Yard.Models;
using Yard.Services;

namespace Yard.Commands
{
    internal class ValorantCheckCommand : BaseCommandModule
    {
        private readonly ValorantScraper _scraper;

        public ValorantCheckCommand(ValorantScraper scraper)
        {
            _scraper = scraper;
        }

        [Command("vlr")]
        public async Task ValorantCommand(CommandContext ctx, [RemainingText] string username)
        {
            // switched over to selenium cause tracker is js generated, couldn't scrap with agilitypack
            try
            {
                var stats = await _scraper.GetStatsAsync(username);

                if (stats == null)
                {
                    await ctx.RespondAsync("Profile not found or private.");
                    return;
                }
                
                var embed = EmbedBuilderForValorant.BuildDiscordEmbed(username, stats);
                await ctx.RespondAsync(embed);
            }
            catch (Exception ex)
            {
                await ctx.RespondAsync("Failed to retrieve stats. The profile might be private or invalid.");
                await Console.Out.WriteLineAsync($"Method failed, exception details: {ex}");
            }
            
        }
    }

    internal static class EmbedBuilderForValorant
    {
        public static DiscordEmbed BuildDiscordEmbed(string username, ValorantPlayerStatistics stats)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Statistics for {username}",
                Color = DiscordColor.Purple
            };
            embed.AddField("Rank", stats.Rank, true);
            embed.AddField("Win %", stats.WinPercentage, true);
            embed.AddField("K/D", stats.KDRatio, true);
            embed.AddField("Wins", stats.Wins, true);
            embed.AddField("Losses", stats.Losses, true);
            embed.AddField("ADR", stats.AverageDamagePerRound, true);
            embed.AddField("KAST", stats.KAST, true);
            embed.AddField("HS %", stats.HeadShotPercentage);
            return embed.Build();
        }
    }
}
