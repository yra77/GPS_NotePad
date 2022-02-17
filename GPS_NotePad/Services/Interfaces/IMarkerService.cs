
using GPS_NotePad.Models;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.Interfaces
{
    public interface IMarkerService
    {
        Task<List<T>> GetData<T>(string table, string email) where T : class, new();
        Task<int> Update(MarkerInfo profile);
        Task<bool> Insert(MarkerInfo profile);
        Task<int> Delete<T>(int id) where T : class, new();
    }
}
