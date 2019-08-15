using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InteractiveNotifs.Hub.Helpers
{
    public static class PushNotificationsAPN
    {
        public static Task SendAsync(string identifier, JObject payload)
        {
            // https://github.com/Redth/PushSharp
            var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox,
                "ApnsCert.p12", "SimplePassword");

            var broker = new ApnsServiceBroker(config);

            var taskCompletionSource = new TaskCompletionSource<bool>();

            // Wire up events
            broker.OnNotificationFailed += (notification, aggregateEx) => {

                aggregateEx.Handle(ex => {

                    // See what kind of exception it was to further diagnose
                    if (ex is ApnsNotificationException notificationException)
                    {

                        // Deal with the failed notification
                        var apnsNotification = notificationException.Notification;
                        var statusCode = notificationException.ErrorStatusCode;

                        Console.WriteLine($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}");
                        taskCompletionSource.SetException(new Exception($"Apple Notification Failed: ID={apnsNotification.Identifier}, Code={statusCode}"));

                    }
                    else
                    {
                        // Inner exception might hold more useful information like an ApnsConnectionException			
                        Console.WriteLine($"Apple Notification Failed for some unknown reason : {ex.InnerException}");
                        taskCompletionSource.SetException(new Exception($"Apple Notification Failed for some unknown reason : {ex.InnerException}"));
                    }

                    // Mark it as handled
                    return true;
                });
            };

            broker.OnNotificationSucceeded += (notification) => {
                Console.WriteLine("Apple Notification Sent!");
                taskCompletionSource.SetResult(true);
            };

            // Start the broker
            broker.Start();

            // Queue a notif to send
            broker.QueueNotification(new ApnsNotification()
            {
                DeviceToken = identifier.Replace(" ", ""),
                Payload = payload
            });

            broker.Stop();

            return taskCompletionSource.Task;
        }
    }
}
