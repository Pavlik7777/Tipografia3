using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tipografia3.Models;

namespace Tipografia3.Services;
public class DogovorPageService : IDBService<Dogovor>
{
    public async Task<List<Dogovor>> GetAsync()
    {
        using var db = new TipografiaContext();
        return await db.Dogovors.ToListAsync();
    }

    public async Task AddAsync(Dogovor dogovor)
    {
        using var db = new TipografiaContext();
        await db.Dogovors.AddAsync(dogovor);
        await db.SaveChangesAsync();
    }

    public async Task EditAsync(Dogovor dogovor)
    {
        using var db = new TipografiaContext();
        var edit = db.Dogovors.First(p => p.IdDogovor == dogovor.IdDogovor);
        edit.IdClient = dogovor.IdClient;
        edit.NumberDogovor = dogovor.NumberDogovor;
        edit.DateOforml = dogovor.DateOforml;
        edit.DateDue = dogovor.DateDue;
        db.Dogovors.Update(edit);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var db = new TipografiaContext();
        var delete = db.Dogovors.First(p => p.IdDogovor == id);
        db.Dogovors.Remove(delete);
        await db.SaveChangesAsync();
    }

    public async Task<Dogovor?> GetByIdAsync(int id)
    {
        using var db = new TipografiaContext();
        return await db.Dogovors.FirstOrDefaultAsync(p => p.IdDogovor == id);
    }
}