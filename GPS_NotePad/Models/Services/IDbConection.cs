
using SQLite;

namespace GPS_NotePad.Models.Services
{
    public interface ISQLiteAsyncConnectionProvider
    {
        SQLiteAsyncConnection GetConnection();
    }
}
