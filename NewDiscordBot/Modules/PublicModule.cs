using Discord;
using Discord.Commands;
using NewDiscordBot.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NewDiscordBot.Modules
{
    // Modules must be public and inherit from an IModuleBase
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        public PictureService PictureService { get; set; }

        [Command("ping")]
        [Alias("pong", "hello")]
        public Task PingAsync()
            => ReplyAsync("pong!");

        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user = user ?? Context.User;

            await ReplyAsync(user.ToString());
        }

        [Command("cat")]
        public async Task GetCatAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await PictureService.GetCatPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }

        [Command("dog")]
        public async Task GetDogAsync()
        {
            Dictionary<String, String> responseDictionary = await PictureService.GetDogUrlAsync();
            await ReplyAsync(responseDictionary["message"]);
        }

        [Command("r")]
        public async Task SearchAsync(String query)
        {
            // VERYCOOL, DOESN'T WORK.
            Console.WriteLine(query);
            await Context.Channel.SendMessageAsync(query);

            var posts = await Task.Run(() => PictureService.GetPicture(query: query));

            var random = new Random();
            int randomIndex = random.Next(posts.Count);

            var selectedPost = posts[randomIndex];
            Console.WriteLine(selectedPost);
            Console.WriteLine(selectedPost.GetType());
        }
    }
}
