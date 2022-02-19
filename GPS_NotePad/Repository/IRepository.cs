using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPS_NotePad.Repository
{
    public interface IRepository
    {
        Task<List<T>> GetDataAsync<T>(string table, string email) where T : class, new();
        Task<int> UpdateAsync<T>(T profile) where T : class, new();
        Task<bool> InsertAsync<T>(T profile) where T : class, new();
        Task<int> DeleteAsync<T>(int id) where T : class, new();
        void CreateTable<T>() where T : class, new();
    }

}
