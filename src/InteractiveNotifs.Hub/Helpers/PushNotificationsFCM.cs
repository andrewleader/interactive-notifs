using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InteractiveNotifs.Hub.Helpers
{
    public static class PushNotificationsFCM
    {
        private const string FCM_SERVER_KEY = "AAAAafFfhDc:APA91bEZH3xTaV-Z1YbjDQNr4mwUbY4ypFEH4Ay396pRbcLZ7ByaJh0fC4QdoGChEojDnRpU89gtoUXsNIxuXbEYBepVaM77TJJHCHB8-o1OQVacu--upy1SU3F7Q6cwbhgBglN1OqtA";
        private const string FCM_SENDER_ID = "455021134903";
        public static async void SendFcmPushNotification(string token, Dictionary<string, string> data)
        {
            try
            {
                // Note that this third party FCM NuGet library uses the legacy HTTP protocol, but it's much simpler so that's fine with me
                using (var sender = new FCM.Net.Sender(FCM_SERVER_KEY))
                {
                    var message = new FCM.Net.Message()
                    {
                        To = token,
                        Data = data
                    };

                    await sender.SendAsync(message);
                }
            }
            catch { }
        }
    }
}
