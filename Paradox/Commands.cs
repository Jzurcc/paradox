using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using GenerativeAI;
using GenerativeAI.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Paradox.Commands
{
    public class Commands : BaseCommandModule
    {
        // preload GoogleAI
        static GoogleAi googleAI = new GoogleAi(JsonParser.GeminiAPI);
        static GenerativeModel model = googleAI.CreateGenerativeModel("gemini-2.0-flash-lite");
        
        static List<Content> history = 
        [
            new Content("You are a multiverse RPG system.", Roles.User), 
            new Content(model.GenerateContentAsync("Current Playthrough: Create an absurdly specific multiverse with 3 tiers of items, enemies, and NPCs, and unique gameplay mechanics and goals. The goal is the same in each universe: to find paradoxes (anomalies that may seem normal but don't belong in that universe). Of course, the players should still first finish quests and level up as these paradoxes are elusive. Also create a character sheet for the player. The starter stats is a randomized RPG archetype.").ToString(), Roles.Model),
            new Content(model.GenerateContentAsync("Write the plot outline (must have prologue, act 1, act 2, and finale), ALL early game quests, enemy and their types, NPCs, a starting quest that includdes a tutorial for battle, gathering items, and leveling up stats. Also flesh out the world building and add absurdly specific interesting gameplay details (e.g., battle systems, limitations, different nature & beliefs, etc.").ToString(), Roles.Model)
        ];
        ChatSession chatSession = model.StartChat(history:history);

        [Command("Generate")]
        public async Task Generate(CommandContext ctx, [RemainingText] string input)
        {
            //var response = await chatSession.GenerateContentAsync(input);
            var response = await chatSession.GenerateContentAsync(input);
            await ctx.RespondAsync(response.Text());
        }

        [Command("Hug")]
        public async Task Hug(CommandContext ctx)
        {
            HttpClient client = new HttpClient();
            var media = await client.GetAsync("https://api.waifu.pics/sfw/hug");
            var mediaString = await media.Content.ReadAsStringAsync();
            var mediaUrl = JObject.Parse(mediaString)["url"].ToString();
            var embed = new DiscordEmbedBuilder()
            {
                Title = "*Hugs You",
                ImageUrl = mediaUrl,
                Color = new DiscordColor(0x00ff00)
            };

            await ctx.RespondAsync(embed);
        }
        [Command("Start")]
        public async Task Start(CommandContext ctx)
        {
            var response = await chatSession.GenerateContentAsync("Welcome the player and act like a title menu. Format:\n[Title]\n[New Game]\n[Profile]");
            await ctx.RespondAsync(response.Text());
        }

        [Command("New")]
        public async Task NewGame(CommandContext ctx)
        {
            var response = await chatSession.GenerateContentAsync("Introduce the created playthrough universe through a well-written novel style but still readable RPG narrative, then present an action at the end.");
            await ctx.RespondAsync(response.Text());
        }

        [Command("Inventory")]
        public async Task Inventory(CommandContext ctx)
        {
            var response = await chatSession.GenerateContentAsync("Create a well-designed text inventory system with random starter loot (multiverse style)");
            await ctx.RespondAsync(response.Text());
        }

        [Command("!")]
        public async Task Act(CommandContext ctx, [RemainingText] string input)
        { 
            var response = await chatSession.GenerateContentAsync("Take note of your last prompts and the message in this last part will be the response. Only accept reasonable actions within the game and ask them to repeat if it's not. The success of a game action is dependent on ratio on dice rolls that can either be definite, d2, d6, d12, etc. depending on complexity that you roll in the background and tell to the player. If action doesn't fall under this category, ask them again. Tell some optional choices (but other choices can be made) and a description of the choice and context. Write like a novel-style narrative and be creative in your writing depending on universe genre, and be descriptive, show rather than tell.  User Message: " + input);
            await ctx.RespondAsync(response.Text());
        }

    }

    public class JsonParser
    {
        static string json;
        public static string GeminiAPI;
        public static string DiscordToken;

        public static void JsonParseChecker()
        {
            //checks if path exists
            if (File.Exists("config.json"))
            {
                json = File.ReadAllText("config.json");
                GeminiAPI = JObject.Parse(json)["GeminiAPI"].ToString();
                DiscordToken = JObject.Parse(json)["DiscordToken"].ToString();

            }

            else
            {
                ConfigCreator creator = new ConfigCreator();
                Console.WriteLine("Please Enter Discord Token: ");
                creator.DiscordToken = (Console.ReadLine());
                Console.WriteLine("Please Enter GeminiAPI: ");
                creator.GeminiAPI = (Console.ReadLine());
                var ConfigCreatorSerialized = JsonConvert.SerializeObject(creator);
                File.WriteAllText("config.json", ConfigCreatorSerialized);
                JsonParseChecker();
            }
        }
    }

    public class ConfigCreator
    {
        public string DiscordToken;
        public string GeminiAPI;
        public string UserID;
        public string Response;
    }
}
