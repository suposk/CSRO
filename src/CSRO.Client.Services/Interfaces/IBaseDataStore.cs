using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IBaseDataStore<T>
    {
        Task<T> AddItemAsync(T item);
        Task<T> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(int id);
        Task<T> GetItemByIdAsync(int id);
        Task<List<T>> GetItemsAsync(bool forceRefresh = false);
        Task<List<T>> GetItemsByTypeAsync(Enum type, bool forceRefresh = false);
        Task<List<T>> GetItemsByParrentIdAsync(int parrentId, bool forceRefresh = false);
    }
}
