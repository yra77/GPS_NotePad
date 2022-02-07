
using SQLite;

namespace GPS_NotePad.Services
{
    public interface ISQLiteAsyncConnectionProvider
    {
        SQLiteAsyncConnection GetConnection();
    }
}
