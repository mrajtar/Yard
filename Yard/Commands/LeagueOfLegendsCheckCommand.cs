using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using HtmlAgilityPack;

namespace Yard.Commands
{
    public class LeagueOfLegendsCheckCommand : BaseCommandModule
    {
        [Command("lolcheck")]
        public async Task LeagueOfLegendsCommand(CommandContext ctx, string region, [RemainingText] string username)
        {
            string safeUsername;


            if (username.Contains("#"))
            {
                safeUsername = username.Replace("#", "-");
            }
            else
            {
                await ctx.RespondAsync("Invalid username format. Correct format looks like this:!lolcheck region username#riottag");
                return;
            }
            string encodedUsername = Uri.EscapeDataString(safeUsername);
            var web = new HtmlWeb();
            web.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36";

            string tier = "N/A";
            string lp = "N/A";
            string winLose = "N/A";
            string ratio = "N/A";
            string name = "N/A";
            string kda = "N/A";
            string gamesPlayed = "N/A";
            var champions = new List<string>();

            var document = web.Load($"https://www.op.gg/summoners/{region}/{encodedUsername}");
            var parentNode = document.DocumentNode.SelectSingleNode("//div[@id='content-container']");
            if (parentNode != null)
            {
                var tierNode = parentNode.SelectSingleNode(".//div[contains(@class, 'tier')]");
                tier = tierNode?.InnerText.Trim() ?? "N/A";

                var lpNode = parentNode.SelectSingleNode(".//div[contains(@class, 'lp')]");
                lp = lpNode?.InnerText.Trim() ?? "N/A";

                var winLoseNode = parentNode.SelectSingleNode(".//div[contains(concat(' ', @class, ' '), ' win-lose ')]");
                winLose = winLoseNode?.InnerText.Trim() ?? "N/A";

                var ratioNode = parentNode.SelectSingleNode(".//div[contains(@class, 'ratio')]");
                ratio = ratioNode?.InnerText.Trim() ?? "N/A";

                var championBoxes = parentNode?.SelectNodes(".//div[contains(@class, 'champion-box')]");
                if (championBoxes != null)
                {
                    foreach (var box in championBoxes)
                    {
                        var nameNode = box.SelectSingleNode(".//div[contains(@class, 'name')]");
                        name = nameNode?.InnerText.Trim() ?? "N/A";

                        var kdaNode = box.SelectSingleNode(".//div[contains(@class, 'kda')]");
                        kda = kdaNode?.InnerText.Trim() ?? "N/A";

                        var gamesPlayedNode = box.SelectSingleNode(".//div[contains(@class, 'played')]");
                        gamesPlayed = gamesPlayedNode?.InnerText.Trim() ?? "N/A";

                        champions.Add($"{name}: KDA - {kda}, Played - {gamesPlayed}");
                    }
                }
                else
                {
                    Console.WriteLine("No championbox nodes found.");
                }
            }
            else
            {
                Console.WriteLine("Parent node not found.");
            }
            await ctx.RespondAsync($"Current statistics for {encodedUsername}:\n" +
                $"- Tier: {tier}\n" +
                $"- LP: {lp}\n" +
                $"- Win-Lose: {winLose}\n" +
                $"- Ratio: {ratio}\n" +
                $"- Champions: {champions}");
                
        }
    }
}
