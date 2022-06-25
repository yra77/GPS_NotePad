
using GPS_NotePad.Models;
using GPS_NotePad.Services.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.MediaService
{
    class MediaService : IMediaService
    {

        private IWorkingToImagesService _resizeImage;
        private IMultiMediaPicker _multiMediaPicker;

        public MediaService(IWorkingToImagesService resizeImage,
                            IMultiMediaPicker multiMediaPicker)
        {
            _resizeImage = resizeImage;
            _multiMediaPicker = multiMediaPicker;
        }

        #region Interface ImediaService implamentation

        public async Task<string> OpenCamera()
        {
            Xamarin.Essentials.FileResult photo = await Xamarin.Essentials.MediaPicker.CapturePhotoAsync();

            if (photo != null)
            {
                string str = _resizeImage.ResizeImage(photo.FullPath, photo.FileName, true);
                return str;
            }
            return null;
        }

        public async Task<IList<string>> OpenGalery()
        {
            //Xamarin.Essentials.FileResult photo = await Xamarin.Essentials.MediaPicker.PickPhotoAsync();

            //if (photo != null)
            //{
            //    string str = _resizeImage.ResizeImage(photo.FullPath, photo.FileName, true);
            //    //Console.WriteLine("File Size in Bytes: " + new FileInfo(photo.FullPath).Length);
            //    //Console.WriteLine("File Size in Bytes: " + new FileInfo(str).Length);
            //    return str;
            //}

            IList<string> pathList = new List<string>();
            IList<MediaFile> media = await _multiMediaPicker.PickPhotosAsync();
            if (media != null && media.Count > 0)
            {
                foreach (MediaFile item in media)
                {
                    pathList.Add(_resizeImage.ResizeImage(item.Path, item.FileName, true));
                }

                return pathList;
            }

            return null;
        }

        public void SaveToAppFolder(string fileName)
        {

        }

        #endregion
    }
}
