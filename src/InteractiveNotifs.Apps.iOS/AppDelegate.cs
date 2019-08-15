using System;
using System.Collections.Generic;
using System.Linq;
using CoreFoundation;
using Foundation;
using InteractiveNotifs.Api;
using InteractiveNotifs.HubClientSdk;
using UIKit;
using UserNotifications;

namespace InteractiveNotifs.Apps.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            // Request notification permissions from the user
            UNUserNotificationCenter.Current.RequestAuthorization(UNAuthorizationOptions.Alert, (approved, err) => {
                // Handle approval

                if (approved)
                {
                    try
                    {
                        DispatchQueue.MainQueue.DispatchAsync(delegate
                        {
                            try
                            {
                                UIApplication.SharedApplication.RegisterForRemoteNotifications();
                            }
                            catch (Exception ex)
                            {
                                ShowDialog("Failed register for remote notifications", ex.ToString());
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        ShowDialog("Failed register remote notifications", ex.ToString());
                    }
                }
                else
                {
                    ShowDialog("Failed requesting notifications", err.Description);
                }
            });

            return base.FinishedLaunching(app, options);
        }

        public override async void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            try
            {
                var token = deviceToken.Description;
                if (token != null)
                {
                    token = token.Trim('<').Trim('>');
                }

                var client = new HubClient();
                await client.RegisterDeviceAsync(new Device()
                {
                    Type = DeviceType.iOS,
                    Identifier = token
                });

                ShowDialog("Successfully registered!", "All ready to receive notifications");
            }
            catch (Exception ex)
            {
                ShowDialog("Failed sending token", ex.ToString());
            }
        }

        private void ShowDialog(string title, string message)
        {
            new UIAlertView(title, message, null, "OK").Show();
        }

        public override void FailedToRegisterForRemoteNotifications(UIApplication application, NSError error)
        {
            ShowDialog("Failed register remote notifs", error.Description);
        }
    }
}
