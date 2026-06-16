using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tipografia3.Models;

namespace Tipografia3.Converters;

public class NumberCehToNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int numberCeh && parameter is IEnumerable<TipografCeh> cehs)
            return cehs.FirstOrDefault(c => c.NumberCeh == numberCeh)?.NameCeh ?? "Неизвестно";
        return "Неизвестно";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}