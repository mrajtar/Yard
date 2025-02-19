using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Yard.Models;

namespace Yard.Commands
{
    internal class ValorantCheckCommand : BaseCommandModule
    {
        [Command("vlr")]
        public async Task ValorantCommand(CommandContext ctx, [RemainingText] string username)
        {
            // switched over to selenium cause tracker is js generated, couldn't scrap with agilitypack
            string encodedUsername = Uri.EscapeDataString(username);
            var options = new ChromeOptions();
            options.AddArguments("--headless");
            options.AddArguments("--disable-blink-features=AutomationControlled");
            options.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");
            var driver = new ChromeDriver(options);
            try
            {
                await driver.Navigate().GoToUrlAsync($"https://tracker.gg/valorant/profile/riot/{encodedUsername}/overview");
                //File.WriteAllText("output.html", driver.PageSource);
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(6));

                try
                {
                    var errorNode = driver.FindElement(By.XPath("//div[contains(@class, 'content--error')]"));
                    if (errorNode != null)
                    {
                        await ctx.RespondAsync("Profile not found or private.");
                        return;
                    }
                }
                catch (NoSuchElementException)
                {
                
                }

                var parentNode = wait.Until(drv => drv.FindElement(By.XPath("//div[contains(@class, 'area-main-stats')]")));
                var stats = new ValorantPlayerStatistics();
                if (parentNode != null)
                {
                    var rankElement = parentNode.FindElements(By.XPath(".//div[contains(@class, 'stat')]"));
                    foreach (var stat in rankElement)
                    {
                        var labelElement = stat.FindElement(By.XPath(".//span[@class='stat__label']"));
                        var valueElement = stat.FindElement(By.XPath(".//span[@class='stat__value']"));

                        if (labelElement.Text.Trim() == "Rating")
                        {
                            stats.Rank = valueElement.Text.Trim();
                            break;
                        }

                        if (labelElement.Text.Trim() != "Radiant" && !labelElement.Text.Trim().Contains("Immortal"))
                            continue;
                        var subtextElement = stat.FindElements(By.XPath(".//span[@class='stat__subtext']"));
                        string subtext = subtextElement.Count > 0 ? $" ({subtextElement[0].Text.Trim()})" : string.Empty;
                        stats.Rank = $"{labelElement.Text.Trim()} {valueElement.Text.Trim()}{subtext}";
                        break;
                    }
                    
                    stats.WinPercentage = parentNode.FindElement(By.XPath(
                        "//div[contains(@class, 'numbers')][.//span[@class='name' and text()='Win %']]//span[@class='value']"))?.Text.Trim() ?? "N/A";
                    stats.KDRatio = parentNode.FindElement(By.XPath(
                        "//div[contains(@class, 'numbers')][.//span[@class='name' and text()='K/D Ratio']]//span[@class='value']"))?.Text.Trim() ?? "N/A";
                    stats.Wins = parentNode.FindElement(By.XPath(
                        ".//*[local-name()='text' and @text-anchor='middle' and @fill='#fff'][1]"))?.Text.Trim() ?? "N/A";
                    stats.Losses = parentNode.FindElement(By.XPath(
                        ".//*[local-name()='text' and @text-anchor='middle' and @fill='#fff'][2]"))?.Text.Trim() ?? "N/A";
                    stats.AverageDamagePerRound = parentNode.FindElement(By.XPath(
                        "//div[contains(@class, 'stat')][.//span[@class='name' and text()='Damage/Round']]//span[@class='value']"))?.Text.Trim() ?? "N/A";
                    stats.KAST = parentNode.FindElement(By.XPath(
                        "//div[contains(@class, 'numbers')][.//span[@class='name' and text()='KAST']]//span[@class='value']"))?.Text.Trim() ?? "N/A";
                    stats.HeadShotPercentage = parentNode.FindElement(By.XPath(
                        "//div[contains(@class, 'numbers')][.//span[@class='name' and text()='Headshot %']]//span[@class='value']"))?.Text.Trim() ?? "N/A";
                }
                else
                {
                    await Console.Out.WriteLineAsync("No parent node found.");
                }

                var embed = EmbedBuilderForValorant.BuildDiscordEmbed(username, stats);
                await ctx.RespondAsync(embed);
            }
            catch (NoSuchElementException)
            {
                await ctx.RespondAsync("Failed to retrieve stats. The profile might be private or invalid.");
            }
            finally
            {
                driver.Quit();
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
