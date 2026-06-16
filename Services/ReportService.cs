using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tipografia3;
using Tipografia3.Models;

namespace Tipografia3.Services;
public class ReportService
{
    // Отчёт 1: Сводка по цехам
    public async Task<List<CehReport>> GetCehReportAsync()
    {
        using var db = new TipografiaContext();
        var result = new List<CehReport>();

        var cehs = await db.TipografCehs.ToListAsync();
        foreach (var ceh in cehs)
        {
            var products = await db.Products.Where(p => p.NumberCeh == ceh.NumberCeh).ToListAsync();
            var productIds = products.Select(p => p.IdProduct).ToList();
            var orders = await db.Orders.Where(o => productIds.Contains(o.IdProduct)).ToListAsync();

            double totalSum = 0;
            foreach (var order in orders)
            {
                var product = products.FirstOrDefault(p => p.IdProduct == order.IdProduct);
                if (product != null)
                    totalSum += order.Quantity * product.Price1sh;
            }

            result.Add(new CehReport
            {
                NameCeh = ceh.NameCeh,
                BossCeh = ceh.BossCeh,
                PhoneCeh = ceh.PhoneCeh,
                ProductCount = products.Count,
                TotalSum = totalSum
            });
        }
        return result;
    }

    // Отчёт 2: Сводка по продукции
    public async Task<List<ProductReport>> GetProductReportAsync()
    {
        using var db = new TipografiaContext();
        var result = new List<ProductReport>();
        var products = await db.Products.ToListAsync();

        foreach (var product in products)
        {
            var orders = await db.Orders.Where(o => o.IdProduct == product.IdProduct).ToListAsync();
            int totalQuantity = orders.Sum(o => o.Quantity);
            double revenue = totalQuantity * product.Price1sh;
            var ceh = await db.TipografCehs.FirstOrDefaultAsync(c => c.NumberCeh == product.NumberCeh);

            result.Add(new ProductReport
            {
                NameProduct = product.NameProduct ?? "Неизвестно",
                CehName = ceh?.NameCeh ?? "Неизвестно",
                Price = product.Price1sh,
                OrderCount = orders.Count,
                TotalQuantity = totalQuantity,
                Revenue = revenue
            });
        }
        return result;
    }

    // Отчёт 3: Невостребованная продукция
    public async Task<List<Product>> GetUnsoldProductsAsync()
    {
        using var db = new TipografiaContext();
        var orderedProductIds = await db.Orders.Select(o => o.IdProduct).Distinct().ToListAsync();
        return await db.Products.Where(p => !orderedProductIds.Contains(p.IdProduct)).ToListAsync();
    }

    // Отчёт 4: Сводка по договорам
    public async Task<List<DogovorReport>> GetDogovorReportAsync()
    {
        using var db = new TipografiaContext();
        var result = new List<DogovorReport>();
        var dogovors = await db.Dogovors.ToListAsync();

        foreach (var dogovor in dogovors)
        {
            var client = await db.Clients.FirstOrDefaultAsync(c => c.IdClient == dogovor.IdClient);
            var orders = await db.Orders.Where(o => o.NumberDogovor == dogovor.NumberDogovor).ToListAsync();

            double totalSum = 0;
            foreach (var order in orders)
            {
                var product = await db.Products.FirstOrDefaultAsync(p => p.IdProduct == order.IdProduct);
                if (product != null)
                    totalSum += order.Quantity * product.Price1sh;
            }

            bool isCompleted = DateTime.TryParse(dogovor.DateDue, out var dueDate) && dueDate < DateTime.Now;

            result.Add(new DogovorReport
            {
                NumberDogovor = dogovor.NumberDogovor,
                ClientName = client?.NameClient ?? "Неизвестно",
                DateOforml = dogovor.DateOforml,
                DateDue = dogovor.DateDue,
                TotalSum = totalSum,
                Status = isCompleted ? "Выполнен" : "Не выполнен"
            });
        }
        return result;
    }

    // Отчёт 5: Топ-5 продукции по выручке
    public async Task<List<TopProductReport>> GetTopProductsAsync()
    {
        using var db = new TipografiaContext();
        var products = await db.Products.ToListAsync();
        var allOrders = await db.Orders.ToListAsync();

        double totalRevenue = 0;
        var productRevenues = new List<TopProductReport>();

        foreach (var product in products)
        {
            var orders = allOrders.Where(o => o.IdProduct == product.IdProduct).ToList();
            int quantity = orders.Sum(o => o.Quantity);
            double revenue = quantity * product.Price1sh;
            totalRevenue += revenue;

            productRevenues.Add(new TopProductReport
            {
                NameProduct = product.NameProduct ?? "Неизвестно",
                Revenue = revenue,
                Share = 0
            });
        }

        var top5 = productRevenues.OrderByDescending(p => p.Revenue).Take(5).ToList();
        foreach (var item in top5)
        {
            item.Share = totalRevenue > 0 ? (item.Revenue / totalRevenue) * 100 : 0;
        }

        return top5;
    }
}
public class CehReport
{
    public string NameCeh { get; set; } = null!;
    public string BossCeh { get; set; } = null!;
    public string PhoneCeh { get; set; } = null!;
    public int ProductCount { get; set; }
    public double TotalSum { get; set; }
}

public class ProductReport
{
    public string NameProduct { get; set; } = null!;
    public string CehName { get; set; } = null!;
    public double Price { get; set; }
    public int OrderCount { get; set; }
    public int TotalQuantity { get; set; }
    public double Revenue { get; set; }
}

public class DogovorReport
{
    public int NumberDogovor { get; set; }
    public string ClientName { get; set; } = null!;
    public string DateOforml { get; set; } = null!;
    public string DateDue { get; set; } = null!;
    public double TotalSum { get; set; }
    public string Status { get; set; } = null!;
}

public class TopProductReport
{
    public string NameProduct { get; set; } = null!;
    public double Revenue { get; set; }
    public double Share { get; set; }
}