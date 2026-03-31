using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CosUI.Themes;

namespace CosUI.Converters;

public class ThemeAccentConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is ITheme theme) return new SolidColorBrush(theme.PrimaryBlue);
        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
