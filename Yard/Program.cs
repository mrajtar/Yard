using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Yard.config;

namespace Yard
{
    internal class Program
    {
        private static DiscordClient client { get; set; }
        private static CommandsNextExtension commands { get; set; }
        
        static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            client = new DiscordClient(discordConfig);

            await client.ConnectAsync();
            await Task.Delay(-1);

        }
    }
}
