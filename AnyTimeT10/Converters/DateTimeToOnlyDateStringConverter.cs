using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace AnyTimeT10.Converters
{
    class DateTimeToOnlyDateStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                DateTime dt = (DateTime)value;
                return dt.ToString("yyyy-MM-dd");
            }
            else
            {
                return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
