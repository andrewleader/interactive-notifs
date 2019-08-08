using InteractiveNotifs.Api;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace InteractiveNotifs.HubClient
{
    public class HubClient
    {
        private HttpClient _client;

        public HubClient(string url)
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(new Uri(url), "api")
            };
        }

        public async Task RegisterDeviceAsync(Device device)
        {
            await PostAsync("devices", device);
        }

        public async Task SendNotificationAsync(Notification notification)
        {
            await PostAsync("notifications", notification);
        }

        private async Task PostAsync(string path, object content)
        {
            HttpContent httpContent = new StringContent(JsonConvert.SerializeObject(content));
            var resp = await _client.PostAsync(path, httpContent);
            resp.EnsureSuccessStatusCode();
        }
    }
}
