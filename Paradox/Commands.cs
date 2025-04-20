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
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Paradox.Commands
{
    public class Commands : BaseCommandModule
    {
        // preload GoogleAI
        static GoogleAi googleAI = new GoogleAi(JsonParser.GeminiAPI);
        public static GenerativeModel model = googleAI.CreateGenerativeModel("gemini-2.0-flash-lite");
        
        MessageSplitter messageSplitter = new MessageSplitter();

        public static List<Content> history = 
        [
            new Content("You are a multiverse RPG system.", Roles.User), 
            new Content(model.GenerateContentAsync("Current Playthrough: Create either a medieval fantasy, cowboy, modern day, futuristic, apocalyptic, or utopian RPG multiverse world with 3 tiers of items, enemies, and NPCs, and unique gameplay mechanics and goals. Although each playthrough is different, they share the same endgame goal that is to find paradoxes (anomalies that may seem normal but don't belong in that universe). Of course, the players should still first finish quests and level up as these paradoxes are elusive. Also create a character sheet for the player. The starter stats is a randomized RPG archetype. Although it's a multiverse setting, each playthrough should read like just a normal RPG archetype.").ToString(), Roles.Model),
            new Content(model.GenerateContentAsync("Write the plot outline (must have prologue, act 1, act 2, and finale), ALL early game quests, enemy and their types, NPCs, a starting quest that includdes a tutorial for battle, gathering items, and leveling up stats. Also flesh out the world building and add  specific gameplay details (e.g., battle systems, limitations, different nature & beliefs, etc. Increase spawn rate of NPCs and enemies by a lot so there's a lot of interactibles, and also the amount of environments and places they can go, as well as quests. Just format these in bullet points. Then write the prologue. This should be detailed. Cater the multiverse to serious gameplay first, not goofy ahh multiverses. But do always remember that this is still an RPG.").ToString(), Roles.Model),
            new Content("Use discord markup but only sparingly, with # for headers and > for quotes and don't enclose ALL responses with (```)", Roles.User)
        ];
        public static ChatSession chatSession = model.StartChat(history:history);


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
            List<Content> history =
            [
                new Content("Always remember to use Discord Markups, especially > for dialogues and #, ##, and ### for headers and asterisks for bold and italics, and etc. etc.", Roles.User),
                new Content("You are a multiverse RPG system.", Roles.User),
                new Content(model.GenerateContentAsync("Current Playthrough: Create either a medieval fantasy, cowboy, modern day, futuristic, apocalyptic, or utopian RPG multiverse world with 3 tiers of items, enemies, and NPCs, and unique gameplay mechanics and goals. Although each playthrough is different, they share the same endgame goal that is to find paradoxes (anomalies that may seem normal but don't belong in that universe). Of course, the players should still first finish quests and level up as these paradoxes are elusive. Also create a character sheet for the player. The starter stats is a randomized RPG archetype. Although it's a multiverse setting, each playthrough should read like just a normal RPG archetype. Do not tell them that it's a multiverse RPG.").ToString(), Roles.Model),
                new Content(model.GenerateContentAsync("Write the plot outline (must have prologue, act 1, act 2, and finale), ALL early game quests, enemy and their types, NPCs, a starting quest that includdes a tutorial for battle, gathering items, and leveling up stats. Also flesh out the world building and add  specific gameplay details (e.g., battle systems, limitations, different nature & beliefs, etc. Increase spawn rate of NPCs and enemies by a lot so there's a lot of interactibles, and also the amount of environments and places they can go, as well as quests. Just format these in bullet points. Then write the prologue. This should be detailed. Cater the multiverse to serious gameplay first, not goofy ahh multiverses. But do always remember that this is still an RPG.").ToString(), Roles.Model)
            ];

            chatSession = model.StartChat(history: history);

            var response = await chatSession.GenerateContentAsync("use Discord Markups, especially > for dialogues and #, ##, and ### for headers and asterisks for bold and italics, and etc. Welcome the player and act like a title menu. The title should be related to the playthrough universe. Format:\n[Title]\n[New Game]\n[Profile]\n[Statistics]\n[Exit].");
            messageSplitter.ChunkRespond(ctx, response.Text());
        }

        [Command("New")]
        public async Task NewGame(CommandContext ctx)
        {
            await ctx.RespondAsync("# Generating the game... Please hold tight.");
            var prompt = "Start the game. Write an introductory novel-style paragraphs and exposition that puts the player in action with the world, and sets them up with possible starter quests, but do this subconciously and seamlessly, not just 'oh here's a quest'. Use '[newline] > [text]' for the dialogues of characters. Let the user type out the actions, don't present ABCD, still add suggestions, but it should be typed by them instead.";
            messageSplitter.ChunkGenerate(ctx, prompt);
        }

        [Command("Inventory")]
        public async Task Inventory(CommandContext ctx)
        {
            //Fetch the items-template.txt and uses that shi to cook up le item list. cool right?
            var response = await chatSession.GenerateContentAsync($"Follow the template strictly. If it's a new game, create a text inventory system with random starter loot (multiverse style). Don't enclose the template in (```) and when the description reaches 30 characters, continue it on a new line. use the template provided: {Templates.Templates.ItemsTemplate()}");
            messageSplitter.ChunkRespond(ctx, response.Text());
        }

        [Command("act")]
        public async Task Act(CommandContext ctx, [RemainingText] string input)
        { 
            var response = await chatSession.GenerateContentAsync("The user message is an action. use Discord Markups, especially > for dialogues and #, ##, and ### for headers and asterisks for bold and italics, and etc. Take note of your last prompts and the message in this last part will be the response. Only accept reasonable actions within the game and ask them to repeat if it's not. The success of a game action is dependent on ratio on dice rolls that can either be definite, d2, d6, d12, etc. depending on complexity that you roll in the background and tell to the player. If action doesn't fall under this category, ask them again. Tell some optional choices (but other choices can be made) and a description of the choice and context. Write like a novel-style narrative and be creative in your writing depending on universe genre, and be descriptive, show rather than tell.   Also, let the user type out the actions, don't present ABCD, still add suggestions, but it should be typed by them instead. User Message: " + input);
            messageSplitter.ChunkRespond(ctx, response.Text());
        }

        [Command("say")]
        public async Task Say(CommandContext ctx, [RemainingText] string input)
        {
            var response = await chatSession.GenerateContentAsync("The user message is a character dialogue. use Discord Markups, especially > for dialogues and #, ##, and ### for headers and asterisks for bold and italics, and etc. If user dialogue contains possible actions from NPCs or the world, roll a 1d6 (or higher depending on complexity) to see if it succeeds. Write only the response of the world and people around the player, their reactions, etc. Write like a novel-style narrative and be creative in your writing depending on universe genre, and be descriptive, show rather than tell.  User Message: " + input);
            messageSplitter.ChunkRespond(ctx, response.Text());
        }

        [Command("use")]
        public async Task Use(CommandContext ctx, [RemainingText] string input)
        {
            var response = await chatSession.GenerateContentAsync("The user message is a use action. use Discord Markups, especially > for dialogues and #, ##, and ### for headers and asterisks for bold and italics, and etc. This lets the user use an item in the environment or from their inventory. If user actions contains possible actions from NPCs or the world, roll a 1d6 (or higher depending on complexity) to see if it succeeds. Write like a novel-style narrative and be creative in your writing depending on universe genre, and be descriptive, show rather than tell.  User Message: " + input);
            messageSplitter.ChunkRespond(ctx, response.Text());
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

    public class MessageSplitter
    {
        static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, (int)Math.Ceiling(str.Length / (double)chunkSize))
                .Select(i => str.Substring(i * chunkSize, Math.Min(chunkSize, str.Length - i * chunkSize)))
                .ToList();
        }

        // Generates response from prompt and prints it
        public async void ChunkGenerate(CommandContext ctx, string prompt)
        {
            var response = "";
            await foreach (var chunk in Commands.model.StreamContentAsync(prompt))
            {
                response += chunk.Text().TrimEnd('\r', '\n');
            }
            ChunkRespond(ctx, response);
        }

        // Prints a response by 2k-character chunks.
        public async void ChunkRespond(CommandContext ctx, string response)
        {
            var chunks = Split(response, 2000);
            foreach (var chunk in chunks)
            {
                await ctx.RespondAsync(chunk);
            }
            Commands.history.Add(new Content(response, Roles.Model));
            Commands.chatSession.History = Commands.history;
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
