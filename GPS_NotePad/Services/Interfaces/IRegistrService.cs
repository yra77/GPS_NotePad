
using GPS_NotePad.Models;
using System.Threading.Tasks;

namespace GPS_NotePad.Services.Interfaces
{
    public interface IRegistrService
    {
        Task<(bool, string)> Registr(Loginin profile, string passConfirm);
        Task<bool> RegistrGoogle(Loginin profile);
    }
}
