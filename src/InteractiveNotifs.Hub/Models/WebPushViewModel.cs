using InteractiveNotifs.Hub.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace InteractiveNotifs.Hub.Models
{
    public class WebPushViewModel
    {
        public string PublicServerKey { get; set; } = PushNotificationsWeb.PublicKey;

        public string PrivateServerKey { get; set; } = PushNotificationsWeb.PrivateKey;

        public string SubscriptionJson { get; set; }

        public string Message { get; set; }
    }
}
