using AdaptiveBlocks;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace InteractiveNotifs.HubApp.Shared.Previews
{
    public interface IPreviewBlockHost
    {
        void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args);
    }

    public class PreviewBlockHostViewModel
    {
        public string AppName { get; set; } = "App name";

        public Uri AppLogo { get; set; } = new Uri("ms-appx:///Assets/Square44x44Logo.png");

        public AdaptiveBlockContent BlockContent { get; set; }

        public void QuickAssignAppLogo(string appFolder)
        {
            AppLogo = new Uri($"ms-appx:///Assets/Apps/{appFolder}/Square44x44Logo.png");
        }

        public void AssignApp(string appName, string appFolder)
        {
            AppName = appName;
            QuickAssignAppLogo(appFolder);
        }

        public ImageSource GetAppLogoImageSource()
        {
            return new BitmapImage(AppLogo);
        }
    }
}
