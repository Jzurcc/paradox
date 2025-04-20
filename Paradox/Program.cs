using Paradox;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paradox.Commands;

namespace Paradox
{
    class Program
    {
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }
        static async Task Main(string[] args)
        {
            //This basically is the template of to run the whole ahh bot but putting the token
            //there unprotected is bad practice
            JsonParser.JsonParseChecker();
            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = Paradox.Commands.JsonParser.DiscordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };
            Client = new DiscordClient(discordConfig);
            Client.Ready += Client_Ready;

            //This thingy is going to handle the commands
            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { "hey ", "!", "" },
                EnableDms = true,
                EnableMentionPrefix = true,
                DmHelp = true
            };

            //Prefix Commands Registration
            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<Commands.Commands>();

            //Connects to the server
            try
            {
                await Client.ConnectAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine("Token is invalid. Input the token again");
                File.Delete("config.json");
                JsonParser.JsonParseChecker();
            }

            await Task.Delay(-1);
        }

        private static Task Client_Ready(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }
    }
}
