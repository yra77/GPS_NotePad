using System.Threading.Tasks;

namespace GPS_NotePad.Services.TranslatorInterfaces
{
    public interface IMicrophoneService
    {
        Task<bool> GetPermissionAsync();
        void OnRequestPermissionResult(bool isGranted);
    }
}
