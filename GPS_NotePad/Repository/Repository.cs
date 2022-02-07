

using Acr.UserDialogs;
using GPS_NotePad.Services;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
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
    class Repository: IRepository
    {
        private ISQLiteAsyncConnectionProvider ConnectionProvider;
        private SQLiteAsyncConnection Connection;

        public Repository(ISQLiteAsyncConnectionProvider connectionProvider)
        {
            ConnectionProvider = connectionProvider;
            Connection = ConnectionProvider.GetConnection();
        }


        public async Task<int> Delete<T>(int id) where T : class, new()
        {
            return await Connection.DeleteAsync<T>(id);
        }

        public async Task<List<T>> GetData<T>(string table, string email) where T : class, new()
        {
            return await Connection.QueryAsync<T>("SELECT * FROM '" + table + "' WHERE email ='" + email + "'");
        }

        public async Task<bool> Insert<T>(T profile) where T : class, new()
        {
           var u =  await Connection.InsertAsync(profile);
            if(u > 0)
            return true;
            return false;
        }

        public async Task<int> Update<T>(T profile) where T : class, new()
        {
            return await Connection.UpdateAsync(profile);
        }

        public void CreateTable<T>() where T : class, new()
        {
                try { Connection.CreateTableAsync<T>(); }
                catch { UserDialogs.Instance.Alert("Restart the application", "Error", "Ok"); }
        }
    }
}
