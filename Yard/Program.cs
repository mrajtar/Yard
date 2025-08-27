using DSharpPlus;
using DSharpPlus.CommandsNext;
using Microsoft.Extensions.DependencyInjection;
using System;
using Yard.Commands;
using Yard.config;
using Yard.Services;

namespace Yard
{
    internal class Program
    {
        private static DiscordClient Client { get; set; }
        
        static async Task Main(string[] args)
        {
            var jsonReader = new JSONReader();
            var data = await jsonReader.ReadJSON();

            var services = new ServiceCollection();

            services.AddSingleton<LeagueOfLegendsScraper>();
            services.AddSingleton<ValorantScraper>();

            var serviceProvider = services.BuildServiceProvider();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = data.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordConfig);
            var commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = [data.prefix],
                EnableMentionPrefix = true,
                EnableDefaultHelp = false,
                Services = serviceProvider
            });

            commands.RegisterCommands<LeagueOfLegendsCheckCommand>();
            commands.RegisterCommands<ValorantCheckCommand>();

            await Client.ConnectAsync();
            await Task.Delay(-1);

        }
    }
}
