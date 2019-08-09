using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using AdaptiveBlocks.Commands;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace InteractiveNotifs.AppClientSdk.Android.Receivers
{
    [BroadcastReceiver]
    public class NotificationActionReceiver : BroadcastReceiver
    {
        public override async void OnReceive(Context context, Intent intent)
        {
            var pendingResult = base.GoAsync();

            try
            {
                string cmdJson = intent.Extras.GetString("cmd");
                BaseAdaptiveCommand cmd = JsonConvert.DeserializeObject<BaseAdaptiveCommand>(cmdJson);
                if (cmd is AdaptiveHttpCommand httpCommand)
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var req = new HttpRequestMessage(new HttpMethod(httpCommand.Method ?? "GET"), httpCommand.Url);
                        var resp = await client.SendAsync(req);
                        resp.EnsureSuccessStatusCode();
                        var respStr = await resp.Content.ReadAsStringAsync();
                        SendNotif(context, "Activated", respStr);
                    }
                }
                else
                {
                    SendNotif(context, "Activated", "Type: " + cmd.Type);
                }
            }
            catch (Exception ex)
            {
                SendNotif(context, "Activation failed", ex.ToString());
            }

            pendingResult.Finish();
        }

        private void SendNotif(Context context, string title, string content)
        {
            var builder = new NotificationCompat.Builder(context, AppClient.CHANNEL_ID)
                        .SetSmallIcon(Android.Resource.Drawable.abc_btn_switch_to_on_mtrl_00001)
                        .SetAutoCancel(true)
                        .SetContentTitle(title)
                        .SetContentText(content)
                        .SetPriority(1);

            var manager = NotificationManagerCompat.From(context);
            manager.Notify(1, builder.Build());
        }
    }
}