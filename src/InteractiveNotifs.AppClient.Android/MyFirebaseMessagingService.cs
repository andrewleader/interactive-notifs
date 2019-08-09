using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;

namespace InteractiveNotifs.AppClientSdk.Android
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class MyFirebaseMessagingService : FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            try
            {
                if (message.Data.TryGetValue("Notification", out string notification))
                {
                    SendNotification(notification);
                }
            }
            catch (Exception ex)
            {

            }
        }

        public override void OnDeletedMessages()
        {
            // TODO: Sync all accounts since messages were dropped/deleted
        }

        private void SendNotification(string notificationJson)
        {
            var builder = new NotificationCompat.Builder(this, AppClient.CHANNEL_ID)
                .SetSmallIcon(Android.Resource.Drawable.abc_ab_share_pack_mtrl_alpha)
                .SetContentTitle("FCM notification")
                .SetContentText(notificationJson)
                .SetAutoCancel(true);

            var manager = NotificationManagerCompat.From(this);
            manager.Notify(1, builder.Build());
        }
    }
}