

using Acr.UserDialogs;

using GPS_NotePad.Models;
using GPS_NotePad.Services.Interfaces;

using SQLite;

using System.Collections.Generic;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.Repository
{

    class Repository : IRepository
    {
        private readonly ISQLiteAsyncConnectionProvider _connectionProvider;
        private readonly SQLiteAsyncConnection _connection;

        public Repository(ISQLiteAsyncConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
            _connection = _connectionProvider.GetConnection();

            CreateTable<Loginin>();
            CreateTable<MarkerInfo>();
        }


        public async Task<int> DeleteAsync<T>(int id) where T : class, new()
        {
            return await _connection.DeleteAsync<T>(id);
        }

        public async Task<List<T>> GetDataAsync<T>(string table, string email) where T : class, new()
        {
            return await _connection.QueryAsync<T>("SELECT * FROM '" + table + "' WHERE email ='" + email + "'");
        }

        public async Task<bool> InsertAsync<T>(T profile) where T : class, new()
        {
            int u = await _connection.InsertAsync(profile);

            return (u > 0) ? true : false;
        }

        public async Task<int> UpdateAsync<T>(T profile) where T : class, new()
        {
            return await _connection.UpdateAsync(profile);
        }

        public void CreateTable<T>() where T : class, new()
        {
            try
            {
                _ = _connection.CreateTableAsync<T>();
            }
            catch
            {
                _ = UserDialogs.Instance.Alert("Restart the application", "Error", "Ok");
            }
        }
    }
}
