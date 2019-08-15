using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveBlocks;
using AdaptiveBlocks.Transformers.ToastContentTransformer;
using AdaptiveBlocks.Transformers.WebNotification;
using InteractiveNotifs.Api;
using InteractiveNotifs.Hub.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InteractiveNotifs.Hub.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {

        // POST: api/Notifications
        [HttpPost]
        public async Task<DevicesAndResults> Post([FromBody] Notification value)
        {
            return await SendNotification(value);
        }

        public static async Task<DevicesAndResults> SendNotification(Notification notification)
        {
            AdaptiveBlock block = AdaptiveBlock.Parse(notification.AdaptiveBlock).Block;
            var devices = DevicesController.GetDevices();
            Dictionary<Device, Task> results = new Dictionary<Device, Task>();
            var tasks = new List<Task>();

            foreach (var d in devices)
            {
                try
                {
                    tasks.Add(SendNotification(block, notification.AdaptiveBlock, d));
                }
                catch (Exception ex)
                {
                    tasks.Add(Task.FromException(ex));
                }
            }

            try
            {
                await Task.WhenAll(tasks);
            }
            catch { }

            var answer = new DevicesAndResults()
            {
                Devices = devices,
                Results = tasks.Select(i => i.IsCompletedSuccessfully ? "Sent" : i.Exception.ToString())
            };

            return answer;
        }

        public class DevicesAndResults
        {
            public IEnumerable<Device> Devices { get; set; }
            public IEnumerable<string> Results { get; set; }
        }

        private static async Task SendNotification(AdaptiveBlock block, string blockJson, Device device)
        {
            switch (device.Type)
            {
                case DeviceType.Android:
                    await SendAndroidNotification(block, blockJson, device);
                    break;

                case DeviceType.Windows:
                    await SendWindowsNotification(block, blockJson, device);
                    break;

                case DeviceType.Web:
                    await SendWebNotificationAsync(block, blockJson, device);
                    break;

                case DeviceType.iOS:
                    await SendiOSNotificationAsync(block, blockJson, device);
                    break;
            }
        }

        private static async Task SendiOSNotificationAsync(AdaptiveBlock block, string blockJson, Device device)
        {
            var content = block?.View?.Content;
            if (content?.Title == null)
            {
                return;
            }

            dynamic payloadObj = new ExpandoObject();
            payloadObj.aps = new ExpandoObject();
            (payloadObj.aps as ExpandoObject).TryAdd("mutable-content", 1);
            payloadObj.block = blockJson;

            payloadObj.aps.alert = new ExpandoObject();
            payloadObj.aps.alert.title = content.Title;
            if (content.Subtitle != null)
            {
                payloadObj.aps.alert.body = content.Subtitle;
            }

            await PushNotificationsAPN.SendAsync(device.Identifier, JObject.FromObject(payloadObj));
        }

        private static async Task SendWebNotificationAsync(AdaptiveBlock block, string blockJson, Device device)
        {
            var transformResult = (await new AdaptiveBlockToWebNotificationTransformer().TransformAsync(block));
            if (transformResult.Result == null)
            {
                throw new Exception(string.Join("\n", transformResult.Errors));
            }
            else
            {
                await PushNotificationsWeb.SendAsync(device.Identifier, JsonConvert.SerializeObject(transformResult.Result));
            }
        }

        private static async Task SendAndroidNotification(AdaptiveBlock block, string blockJson, Device device)
        {
            await PushNotificationsFCM.SendFcmPushNotification(device.Identifier, new Dictionary<string, string>()
            {
                { "Notification", blockJson }
            });
        }

        private const string WindowsPackageSid = "ms-app://s-1-15-2-4163651854-1969534114-66483262-910187872-795330860-950916538-241190459";
        private const string WindowsSecret = "iiv4jPk60SZz8lYOprU9iD4fD2i3q3fs";
        private static async Task SendWindowsNotification(AdaptiveBlock block, string blockJson, Device device)
        {
            string bodyXml;
            var transformResult = await new AdaptiveBlockToToastContentTransformer().TransformAsync(block);
            if (transformResult.Result != null)
            {
                bodyXml = transformResult.Result.GetContent();
            }
            else
            {
                bodyXml = "<toast><visual><binding template=\"ToastGeneric\"><text>" + blockJson + "</text></binding></visual></toast>";
            }

            var resp = PushNotificationsWNS.Push(bodyXml, device.Identifier, WindowsSecret, WindowsPackageSid, PushNotificationsWNS.NotificationType.Toast);
            if (resp != System.Net.HttpStatusCode.OK)
            {
                throw new Exception(resp.ToString());
            }
        }
    }
}