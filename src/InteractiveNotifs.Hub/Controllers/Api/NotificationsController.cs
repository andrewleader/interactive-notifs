using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            var devices = DevicesController.GetDevices();

            foreach (var d in devices)
            {
                SendNotification(notification, d);
            }
        }

        private static void SendNotification(Notification notification, Device device)
        {
            try
            {
                switch (device.Type)
                {
                    case DeviceType.Android:
                        SendAndroidNotification(notification, device);
                        break;

                    case DeviceType.Windows:
                        SendWindowsNotification(notification, device);
                        break;
                }
            }
            catch { }
        }

        private static void SendAndroidNotification(Notification notification, Device device)
        {
            PushNotificationsFCM.SendFcmPushNotification(device.Identifier, new Dictionary<string, string>()
            {
                { "Notification", notification.AdaptiveBlock }
            });
        }

        private const string WindowsPackageSid = "ms-app://s-1-15-2-4163651854-1969534114-66483262-910187872-795330860-950916538-241190459";
        private const string WindowsSecret = "iiv4jPk60SZz8lYOprU9iD4fD2i3q3fs";
        private static void SendWindowsNotification(Notification notification, Device device)
        {
            string body = "<toast><visual><binding template=\"ToastGeneric\"><text>" + notification.AdaptiveBlock + "</text></binding></visual></toast>";

            PushNotificationsWNS.Push(body, device.Identifier, WindowsSecret, WindowsPackageSid, PushNotificationsWNS.NotificationType.Toast);
        }
    }
}