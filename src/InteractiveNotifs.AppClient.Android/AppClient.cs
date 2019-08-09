using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Firebase;
using Firebase.Iid;

namespace InteractiveNotifs.AppClientSdk.Android
{
    public static class AppClient
    {
        public static async Task RegisterAsync(Context context)
        {
            try
            {
                context = context.ApplicationContext;
                if (!IsPlayServicesAvailable(context))
                {
                    return;
                }

                CreateNotificationChannel(context);

                var token = FirebaseInstanceId.Instance.Token;
                await MyFirebaseIIDService.SendRegistrationToServerAsync(token, context);
            }
            catch (Exception ex)
            {
                Log.Debug("RegisterAppClient", ex.ToString());
            }
        }

        private static bool IsPlayServicesAvailable(Context context)
        {
            int resultCode = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(context);
            if (resultCode != ConnectionResult.Success)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private const string CHANNEL_ID = "DefaultChannel";
        private static void CreateNotificationChannel(Context context)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
            {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel(CHANNEL_ID,
                                                  "FCM Notifications",
                                                  NotificationImportance.Default)
            {

                Description = "Firebase Cloud Messages appear in this channel"
            };

            var notificationManager = (NotificationManager)context.GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}