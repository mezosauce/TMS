using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Time_Managmeent_System.Converters;

public class NullToDefaultImageConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var url = value as string;
        return string.IsNullOrEmpty(url)
            ? "{x:Static models:SymbolCode.User}" // Place a default image in your Resources/Images folder
            : url;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}