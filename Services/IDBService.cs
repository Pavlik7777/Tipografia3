using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tipografia3.Services;

public interface IDBService<T>
{
    Task<List<T>> GetAsync();
    Task AddAsync(T item);
    Task EditAsync(T item);
    Task DeleteAsync(int id);
    Task<T?> GetByIdAsync(int id);
}