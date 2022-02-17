

using Acr.UserDialogs;
using GPS_NotePad.Services.Interfaces;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GPS_NotePad.Repository
{

    class Repository: IRepository
    {
        private readonly ISQLiteAsyncConnectionProvider _connectionProvider;
        private readonly SQLiteAsyncConnection _connection;

        public Repository(ISQLiteAsyncConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _connection = _connectionProvider.GetConnection();
        }


        public async Task<int> Delete<T>(int id) where T : class, new()
        {
            return await _connection.DeleteAsync<T>(id);
        }

        public async Task<List<T>> GetData<T>(string table, string email) where T : class, new()
        {
            return await _connection.QueryAsync<T>("SELECT * FROM '" + table + "' WHERE email ='" + email + "'");
        }

        public async Task<bool> Insert<T>(T profile) where T : class, new()
        {
           var u =  await _connection.InsertAsync(profile);
            if(u > 0)
            return true;
            return false;
        }

        public async Task<int> Update<T>(T profile) where T : class, new()
        {
            return await _connection.UpdateAsync(profile);
        }

        public void CreateTable<T>() where T : class, new()
        {
                try { _connection.CreateTableAsync<T>(); }
                catch { UserDialogs.Instance.Alert("Restart the application", "Error", "Ok"); }
        }
    }
}
