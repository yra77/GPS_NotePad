
using GPS_NotePad.Services.Interfaces;

using System.Threading.Tasks;


namespace GPS_NotePad.Services.MediaService
{
    class MediaService : IMediaService
    {

        private WorkingToImagesService _resizeImage;

        public MediaService(WorkingToImagesService resizeImage)
        {
            _resizeImage = resizeImage;
        }

        #region ----------------- Interface ImediaService implamentation -----------------
        public async Task<string> OpenCamera()
        {
            var photo = await Xamarin.Essentials.MediaPicker.CapturePhotoAsync();
            if (photo != null)// do not remove - will be error
            {
                string str = _resizeImage.ResizeImage(photo.FullPath, photo.FileName, false);
                return str;
            }
            return Constants.Constant.DEFAULT_IMAGE;
        }

        public async Task<string> OpenGalery()
        {
            var photo = await Xamarin.Essentials.MediaPicker.PickPhotoAsync();
            if (photo != null)// do not remove - will be error
            {
                string str = _resizeImage.ResizeImage(photo.FullPath, photo.FileName, false);
                //Console.WriteLine("File Size in Bytes: " + new FileInfo(photo.FullPath).Length);
                //Console.WriteLine("File Size in Bytes: " + new FileInfo(str).Length);
                return str;
            }
            return Constants.Constant.DEFAULT_IMAGE;
        }

        public void SaveToAppFolder(string fileName)
        {
           
        }
        #endregion
    }
}
