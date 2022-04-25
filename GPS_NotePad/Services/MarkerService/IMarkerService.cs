
using GPS_NotePad.Models;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.MarkerService
{
    public interface IMarkerService
    {
        Task<List<T>> GetDataAsync<T>(string table, string email) where T : class, new();
        Task<bool> UpdateAsync(MarkerInfo profile);
        Task<bool> InsertAsync(MarkerInfo profile);
        Task<int> DeleteAsync<T>(int id) where T : class, new();
    }
}
