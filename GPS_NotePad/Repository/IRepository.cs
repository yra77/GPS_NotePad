using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPS_NotePad.Repository
{
    public interface IRepository
    {
        Task<List<T>> GetData<T>(string table, string email) where T : class, new();
        Task<int> Update<T>(T profile) where T : class, new();
        Task<bool> Insert<T>(T profile) where T : class, new();
        Task<int> Delete<T>(int id) where T : class, new();
        void CreateTable<T>() where T : class, new();
    }

}
