
using GPS_NotePad.Models.Services;
using SQLite;
using System;
using System.IO;

namespace GPS_NotePad.iOS.Services
{
    class SQLiteAsyncConnectionProvider : ISQLiteAsyncConnectionProvider
    {
        private SQLiteAsyncConnection Connection { get; set; }

        public SQLiteAsyncConnection GetConnection()
        {
            if (Connection != null) { return Connection; }

            var path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            path = Path.Combine(path, "..", "Library", "database.db3");
            return Connection = new SQLiteAsyncConnection(path);
        }
    }
}