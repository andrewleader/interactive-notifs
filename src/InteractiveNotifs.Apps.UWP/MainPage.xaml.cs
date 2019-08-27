using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.PushNotifications;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace InteractiveNotifs.Apps.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            var dontWait = GetPushToken();
        }

        public async Task GetPushToken()
        {
            try
            {
                const string appServerKey = "BGg3UxXo3J_rH6VrJB2er_F8o7m2ZTGb2jiNm3tmlK4ORxsskX1HIVys5TA8lGkCYC-ur8GwrZMy-v0LZOwazvk";
                //IBuffer appServerKeyBuffer = urlB64ToUint8Array(appServerKey).AsBuffer();
                IBuffer appServerKeyBuffer = new byte[] { 4, 104, 55, 83, 21, 232, 220, 159, 235, 31, 165, 107, 36, 29, 158, 175, 241, 124, 163, 185, 182, 101, 49, 155, 218, 56, 141, 155, 123, 102, 148, 174, 14, 71, 27, 44, 145, 125, 71, 33, 92, 172, 229, 48, 60, 148, 105, 2, 96, 47, 174, 175, 193, 176, 173, 147, 50, 250, 253, 11, 100, 236, 26, 206, 249 }.AsBuffer();

                var channel = await PushNotificationChannelManager.GetDefault().CreateRawPushNotificationChannelWithAlternateKeyForApplicationAsync(appServerKeyBuffer, "mainChannel2");
                //var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                //channel.PushNotificationReceived += Channel_PushNotificationReceived; // Foreground event doesn't work
                TextBoxWebEndpointUrl.Text = channel.Uri;
            }
            catch (Exception ex)
            {
                await new MessageDialog(ex.ToString()).ShowAsync();
            }
        }

        private void Channel_PushNotificationReceived(PushNotificationChannel sender, PushNotificationReceivedEventArgs args)
        {
            var dontWait = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
            {
                var dontWait2 = new MessageDialog("Received push").ShowAsync();
            });
        }

        private static byte[] urlB64ToUint8Array(string base64String)
        {
            var paddingLength = (4 - base64String.Length % 4) % 4;
            var padding = string.Join("", new int[paddingLength].Select(i => "="));
            var base64 = (base64String + padding);

            base64 = Regex.Replace(base64, "\\-", "+");

            base64 = Regex.Replace(base64, "_", "/");

            var rawData = Convert.FromBase64String(base64);
            return rawData;
            //const outputArray = new Uint8Array(rawData.length);

            //for (let i = 0; i < rawData.length; ++i)
            //{
            //    outputArray[i] = rawData.charCodeAt(i);
            //}
            //return outputArray;
        }
    }
}
