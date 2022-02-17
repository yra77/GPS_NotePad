
using GPS_NotePad.Services.Interfaces;
using SQLite;
using System.IO;

namespace GPS_NotePad.Droid.Services
{
    class SQLiteAsyncConnectionProvider : ISQLiteAsyncConnectionProvider
    {
        private SQLiteAsyncConnection Connection { get; set; }

        public SQLiteAsyncConnection GetConnection()
        {
            if (Connection != null) { return Connection; }

            var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            path = Path.Combine(path, "database.db3");
            return Connection = new SQLiteAsyncConnection(path);
        }
    }
}