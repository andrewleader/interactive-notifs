﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AdaptiveBlocks;
using Android.App;
using Android.App.Job;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using InteractiveNotifs.AppClientSdk.Android.Receivers;
using Newtonsoft.Json;

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
            try
            {
                var block = AdaptiveBlock.Parse(notificationJson).Block;

                var content = block?.View?.Content;
                if (content != null)
                {
                    var builder = new NotificationCompat.Builder(this, AppClient.CHANNEL_ID)
                        .SetSmallIcon(Android.Resource.Drawable.abc_btn_switch_to_on_mtrl_00001)
                        .SetAutoCancel(true)
                        .SetPriority(1);

                    if (content.Title != null)
                    {
                        builder.SetContentTitle(content.Title);
                    }

                    if (content.Subtitle != null)
                    {
                        builder.SetContentText(content.Subtitle);
                    }

                    foreach (var action in content.GetSimplifiedActions())
                    {
                        if (action.Inputs.Count == 0 && action.Command != null)
                        {
                            Intent actionIntent = new Intent(this, typeof(NotificationActionReceiver));
                            actionIntent.SetAction("com.microsoft.InteractiveNotifs.ApiClient.Android.InvokeAction");
                            actionIntent.PutExtra("cmd", JsonConvert.SerializeObject(action.Command));
                            PendingIntent pendingActionIntent = PendingIntent.GetBroadcast(this, 0, actionIntent, PendingIntentFlags.UpdateCurrent);

                            builder.AddAction(Android.Resource.Drawable.abc_btn_check_material, action.Title, pendingActionIntent);
                        }
                    }

                    var manager = NotificationManagerCompat.From(this);
                    manager.Notify(1, builder.Build());
                }
            }
            catch { }
        }
    }
}