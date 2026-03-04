using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace PaintApp.UI;

public class AvaloniaColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not byte[] colorArray || colorArray.Length < 3)
        {
            return Brushes.Transparent;
        }
        return new Color(colorArray[3], colorArray[0], colorArray[1], colorArray[2]);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Color color)
        {
            return new byte[] { color.R, color.G, color.B, color.A };
        }
        return new byte[] { 0, 0, 0, 255 };
    }
}