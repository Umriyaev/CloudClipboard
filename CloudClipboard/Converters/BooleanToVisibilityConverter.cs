using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace CloudClipboard.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(targetType ==typeof(Visibility))
            {
                var visible = (bool)value;
                return visible ? Visibility.Visible : Visibility.Collapsed;
            }
            throw new InvalidOperationException("Can only convert to Visibility");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
