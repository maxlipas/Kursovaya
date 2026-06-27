using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace KursMVVM.Services
{
    public interface ILoadInteface<T>
    {
        Task LoadList(string path);
        Task Upload(string path, List<T> list);
    }
}
