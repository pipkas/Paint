using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using PaintApp.Src;

namespace PaintApp.UI;

public class EnumNullableConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null) return false;
        var tool = (Tool)value;
        return tool.ToolType.ToString() == parameter.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}