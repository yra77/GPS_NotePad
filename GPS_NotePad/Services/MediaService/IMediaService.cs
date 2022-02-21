
using System.Threading.Tasks;

namespace GPS_NotePad.Services.MediaService
{
    public interface IMediaService
    {
        void SaveToAppFolder(string fileName);
        Task<string> OpenGalery();
        Task<string> OpenCamera();
    }
}
