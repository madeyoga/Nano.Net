using Discord.Commands;
using NewDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewDiscordBot.Modules
{
    public class MusicModule : ModuleBase<SocketCommandContext>
    {
        public YoutubeService YoutubeService { get; set; }
        
        public MusicModule()
        {

        }

        [Command("search")]
        public async Task SearchYoutubeVideo([Remainder] string query)
        {
            dynamic data = await YoutubeService.SearchVideosByQuery(query);

            List<string> videoTitles = new List<string>();

            int counter = 1;
            foreach (var video in data["items"])
            {
                string videoTitle = $"[**{counter}**]. **{video["snippet"]["title"]}**";
                videoTitles.Add(videoTitle);
                counter += 1;
            }

            string reply = string.Join("\n", videoTitles);

            await Context.Channel.SendMessageAsync(reply);
        }
    }
}
