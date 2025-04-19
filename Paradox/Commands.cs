using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Paradox.Commands
{
    public class Commands : BaseCommandModule
    {
        [Command("Hug")]
        public async Task Hug(CommandContext ctx)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync("https://api.waifu.pics/sfw/hug");
            var responseString = await response.Content.ReadAsStringAsync();
            var ImageUrl = JObject.Parse(responseString)["url"].ToString();
            var embed = new DiscordEmbedBuilder()
            {
                Title = "*Hugs You",
                ImageUrl = ImageUrl,
                Color = new DiscordColor(0x00ff00)
            };

            await ctx.RespondAsync(embed);
        }
    }

    public class JsonParser
    {
        static string json;
        public static string GoogleAPI;
        public static string DiscordToken;

        public static void JsonParseChecker()
        {
            //checks if path exists
            if (File.Exists("config.json"))
            {
                json = File.ReadAllText("config.json");
                GoogleAPI = JObject.Parse(json)["GoogleAPI"].ToString();
                DiscordToken = JObject.Parse(json)["DiscordToken"].ToString();

            }

            else
            {
                ConfigCreator creator = new ConfigCreator();
                Console.WriteLine("Please Enter Discord Token: ");
                creator.DiscordToken = (Console.ReadLine());
                Console.WriteLine("Please Enter GoogleAPI: ");
                creator.GoogleAPI = (Console.ReadLine());
                var ConfigCreatorSerialized = JsonConvert.SerializeObject(creator);
                File.WriteAllText("config.json", ConfigCreatorSerialized);
                JsonParseChecker();
            }
        }
    }

    public class ConfigCreator
    {
        public string DiscordToken;
        public string GoogleAPI;
        public string UserID;
        public string Response;
    }
}
