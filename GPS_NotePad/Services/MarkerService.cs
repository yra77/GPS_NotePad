
using GPS_NotePad.Models;
using GPS_NotePad.Repository;
using GPS_NotePad.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GPS_NotePad.Services
{

    class MarkerService : IMarkerService
    {

        private readonly IRepository _repository;

        public MarkerService(IRepository repository)
        {
            _repository = repository;
            _repository.CreateTable<MarkerInfo>();
        }


        #region Public method,  Intarface IRegisterService implementation

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

        public async Task<int> UpdateAsync(MarkerInfo profile)
        {
            return await _repository.UpdateAsync(profile);
        }

        #endregion
    }
}
