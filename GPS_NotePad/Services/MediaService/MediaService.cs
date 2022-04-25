
using GPS_NotePad.Services.Interfaces;

using System.Threading.Tasks;


namespace GPS_NotePad.Services.MediaService
{
    class MediaService : IMediaService
    {

        private IWorkingToImagesService _resizeImage;

        public MediaService(IWorkingToImagesService resizeImage)
        {
            _resizeImage = resizeImage;
        }

        #region Interface ImediaService implamentation
        public async Task<string> OpenCamera()
        {
            Xamarin.Essentials.FileResult photo = await Xamarin.Essentials.MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                string str = _resizeImage.ResizeImage(photo.FullPath, photo.FileName, false);
                return str;
            }
            return null;
        }

        public async Task<string> OpenGalery()
        {
            Xamarin.Essentials.FileResult photo = await Xamarin.Essentials.MediaPicker.PickPhotoAsync();

            if (photo != null)
            {
                string str = _resizeImage.ResizeImage(photo.FullPath, photo.FileName, false);
                //Console.WriteLine("File Size in Bytes: " + new FileInfo(photo.FullPath).Length);
                //Console.WriteLine("File Size in Bytes: " + new FileInfo(str).Length);
                return str;
            }
            return null;
        }

        public void SaveToAppFolder(string fileName)
        {
           
        }

        #endregion
    }
}
