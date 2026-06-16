using Avalonia.Data.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tipografia3.Models;

namespace Tipografia3.Converters;

public class IdDogovorToClientConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int numberDogovor && parameter is IEnumerable<Dogovor> dogovors)
        {
            var dogovor = dogovors.FirstOrDefault(d => d.NumberDogovor == numberDogovor);
            if (dogovor != null && parameter is IEnumerable <Client> clients)
            {
                var client = clients.FirstOrDefault(c => c.IdClient == dogovor.IdClient);
                return client?.NameClient ?? "Неизвестно";
            }
        }
        return "Неизвестно";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}