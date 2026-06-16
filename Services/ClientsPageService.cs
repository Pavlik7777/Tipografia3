using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tipografia3.Models;

namespace Tipografia3.Services;
public class ClientsPageService : IDBService<Client>
{
    public async Task<List<Client>> GetAsync()
    {
        using var db = new TipografiaContext();
        return await db.Clients.ToListAsync();
    }

    public async Task AddAsync(Client client)
    {
        using var db = new TipografiaContext();
        await db.Clients.AddAsync(client);
        await db.SaveChangesAsync();
    }

    public async Task EditAsync(Client client)
    {
        using var db = new TipografiaContext();
        var edit = db.Clients.First(p => p.IdClient == client.IdClient);
        edit.NameClient = client.NameClient;
        edit.Address = client.Address;
        db.Clients.Update(edit);
        await db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        using var db = new TipografiaContext();
        var delete = db.Clients.First(p => p.IdClient == id);
        db.Clients.Remove(delete);
        await db.SaveChangesAsync();
    }

    public async Task<Client?> GetByIdAsync(int id)
    {
        using var db = new TipografiaContext();
        return await db.Clients.FirstOrDefaultAsync(p => p.IdClient == id);
    }
}