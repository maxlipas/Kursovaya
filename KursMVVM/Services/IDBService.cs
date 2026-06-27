using KursMVVM.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KursMVVM.Services
{
    public interface IDBService<T> where T : class
    {
        Task<List<T>> get();
        Task Add(T t);
        Task Edit(T t);
        Task Delete(int id);
        Task<T?> getById(int id);
    }
}
