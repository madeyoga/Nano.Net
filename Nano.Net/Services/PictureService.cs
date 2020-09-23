using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace Nano.Net.Services
{
    public class PictureService
    {
        private readonly HttpClient client;

        public PictureService(HttpClient _client)
        {
            client = _client;
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
    }
}
