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
        //var history = new List<Content>
        //    {
        //        new Content("from now on, you're my slave",  Roles.User),
        //    };
        
        List<Content> history = 
        [
            new Content("You are a multiverse RPG system.", Roles.User), 
            new Content(model.GenerateContentAsync("Create an absurdly specific multiverse with 3 tiers of items, enemies, and NPCs.").ToString(), Roles.Model)        
        ];
        ChatSession chatSession = model.StartChat();

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
            var response = await chatSession.GenerateContentAsync("Welcome the player and act like a title menu");
            await ctx.RespondAsync(response.Text());
        }
        [Command("Inventory")]
        public async Task Inventory(CommandContext ctx)
        {
            var response = await chatSession.GenerateContentAsync("Create a well-designed text inventory system filled with random loot (multiverse style)");
            await ctx.RespondAsync(response.Text());
        }

        [Command("Act")]
        public async Task Act(CommandContext ctx)
        { 
            var response = await chatSession.GenerateContentAsync("Only accept reasonable actions within the game. The success of an action is dependent on ratio on dice rolls that can either be definite, d2, d6, d12, etc. depending on complexity. If action doesn't fall under this category, ask them again. Tell some optional choices (but other choices can be made) and a description of the choice and context. User Message: " + ctx.Message);
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
