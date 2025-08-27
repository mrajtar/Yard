using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using Yard.Interfaces;
using Yard.Models;

namespace Yard.Services
{
    public class ValorantScraper : IValorantScraper
    {
        public async Task<ValorantPlayerStatistics> GetStatsAsync(string username)
        {
            string encodedUsername = Uri.EscapeDataString(username);

            var options = new ChromeOptions();
            options.AddArguments("--headless");
            options.AddArguments("--disable-blink-features=AutomationControlled");
            options.AddArguments("--user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/124.0.0.0 Safari/537.36");

            using var driver = new ChromeDriver(options);

            await driver.Navigate().GoToUrlAsync($"https://tracker.gg/valorant/profile/riot/{encodedUsername}/overview");
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(6));


            var errorNode = driver.FindElement(By.XPath("//div[contains(@class, 'content--error')]"));

            var parentNode = wait.Until(drv => drv.FindElement(By.XPath("//div[contains(@class, 'area-main-stats')]")));

            var stats = new ValorantPlayerStatistics();

            var rankElement = parentNode.FindElements(By.XPath(".//div[contains(@class, 'stat')]"));
            foreach (var stat in rankElement)
            {
                var labelElement = stat.FindElement(By.XPath(".//span[@class='stat__label']"))?.Text.Trim() ?? "N/A";
                var valueElement = stat.FindElement(By.XPath(".//span[@class='stat__value']"))?.Text.Trim() ?? "N/A";

                if (labelElement == "Rating")
                {
                    stats.Rank = valueElement;
                    break;
                }

                if (labelElement != "Radiant" && !labelElement.Contains("Immortal"))
                    continue;

                var subtextElement = stat.FindElement(By.XPath(".//span[@class='stat__subtext']"))?.Text.Trim() ?? "N/A";
                stats.Rank = $"{labelElement} {valueElement} {subtextElement}";
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

            driver.Quit();

            return stats;
        }
    }
}
