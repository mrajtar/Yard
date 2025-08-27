using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Yard.Models;
using Yard.Services;

namespace Yard.Commands;

internal class LeagueOfLegendsCheckCommand : BaseCommandModule
{
    private readonly LeagueOfLegendsScraper _scraper;

    public LeagueOfLegendsCheckCommand(LeagueOfLegendsScraper scraper)
    {
        _scraper = scraper;
    }

    [Command("lol")]
    public async Task LeagueOfLegendsCommand(CommandContext ctx, string region, [RemainingText] string username)
    {
        string safeUsername;
        if (!InputValidatorForLeagueOfLegends.IsValidRegion(region))
        {
            await ctx.RespondAsync("Invalid region. Please enter a valid server (na, eune, euw, me, ru, kr, oce, jp, br, tr, las, lan, tw, sea, vn).");
            return;
        }

        if (InputValidatorForLeagueOfLegends.IsValidUsername(username))
        {
            safeUsername = username.Replace("#", "-");
        }
        else
        {
            await ctx.RespondAsync("Invalid format. Correct format looks like this:!lol region username#tag");
            return;
        }

        var stats = await _scraper.GetStatsAsync(region, safeUsername);


        if (stats is { Tier: "N/A", LeaguePoints: "N/A", WinsAndLosses: "N/A", WinPercentage: "N/A", Champions: null })
        {
            await ctx.RespondAsync($"No data found for summoner {username} in {region.ToUpper()}." +
                                   $" Check for typos in username/region or the user has not played any ranked games this split.");
            return;
        }

        var embed = EmbedBuilderForLeagueOfLegends.BuildDiscordEmbed(region, username, stats);
        await ctx.RespondAsync(embed);
    }
}

internal static class EmbedBuilderForLeagueOfLegends
{
    public static DiscordEmbed BuildDiscordEmbed(string region, string username, LeagueOfLegendsPlayerStatistics stats)
    {
        var embed = new DiscordEmbedBuilder
        {
            Title = $"Statistics for {username}",
            Description = $"Region: {region.ToUpper()}",
            Color = DiscordColor.Purple
        };
        embed.AddField("Tier", stats.Tier, true);
        embed.AddField("LP", stats.LeaguePoints, true);
        embed.AddField("Win %", stats.WinsAndLosses, true);
        embed.AddField("Win-Lose", stats.WinPercentage, true);
        if (stats.Champions.Count > 0)
            embed.AddField("Most played champions", string.Join(Environment.NewLine, stats.Champions));
        return embed.Build();
    }
}

internal static class InputValidatorForLeagueOfLegends
{
    private static readonly HashSet<string> ValidRegions =
        ["na", "eune", "euw", "me", "ru", "kr", "oce", "jp", "br", "tr", "las", "lan", "tw", "sea", "vn"];

    public static bool IsValidRegion(string region) => ValidRegions.Contains(region.ToLower());

    public static bool IsValidUsername(string username) => username.Contains('#');
}