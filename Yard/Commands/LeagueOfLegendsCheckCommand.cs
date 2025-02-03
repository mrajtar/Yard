using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using HtmlAgilityPack;
using Yard.Models;

namespace Yard.Commands;

internal class LeagueOfLegendsCheckCommand : BaseCommandModule
{
    private static readonly List<string> ValidServers =
        ["na", "eune", "euw", "me", "ru", "kr", "oce", "jp", "br", "tr", "las", "lan", "tw", "sea", "vn"];

    [Command("lol")]
    public async Task LeagueOfLegendsCommand(CommandContext ctx, string region, [RemainingText] string username)
    {
        string safeUsername;
        if (!ValidServers.Contains(region.ToLower()))
        {
            await ctx.RespondAsync("Invalid region. Please enter a valid server (na, eune, euw, me, ru, kr, oce, jp, br, tr, las, lan, tw, sea, vn).");
            return;
        }

        if (username.Contains('#'))
        {
            safeUsername = username.Replace("#", "-");
        }
        else
        {
            await ctx.RespondAsync("Invalid format. Correct format looks like this:!lol region username#riottag");
            return;
        }

        var encodedUsername = Uri.EscapeDataString(safeUsername);
        var web = new HtmlWeb();
        web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

        var stats = new LeagueOfLegendsPlayerStatistics();
        try
        {
            var document = web.Load($"https://www.op.gg/summoners/{region}/{encodedUsername}");
            var parentNode = document.DocumentNode.SelectSingleNode("//div[@id='content-container']");
            if (parentNode != null)
            {
                stats.Tier = parentNode.SelectSingleNode(".//div[contains(@class, 'tier')]")?.InnerText.Trim() ??"N/A";

                stats.LeaguePoints =parentNode.SelectSingleNode(".//div[contains(@class, 'lp')]")?.InnerText.Trim() ?? "N/A";

                stats.WinPercentage = parentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' win-lose ')]")?.InnerText.Trim() ?? "N/A";

                stats.WinsAndLosses = parentNode.SelectSingleNode(".//div[contains(@class, 'ratio')]")?.InnerText.Trim() ?? "N/A";

                var championBoxes = parentNode.SelectNodes(".//div[contains(@class, 'champion-box')]");
                if (championBoxes != null)
                    foreach (var box in championBoxes)
                    {
                        var name = box.SelectSingleNode(".//div[contains(@class, 'name')]")?.InnerText.Trim() ?? "N/A";

                        var kda = box.SelectSingleNode(".//div[contains(@class, 'ere6j7v2')]")?.InnerText.Trim() ?? "N/A";

                        var gamesPlayed = box.SelectSingleNode(".//div[contains(@class, 'count')]")?.InnerText.Trim() ?? "N/A";

                        var winRate = box.SelectSingleNode(".//div[contains(@class, 'ere6j7v3')]")?.InnerText.Trim() ?? "N/A";

                        stats.Champions.Add($"{name} - {kda} - {gamesPlayed} - {winRate} WR");
                    }
                else
                    await Console.Out.WriteLineAsync("No champion box nodes found.");
            }

            else
            {
                await Console.Out.WriteLineAsync("Parent node not found.");
            }
        }
        catch (Exception ex)
        {
            await Console.Out.WriteLineAsync($"Error fetching data: {ex.Message}");
        }

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
        if (stats.Champions.Count != 0)
            embed.AddField("Most played champions", string.Join(Environment.NewLine, stats.Champions));

        if (stats.Tier == "N/A" && stats is
                { LeaguePoints: "N/A", WinsAndLosses: "N/A", WinPercentage: "N/A", Champions: null })
        {
            await ctx.RespondAsync($"No data found for summoner {username} in {region.ToUpper()}." +
                                   $" Check for typos in username/region or the user has not played any ranked games this split.");
            return;
        }

        await ctx.RespondAsync(embed);
    }
}