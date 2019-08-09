using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AdaptiveBlocks;
using AdaptiveBlocks.Commands;
using AdaptiveBlocks.Inputs;
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
                        if ((action.Inputs.Count == 0 || action.Inputs.Count == 1 && action.Inputs[0] is AdaptiveTextInputBlock) && action.Command != null)
                        {
                            PendingIntent pendingIntent;
                            if (action.Command is AdaptiveOpenUrlCommand openUrlCommand)
                            {
                                Intent openUrlIntent = new Intent(Intent.ActionView, global::Android.Net.Uri.Parse(openUrlCommand.Url));
                                pendingIntent = PendingIntent.GetActivity(this, 0, openUrlIntent, 0);
                            }
                            else
                            {
                                Intent actionIntent = new Intent(this, typeof(NotificationActionReceiver));
                                actionIntent.SetAction("com.microsoft.InteractiveNotifs.ApiClient.Android.InvokeAction");
                                actionIntent.PutExtra("cmd", JsonConvert.SerializeObject(action.Command));
                                if (action.Inputs.FirstOrDefault() is AdaptiveTextInputBlock tbInputBlock)
                                {
                                    actionIntent.PutExtra("textId", tbInputBlock.Id);
                                }
                                pendingIntent = PendingIntent.GetBroadcast(this, new Random().Next(int.MaxValue), actionIntent, PendingIntentFlags.OneShot);
                            }

                            if (action.Inputs.FirstOrDefault() is AdaptiveTextInputBlock tbInput)
                            {
                                var remoteInput = new global::Android.Support.V4.App.RemoteInput.Builder("text")
                                    .SetLabel(tbInput.Placeholder ?? action.Title)
                                    .Build();

                                builder.AddAction(new NotificationCompat.Action.Builder(
                                    Resource.Drawable.abc_ic_go_search_api_material,
                                    action.Title,
                                    pendingIntent)
                                    .AddRemoteInput(remoteInput)
                                    .Build());
                            }
                            else
                            {
                                builder.AddAction(Android.Resource.Drawable.abc_btn_check_material, action.Title, pendingIntent);
                            }
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