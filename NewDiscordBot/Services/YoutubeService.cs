using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using YoutubeSearchAPI;

namespace NewDiscordBot.Services
{
    public class YoutubeService
    {
        private YoutubeSearchClient ytsClient;

        public YoutubeService()
        {
            ytsClient = new YoutubeSearchClient(Environment.GetEnvironmentVariable("DEVELOPER_KEY"));
        }

        public async Task <dynamic> SearchVideosByQuery(string query)
        {
            dynamic response = await ytsClient.Search(query);
            return response;
        }
    }
}
