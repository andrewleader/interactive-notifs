using AdaptiveBlocks;
using AdaptiveBlocks.Inputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InteractiveNotifs.HubBlazorApp.ViewModels
{
    public class AndroidNotificationViewModel
    {
        public string SmallIcon { get; set; }
        public string ContentTitle { get; set; }
        public string ContentText { get; set; }
        public string LargeIcon { get; set; }

        public AndroidNotificationStyle Style { get; set; }

        public List<AndroidNotificationAction> Actions { get; } = new List<AndroidNotificationAction>();

        public static AndroidNotificationViewModel Create(AdaptiveBlock block)
        {
            var content = block?.View?.Content;
            if (content != null)
            {
                var builder = new AndroidNotificationViewModel()
                {
                    ContentTitle = content.Title,
                    ContentText = content.Subtitle
                };

                var profileImg = content.GetProfileImage();
                var heroImg = content.GetHeroImage();

                if (heroImg != null)
                {
                    builder.LargeIcon = heroImg.Url;
                    builder.Style = new AndroidNotificationBigPictureStyle()
                    {
                        BigPicture = heroImg.Url,
                        BigLargeIcon = profileImg?.Url
                    };
                }
                else
                {
                    if (profileImg != null)
                    {
                        builder.LargeIcon = profileImg.Url;
                    }

                    // Expandable
                    builder.Style = new AndroidNotificationBigTextStyle()
                    {
                        BigText = content.Subtitle
                    };
                }

                foreach (var action in content.GetSimplifiedActions())
                {
                    if ((action.Inputs.Count == 0 || action.Inputs.Count == 1 && action.Inputs[0] is AdaptiveTextInputBlock) && action.Command != null)
                    {
                        builder.Actions.Add(new AndroidNotificationAction()
                        {
                            Title = action.Title
                        });
                    }
                }

                return builder;
            }

            return null;
        }
    }

    public abstract class AndroidNotificationStyle
    {
    }

    public class AndroidNotificationBigTextStyle : AndroidNotificationStyle
    {
        public string BigText { get; set; }
    }

    public class AndroidNotificationBigPictureStyle : AndroidNotificationStyle
    {
        public string BigPicture { get; set; }
        public string BigLargeIcon { get; set; }
    }

    public class AndroidNotificationAction
    {
        public string Icon { get; set; }
        public string Title { get; set; }
    }
}
