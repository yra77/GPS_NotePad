
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.MediaService
{
    public interface IMediaService
    {
        void SaveToAppFolder(string fileName);
        Task<IList<string>> OpenGalery();
        Task<string> OpenCamera();
    }
}
