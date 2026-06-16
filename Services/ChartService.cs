using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Tipografia3.Services;
public class ChartService
{
    // График 1: Объём заказов по цехам (столбчатая)
    public async Task<Dictionary<string, double>> GetOrdersByCehAsync()
    {
        using var db = new TipografiaContext();
        var result = new Dictionary<string, double>();
        var cehs = await db.TipografCehs.ToListAsync();

        foreach (var ceh in cehs)
        {
            var products = await db.Products.Where(p => p.NumberCeh == ceh.NumberCeh).ToListAsync();
            var productIds = products.Select(p => p.IdProduct).ToList();
            var orders = await db.Orders.Where(o => productIds.Contains(o.IdProduct)).ToListAsync();

            double total = orders.Sum(o => o.Quantity * (products.FirstOrDefault(p => p.IdProduct == o.IdProduct)?.Price1sh ?? 0));
            result[ceh.NameCeh] = total;
        }
        return result;
    }

    // График 2: Доля заказов по видам продукции (круговая)
    public async Task<Dictionary<string, double>> GetOrdersByProductAsync()
    {
        using var db = new TipografiaContext();
        var result = new Dictionary<string, double>();
        var products = await db.Products.ToListAsync();

        foreach (var product in products)
        {
            var orders = await db.Orders.Where(o => o.IdProduct == product.IdProduct).ToListAsync();
            double total = orders.Sum(o => o.Quantity * product.Price1sh);
            result[product.NameProduct ?? "Неизвестно"] = total;
        }
        return result;
    }

    // График 3: Динамика заказов по месяцам (линейный)
    public async Task<Dictionary<string, double>> GetOrdersByMonthAsync()
    {
        using var db = new TipografiaContext();
        var result = new SortedDictionary<string, double>();
        var dogovors = await db.Dogovors.ToListAsync();

        foreach (var dogovor in dogovors)
        {
            if (DateTime.TryParse(dogovor.DateOforml, out var date))
            {
                string monthKey = date.ToString("yyyy-MM");
                var orders = await db.Orders.Where(o => o.NumberDogovor == dogovor.NumberDogovor).ToListAsync();
                double sum = 0;
                foreach (var order in orders)
                {
                    var product = await db.Products.FirstOrDefaultAsync(p => p.IdProduct == order.IdProduct);
                    if (product != null)
                        sum += order.Quantity * product.Price1sh;
                }

                if (result.ContainsKey(monthKey))
                    result[monthKey] += sum;
                else
                    result[monthKey] = sum;
            }
        }
        return new Dictionary<string, double>(result);
    }

    // График 4: Распределение стоимости единицы продукции (гистограмма)
    public async Task<List<double>> GetPriceDistributionAsync()
    {
        using var db = new TipografiaContext();
        return await db.Products.Select(p => p.Price1sh).ToListAsync();
    }
}