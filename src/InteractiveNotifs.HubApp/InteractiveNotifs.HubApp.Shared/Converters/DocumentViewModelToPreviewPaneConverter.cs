using InteractiveNotifs.HubApp.Shared.ViewModels.Documents;
using InteractiveNotifs.HubApp.Shared.Views.PreviewPanes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace InteractiveNotifs.HubApp.Shared.Converters
{
    public class DocumentViewModelToPreviewPaneConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is BlocksDocumentViewModel)
            {
                return new BlocksPreviewPane()
                {
                    DataContext = value
                };
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
