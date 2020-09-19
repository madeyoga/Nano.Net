using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Reddit;
using Reddit.Controllers;
using Reddit.Inputs.Search;
using Newtonsoft.Json;

namespace NewDiscordBot.Services
{
    public class PictureService
    {
        private readonly HttpClient client;
        private RedditClient redditClient;

        public PictureService(HttpClient _client)
        {
            client = _client;

            redditClient = new RedditClient(
                appId: Environment.GetEnvironmentVariable("REDDIT_CLIENT_ID"), 
                appSecret: Environment.GetEnvironmentVariable("Reddit_CLIENT_SECRET"),
                refreshToken: Environment.GetEnvironmentVariable("REDDIT_USER_AGENT")
                );
        }

        public async Task<Stream> GetCatPictureAsync()
        {
            var resp = await client.GetAsync("https://cataas.com/cat");
            return await resp.Content.ReadAsStreamAsync();
        }

        public async Task<Dictionary<String, String>> GetDogUrlAsync()
        {
            var resp = await client.GetAsync("https://dog.ceo/api/breeds/image/random");
            var jsonString = await resp.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<Dictionary<String, String>>(jsonString);
        }

        public List<Post> GetPicture(String query, String subreddit = "")
        {
            List<Post> posts = redditClient.Subreddit(subreddit).Search(new SearchGetSearchInput("Bernie Sanders"));
            if (posts.Count == 0)
            {
                posts = redditClient.Search(new SearchGetSearchInput(query));
            }
            return posts;
        }

    }
}
