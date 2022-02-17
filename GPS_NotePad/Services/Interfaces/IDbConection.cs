
using SQLite;

namespace GPS_NotePad.Services.Interfaces
{
    public interface ISQLiteAsyncConnectionProvider
    {
        SQLiteAsyncConnection GetConnection();
    }
}
