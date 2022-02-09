
using GPS_NotePad.Models;
using GPS_NotePad.Repository;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace GPS_NotePad.Services
{
    public interface ITo_RepositoryService
    {
        Task<List<T>> GetData<T>(string table, string email) where T : class, new();
        // Task<int> Update(AddProfile profile);
        Task<bool> Insert(Loginin profile);
        Task<bool> Insert(MarkerInfo profile);
        Task<int> Delete<T>(int id) where T : class, new();
    }

    class To_RepositoryService : ITo_RepositoryService
    {
        private readonly IRepository _repository;

        public To_RepositoryService(IRepository repository)
        {
            _repository = repository;
            _repository.CreateTable<Loginin>();
            _repository.CreateTable<MarkerInfo>();
        }

#region Public method,  Intarface implementation
        public async Task<int> Delete<T>(int id) where T : class, new()
        {
            return await _repository.Delete<T>(id);
        }

        public async Task<List<T>> GetData<T>(string table, string email) where T : class, new()
        {
            return await _repository.GetData<T>(table, email);
        }

        public async Task<bool> Insert(Loginin profile)
        {
            var res = await _repository.GetData<Loginin>("Loginin", profile.email);
            if (!res.Any())
            {
                return await _repository.Insert<Loginin>(profile);
            }
            else
                return false;
        }
        public async Task<bool> Insert(MarkerInfo profile)
        {
            return await _repository.Insert<MarkerInfo>(profile);
        }
        #endregion
    }
}
