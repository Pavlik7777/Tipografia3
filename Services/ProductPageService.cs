using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tipografia3.Models;

namespace Tipografia3.Services;
public class ProductPageService : IDBService<Product>
{
    public async Task<List<Product>> GetAsync()
    {
        using var db = new TipografiaContext();
        return await db.Products.ToListAsync();
    }

    public async Task AddAsync(Product product)
    {
        using var db = new TipografiaContext();
        await db.Products.AddAsync(product);
        await db.SaveChangesAsync();
    }

    public async Task EditAsync(Product product)
    {
        using var db = new TipografiaContext();
        var edit = db.Products.First(p => p.IdProduct == product.IdProduct);
        edit.NameProduct = product.NameProduct;
        edit.NumberCeh = product.NumberCeh;
        edit.Price1sh = product.Price1sh;
        db.Products.Update(edit);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var db = new TipografiaContext();
        var delete = db.Products.First(p => p.IdProduct == id);
        db.Products.Remove(delete);
        await db.SaveChangesAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        using var db = new TipografiaContext();
        return await db.Products.FirstOrDefaultAsync(p => p.IdProduct == id);
    }
}