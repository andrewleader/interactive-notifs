using AdaptiveBlocks.Commands;
using Microsoft.Toolkit.Uwp.Notifications;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace InteractiveNotifs.AppClient.Uwp.BackgroundTasks
{
    public sealed class ToastNotificationBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral since we're executing async code
            var deferral = taskInstance.GetDeferral();

            try
            {
                // If it's a toast notification action
                if (taskInstance.TriggerDetails is ToastNotificationActionTriggerDetail)
                {
                    // Get the toast activation details
                    var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;

                    // Deserialize the command
                    var cmd = JsonConvert.DeserializeObject<BaseAdaptiveCommand>(details.Argument);

                    if (cmd is AdaptiveHttpCommand httpCommand)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            var req = new HttpRequestMessage(new HttpMethod(httpCommand.Method ?? "GET"), httpCommand.Url);
                            var resp = await client.SendAsync(req);
                            resp.EnsureSuccessStatusCode();
                            var respStr = await resp.Content.ReadAsStringAsync();
                            SendToast("Activated", respStr);
                        }
                    }
                    else
                    {
                        SendToast("Activated", "Type: " + cmd.Type);
                    }
                }

                // Otherwise handle other background activations
                else
                    throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                SendToast("Failed", ex.ToString());
            }

            finally
            {
                // And finally release the deferral since we're done
                deferral.Complete();
            }
        }

        /// <summary>
        /// Simple method to show a basic toast with a message.
        /// </summary>
        /// <param name="message"></param>
        public static void SendToast(string title, string message)
        {
            ToastContent content = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = title
                            },

                            new AdaptiveText()
                            {
                                Text = message
                            }
                        }
                    }
                }
            };

            ToastNotificationManager.CreateToastNotifier().Show(new ToastNotification(content.GetXml()));
        }
    }
}
