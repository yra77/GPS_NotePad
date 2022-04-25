
using GPS_NotePad.Models;
using GPS_NotePad.Services.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.MarkerService
{

    class MarkerService : IMarkerService
    {

        private readonly IRepository _repository;

        public MarkerService(IRepository repository)
        {
            _repository = repository;
        }


        #region --- Intarface IMarkerService implementation ----

        public async Task<int> DeleteAsync<T>(int id) where T : class, new()
        {
            return await _repository.DeleteAsync<T>(id);
        }

        public async Task<List<T>> GetDataAsync<T>(string table, string email) where T : class, new()
        {
            return await _repository.GetDataAsync<T>(table, email);
        }

        public async Task<bool> InsertAsync(MarkerInfo profile)
        {
            return await _repository.InsertAsync<MarkerInfo>(profile);
        }

        public async Task<bool> UpdateAsync(MarkerInfo profile)
        {
            int res = await _repository.UpdateAsync<MarkerInfo>(profile);
            Console.WriteLine("YYYYYYYYYYyy" + res);
            return res > 0;
        }

        #endregion
    }
}
