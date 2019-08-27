using System;
using System.Linq;
using AdaptiveBlocks;
using Foundation;
using UIKit;
using UserNotifications;

namespace InteractiveNotifs.Apps.iOS.NotificationService
{
    // Note that you MUST DEPLOY IN RELEASE MODE to use extensions. Otherwise the app will fail deploying with a ditto error 1 code
    // Also note you have to CLEAN this project before deploying to make sure changes are applied
    [Register("NotificationService")]
    public class NotificationService : UNNotificationServiceExtension
    {
        Action<UNNotificationContent> ContentHandler { get; set; }
        UNMutableNotificationContent BestAttemptContent { get; set; }

        protected NotificationService(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

        public override void DidReceiveNotificationRequest(UNNotificationRequest request, Action<UNNotificationContent> contentHandler)
        {
            ContentHandler = contentHandler;
            BestAttemptContent = (UNMutableNotificationContent)request.Content.MutableCopy();

            try
            {
                NSObject blockObj = request.Content.UserInfo["block"];
                string blockJson = blockObj.Description;
                var block = AdaptiveBlock.Parse(blockJson).Block;

                var content = block?.View?.Content;
                if (content != null)
                {
                    var actions = content.GetSimplifiedActions().ToArray();
                    if (actions.Any())
                    {
                        // You seemingly can't create notification categories (or create new notifications) from this service

                        //string titles = string.Join(",", actions.Select(i => i.Title));


                        //UNNotificationAction[] unActions = actions.Take(1).Select(i => UNNotificationAction.FromIdentifier("myact", i.Title, UNNotificationActionOptions.None)).ToArray();


                        //string categoryId = "mycat";

                        //var category = UNNotificationCategory.FromIdentifier(
                        //    identifier: categoryId,
                        //    actions: unActions,
                        //    intentIdentifiers: new string[] { },
                        //    options: UNNotificationCategoryOptions.None);

                        //UNUserNotificationCenter.Current.SetNotificationCategories(new NSSet<UNNotificationCategory>(new UNNotificationCategory[] { category }));

                        //var newContent = new UNMutableNotificationContent();
                        //newContent.Title = "New notification";
                        ////newContent.CategoryIdentifier = categoryId;
                        //var trigger = UNTimeIntervalNotificationTrigger.CreateTrigger(5, false);

                        //var requestID = "sampleRequest";
                        //var newRequest = UNNotificationRequest.FromIdentifier(requestID, newContent, trigger);

                        //UNUserNotificationCenter.Current.AddNotificationRequest(newRequest, (err) => {
                        //    if (err != null)
                        //    {
                        //        // Do something with error...
                        //    }
                        //});

                        //BestAttemptContent.CategoryIdentifier = categoryId;
                        //BestAttemptContent.Title = "Set category id 3";
                    }
                }
            }
            catch (Exception ex) {
                BestAttemptContent.Subtitle = LimitLength(ex.ToString());
            }


            ContentHandler(BestAttemptContent);
        }

        public override void TimeWillExpire()
        {
            // Called just before the extension will be terminated by the system.
            // Use this as an opportunity to deliver your "best attempt" at modified content, otherwise the original push payload will be used.

            ContentHandler(BestAttemptContent);
        }

        private static string LimitLength(string str)
        {
            if (str.Length > 100)
            {
                return str.Substring(97) + "...";
            }
            return str;
        }
    }
}
