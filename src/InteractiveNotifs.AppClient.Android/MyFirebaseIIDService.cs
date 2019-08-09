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

        public static async Task SendRegistrationToServerAsync(string token)
        {
            try
            {
                HubClient c = new HubClient();
                await c.RegisterDeviceAsync(new Device()
                {
                    Type = DeviceType.Android,
                    Identifier = token
                });
            }
            catch (Exception ex)
            {
                Log.Debug("SendRegistration", ex.ToString());
            }
        }
    }
}