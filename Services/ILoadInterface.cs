using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tipografia3.Services;

public interface ILoadInterface<T>
{
    Task LoadListAsync(string path);
    Task UploadAsync(string path, List<T> list);
}