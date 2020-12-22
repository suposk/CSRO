using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IBaseDataStore<T>
    {
        Task<bool> AddItemAsync(T item);
        Task<bool> UpdateItemAsync(T item);
        Task<bool> DeleteItemAsync(int id);
        Task<T> GetItemByIdAsync(int id);
        Task<IList<T>> GetItemsAsync(bool forceRefresh = false);
        Task<IList<T>> GetItemsByTypeAsync(Enum type, bool forceRefresh = false);
        Task<IList<T>> GetItemsByParrentIdAsync(int parrentId, bool forceRefresh = false);
    }
}
