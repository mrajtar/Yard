using DSharpPlus;
using DSharpPlus.CommandsNext;
using Yard.Commands;
using Yard.config;

namespace Yard
{
    internal class Program
    {
        private static DiscordClient Client { get; set; }
        
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

            Client = new DiscordClient(discordConfig);
            var commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = [jsonReader.prefix],
                EnableMentionPrefix = true,
                EnableDefaultHelp = false
            });

            commands.RegisterCommands<LeagueOfLegendsCheckCommand>();
            commands.RegisterCommands<ValorantCheckCommand>();

            await Client.ConnectAsync();
            await Task.Delay(-1);

        }
    }
}
