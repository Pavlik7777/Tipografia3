using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tipografia3.Models;

namespace Tipografia3.Converters;

public class IdProductToNameConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int id && parameter is IEnumerable<Product> products)
            return products.FirstOrDefault(p => p.IdProduct == id)?.NameProduct ?? "Неизвестно";
        return "Неизвестно";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}