using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tipografia3.Models;

namespace Tipografia3.Services;
public class CehPageService : IDBService<TipografCeh>
{
    public async Task<List<TipografCeh>> GetAsync()
    {
        using var db = new TipografiaContext();
        return await db.TipografCehs.ToListAsync();
    }

    public async Task AddAsync(TipografCeh ceh)
    {
        using var db = new TipografiaContext();
        await db.TipografCehs.AddAsync(ceh);
        await db.SaveChangesAsync();
    }

    public async Task EditAsync(TipografCeh ceh)
    {
        using var db = new TipografiaContext();
        var edit = db.TipografCehs.First(p => p.IdTipograf == ceh.IdTipograf);
        edit.NumberCeh = ceh.NumberCeh;
        edit.NameCeh = ceh.NameCeh;
        edit.BossCeh = ceh.BossCeh;
        edit.PhoneCeh = ceh.PhoneCeh;
        db.TipografCehs.Update(edit);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var db = new TipografiaContext();
        var delete = db.TipografCehs.First(p => p.IdTipograf == id);
        db.TipografCehs.Remove(delete);
        await db.SaveChangesAsync();
    }

    public async Task<TipografCeh?> GetByIdAsync(int id)
    {
        using var db = new TipografiaContext();
        return await db.TipografCehs.FirstOrDefaultAsync(p => p.IdTipograf == id);
    }
}