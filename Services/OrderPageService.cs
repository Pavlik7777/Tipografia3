using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tipografia3.Models;

namespace Tipografia3.Services;
public class OrderPageService : IDBService<Order>
{
    public async Task<List<Order>> GetAsync()
    {
        using var db = new TipografiaContext();
        return await db.Orders.ToListAsync();
    }

    public async Task AddAsync(Order order)
    {
        using var db = new TipografiaContext();
        await db.Orders.AddAsync(order);
        await db.SaveChangesAsync();
    }

    public async Task EditAsync(Order order)
    {
        using var db = new TipografiaContext();
        var edit = db.Orders.First(p => p.IdOrder == order.IdOrder);
        edit.NumberDogovor = order.NumberDogovor;
        edit.IdProduct = order.IdProduct;
        edit.Quantity = order.Quantity;
        db.Orders.Update(edit);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var db = new TipografiaContext();
        var delete = db.Orders.First(p => p.IdOrder == id);
        db.Orders.Remove(delete);
        await db.SaveChangesAsync();
    }

    public async Task<Order?> GetByIdAsync(int id)
    {
        using var db = new TipografiaContext();
        return await db.Orders.FirstOrDefaultAsync(p => p.IdOrder == id);
    }
}