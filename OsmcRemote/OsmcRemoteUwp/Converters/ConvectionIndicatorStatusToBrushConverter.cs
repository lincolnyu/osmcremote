using OsmcRemoteUwp.Data;
using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace OsmcRemoteUwp.Converters
{
    public class ConvectionIndicatorStatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var status = (ConnectionIndicatorStatuses)value;
            switch (status)
            {
                case ConnectionIndicatorStatuses.Initial:
                case ConnectionIndicatorStatuses.Disconnected:
                    return new SolidColorBrush(Colors.Orange);
                case ConnectionIndicatorStatuses.Checking:
                    return new SolidColorBrush(Colors.Yellow);
                case ConnectionIndicatorStatuses.Connected:
                    return new SolidColorBrush(Colors.Green);
            }
            throw new NotSupportedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }
}
