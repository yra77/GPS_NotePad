using GPS_NotePad.Models;
using GPS_NotePad.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPS_NotePad.ViewModels.Services
{
    public interface ITo_RepositoryService
    {
        Task<List<T>> GetData<T>(string table, string email) where T : class, new();
        // Task<int> Update(AddProfile profile);
        Task<bool> Insert(Loginin profile);
        // Task<bool> Insert(string login, string nickName, string name, string description, string imageUri);
        Task<int> Delete<T>(int id) where T : class, new();
    }
    class To_RepositoryService : ITo_RepositoryService
    {
        private readonly IRepository repository;

        public To_RepositoryService(IRepository _repository)
        {
            repository = _repository;
            repository.CreateTable<Loginin>();
        }
        public async Task<int> Delete<T>(int id) where T : class, new()
        {
            return await repository.Delete<T>(id);
        }

        public async Task<List<T>> GetData<T>(string table, string email) where T : class, new()
        {
            return await repository.GetData<T>(table, email);
        }

        public async Task<bool> Insert(Loginin profile)
        {
            var res = await repository.GetData<Loginin>("Loginin", profile.email);
            if (!res.Any())
            {
                return await repository.Insert(profile);
            }
            else
                return false;
        }
    }
}
