using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using CosUI.Themes;

namespace CosUI.Converters;

public class StatusToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int status) return DependencyProperty.UnsetValue;
        var res = Application.Current.Resources;
        var key = status switch
        {
            0 => ThemeKeys.Brush(ThemeKeys.TextPrint),
            1 => ThemeKeys.Brush(ThemeKeys.TextWarn),
            2 => ThemeKeys.Brush(ThemeKeys.TextError),
            3 => ThemeKeys.Brush(ThemeKeys.TextInfo),
            6 => ThemeKeys.Brush(ThemeKeys.TextPrimary),
            _ => ThemeKeys.Brush(ThemeKeys.TextSecondary)
        };
        return res[key] ?? DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
