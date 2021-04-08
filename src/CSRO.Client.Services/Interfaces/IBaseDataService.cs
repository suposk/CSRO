using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IBaseDataService<T>
    {
        Task<T> AddItemAsync(T item);
        //Task<T> UpdateItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(int id);
        Task<T> GetItemByIdAsync(int id);
        Task<List<T>> GetItemsAsync();
        virtual Task<List<T>> GetItemsByUserId(string userId) { throw new NotImplementedException(); }
        virtual Task<List<T>> GetItemsByParrentIdAsync(int parrentId) { throw new NotImplementedException(); }
    }
}
