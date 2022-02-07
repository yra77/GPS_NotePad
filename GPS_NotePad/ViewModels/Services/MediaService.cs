using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GPS_NotePad.ViewModels.Services
{
    public interface IMediaService
    {
        void SaveToAppFolder(string fileName);
        Task<string> OpenGalery();
        Task<string> OpenCamera();

    }

    class MediaService : IMediaService
    {

        private IResizeImageService _resizeImage;
        public MediaService(IResizeImageService resizeImage)
        {
            _resizeImage = resizeImage;
        }
        public async Task<string> OpenCamera()
        {
            var photo = await Xamarin.Essentials.MediaPicker.CapturePhotoAsync();
            if (photo != null)// do not remove - will be error
            {
                string str = _resizeImage.ResizeImage(photo.FullPath, photo.FileName);
                return str;
            }
            return "paris.png";
        }

        public async Task<string> OpenGalery()
        {
            var photo = await Xamarin.Essentials.MediaPicker.PickPhotoAsync();
            if (photo != null)// do not remove - will be error
            {
                string str = _resizeImage.ResizeImage(photo.FullPath, photo.FileName);
                //Console.WriteLine("File Size in Bytes: " + new FileInfo(photo.FullPath).Length);
                //Console.WriteLine("File Size in Bytes: " + new FileInfo(str).Length);
                return str;
            }
            return "paris.png";
        }

        public void SaveToAppFolder(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
