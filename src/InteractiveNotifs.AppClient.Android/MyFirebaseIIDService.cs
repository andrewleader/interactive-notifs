using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Firebase.Iid;
using InteractiveNotifs.Api;
using InteractiveNotifs.HubClientSdk;

namespace InteractiveNotifs.AppClientSdk.Android
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class MyFirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        public override async void OnTokenRefresh()
        {
            try
            {
                await SendRegistrationToServerAsync();
            }
            catch { }
        }

        public static async Task SendRegistrationToServerAsync()
        {
            try
            {
                var refreshedToken = FirebaseInstanceId.Instance.Token;
                Log.Debug(TAG, "Refreshed token: " + refreshedToken);
                HubClient c = new HubClient();
                await c.RegisterDeviceAsync(new Device()
                {
                    Type = DeviceType.Android,
                    Identifier = refreshedToken
                });
            }
            catch (Exception ex)
            {
                Log.Debug("SendRegistration", ex.ToString());
            }
        }

        public static async Task SendRegistrationToServerAsync(string token, Context context)
        {
            try
            {
                HubClient c = new HubClient();
                await c.RegisterDeviceAsync(new Device()
                {
                    Type = DeviceType.Android,
                    Identifier = token
                });
                Toast.MakeText(context, "Successfully registered device", ToastLength.Short).Show();
                var builder = new NotificationCompat.Builder(context, AppClient.CHANNEL_ID)
                    .SetSmallIcon(Android.Resource.Drawable.abc_ab_share_pack_mtrl_alpha)
                    .SetContentTitle("Successfully registered!")
                    .SetContentText("Device has been registered")
                    .SetAutoCancel(true);

                var manager = NotificationManagerCompat.From(context);
                manager.Notify(1, builder.Build());
            }
            catch (Exception ex)
            {
                Log.Debug("SendRegistration", ex.ToString());
                Toast.MakeText(context, ex.ToString(), ToastLength.Long).Show();
            }
        }
    }
}