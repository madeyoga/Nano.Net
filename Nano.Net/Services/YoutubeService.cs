using System;
using System.Threading.Tasks;
using YoutubeSearchAPI;

namespace Nano.Net.Services
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
