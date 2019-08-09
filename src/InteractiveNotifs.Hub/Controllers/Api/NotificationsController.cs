using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveBlocks;
using AdaptiveBlocks.Transformers.ToastContentTransformer;
using InteractiveNotifs.Api;
using InteractiveNotifs.Hub.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace InteractiveNotifs.Hub.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {

        // POST: api/Notifications
        [HttpPost]
        public void Post([FromBody] Notification value)
        {
            SendNotification(value);
        }

        public static void SendNotification(Notification notification)
        {
            AdaptiveBlock block = AdaptiveBlock.Parse(notification.AdaptiveBlock).Block;
            var devices = DevicesController.GetDevices();

            foreach (var d in devices)
            {
                SendNotification(block, notification.AdaptiveBlock, d);
            }
        }

        private static void SendNotification(AdaptiveBlock block, string blockJson, Device device)
        {
            try
            {
                switch (device.Type)
                {
                    case DeviceType.Android:
                        SendAndroidNotification(block, blockJson, device);
                        break;

                    case DeviceType.Windows:
                        SendWindowsNotification(block, blockJson, device);
                        break;
                }
            }
            catch { }
        }

        private static void SendAndroidNotification(AdaptiveBlock block, string blockJson, Device device)
        {
            PushNotificationsFCM.SendFcmPushNotification(device.Identifier, new Dictionary<string, string>()
            {
                { "Notification", blockJson }
            });
        }

        private const string WindowsPackageSid = "ms-app://s-1-15-2-4163651854-1969534114-66483262-910187872-795330860-950916538-241190459";
        private const string WindowsSecret = "iiv4jPk60SZz8lYOprU9iD4fD2i3q3fs";
        private static async void SendWindowsNotification(AdaptiveBlock block, string blockJson, Device device)
        {
            var transformResult = await new AdaptiveBlockToToastContentTransformer().TransformAsync(block);
            if (transformResult.Result != null)
            {
                PushNotificationsWNS.Push(transformResult.Result.GetContent(), device.Identifier, WindowsSecret, WindowsPackageSid, PushNotificationsWNS.NotificationType.Toast);
            }
            else
            {
                string body = "<toast><visual><binding template=\"ToastGeneric\"><text>" + blockJson + "</text></binding></visual></toast>";
            }
        }
    }
}