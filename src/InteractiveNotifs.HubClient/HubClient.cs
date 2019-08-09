using InteractiveNotifs.Api;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace InteractiveNotifs.HubClientSdk
{
    public class HubClient
    {
        //private const string DEFAULT_URL = "https://localhost:44358/";
        private const string DEFAULT_URL = "https://interactivenotifs.azurewebsites.net/";

        private HttpClient _client;

        public HubClient(string url = DEFAULT_URL)
        {
            _client = new HttpClient()
            {
                BaseAddress = new Uri(new Uri(url), "api/")
            };
        }

        public async Task RegisterDeviceAsync(Device device)
        {
            await PostAsync("devices/", device);
        }

        public async Task SendNotificationAsync(Notification notification)
        {
            await PostAsync("notifications/", notification);
        }

        private async Task PostAsync(string path, object content)
        {
            string json = JsonConvert.SerializeObject(content);
            HttpContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync(path, httpContent);
            resp.EnsureSuccessStatusCode();
        }
    }
}
