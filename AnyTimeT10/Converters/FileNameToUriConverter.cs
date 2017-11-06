using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Data;

namespace AnyTimeT10.Converters
{
    class FileNameToUriConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            try
            {
                String AvatarFilename = ApplicationData.Current.LocalFolder.Path + "/Avatars/" + value.ToString();
                return AvatarFilename;
            }
            catch (Exception ex)
            {
                return "ms-appx:///Assets/Avatars/EmptyAvatar.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            try
            {
                return value.ToString().Split('/').Last();
            }
            catch (Exception ex)
            {
                return "ms-appx:///Assets/Avatars/EmptyAvatar.png";
            }
        }
    }
}
