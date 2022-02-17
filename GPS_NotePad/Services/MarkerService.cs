
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

        public async Task<int> Delete<T>(int id) where T : class, new()
        {
            return await _repository.Delete<T>(id);
        }

        public async Task<List<T>> GetData<T>(string table, string email) where T : class, new()
        {
            return await _repository.GetData<T>(table, email);
        }

        public async Task<bool> Insert(MarkerInfo profile)
        {
            return await _repository.Insert<MarkerInfo>(profile);
        }

        public Task<int> Update(MarkerInfo profile)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
