using InteractiveNotifs.Api;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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

        public Task<IEnumerable<Device>> GetDevicesAsync()
        {
            return GetAsync<IEnumerable<Device>>("devices/");
        }

        public async Task RegisterDeviceAsync(Device device)
        {
            await PostAsync("devices/", device);
        }

        public async Task<SendNotificationResult> SendNotificationAsync(Notification notification)
        {
            var resp = await PostAsync("notifications/", notification);
            string json = await resp.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<DevicesAndResults>(json);

            var answer = new SendNotificationResult();

            for (int i = 0; i < obj.Devices.Length && i < obj.Results.Length; i++)
            {
                answer.Results.Add(new KeyValuePair<Device, string>(obj.Devices[i], obj.Results[i]));
            }

            return answer;
        }

        private async Task<HttpResponseMessage> PostAsync(string path, object content)
        {
            string json = JsonConvert.SerializeObject(content);
            HttpContent httpContent = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
            var resp = await _client.PostAsync(path, httpContent);
            resp.EnsureSuccessStatusCode();
            return resp;
        }

        private async Task<T> GetAsync<T>(string path)
        {
            var json = await _client.GetStringAsync(path);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public class DevicesAndResults
        {
            public Device[] Devices { get; set; }
            public string[] Results { get; set; }
        }
    }

    public class SendNotificationResult
    {
        public List<KeyValuePair<Device, string>> Results { get; } = new List<KeyValuePair<Device, string>>();
    }
}
