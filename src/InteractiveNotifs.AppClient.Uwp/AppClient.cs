using InteractiveNotifs.Api;
using InteractiveNotifs.AppClient.Uwp.BackgroundTasks;
using InteractiveNotifs.HubClientSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Networking.PushNotifications;
using Windows.Storage;

namespace InteractiveNotifs.AppClient.Uwp
{
    public static class AppClient
    {
        public static IAsyncAction RegisterAsync()
        {
            return RegisterHelperAsync().AsAsyncAction();
        }

        private static async Task RegisterHelperAsync()
        {
            try
            {
                await BackgroundExecutionManager.RequestAccessAsync();

                RegisterBackgroundTask();

                var channel = await PushNotificationChannelManager.CreatePushNotificationChannelForApplicationAsync();
                await SendRegistrationToServerAsync(channel.Uri);
            }
            catch (Exception ex)
            {

            }
        }

        private static async Task SendRegistrationToServerAsync(string token)
        {
            try
            {
                HubClient c = new HubClient();
                await c.RegisterDeviceAsync(new Device()
                {
                    Type = DeviceType.Windows,
                    Identifier = token
                });
                ToastNotificationBackgroundTask.SendToast("Registered", "Device successfully registered!");
            }
            catch (Exception ex)
            {
                ToastNotificationBackgroundTask.SendToast("Failed to register", ex.ToString());
            }
        }

        private static void RegisterBackgroundTask()
        {
            const string taskName = "ToastBackgroundTask";

            // If background task is already registered, do nothing
            if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(taskName)))
                return;

            // Otherwise create the background task
            var builder = new BackgroundTaskBuilder()
            {
                Name = taskName,
                TaskEntryPoint = typeof(ToastNotificationBackgroundTask).FullName
            };

            // And set the toast action trigger
            builder.SetTrigger(new ToastNotificationActionTrigger());

            // And register the task
            builder.Register();
        }
    }
}
