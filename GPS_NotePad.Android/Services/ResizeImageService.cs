

using GPS_NotePad.Services.Interfaces;
using Android.Graphics;
using System.IO;
using Xamarin.Essentials;
using Android.Media;

namespace GPS_NotePad.Droid.Services
{
    class ResizeImageService : IWorkingToImagesService
    {
        public string ResizeImage(string imagePath, string nameImg, bool isGalery)
        {

            BitmapFactory.Options options = new BitmapFactory.Options();

            Bitmap originalImage = BitmapFactory.DecodeFile(imagePath);

            //if (isGalery)
            //{
            //    originalImage = Rotate(originalImage);
            //}

            if (originalImage.Height > 1400)
            {
                int height = originalImage.Height / 3;
                int width = originalImage.Width / 3;

                originalImage = Bitmap.CreateScaledBitmap(originalImage, width, height, true);

                if (isGalery)
                {
                    originalImage = Rotate(originalImage, imagePath);
                }
            }

            using (MemoryStream ms = new MemoryStream())
            {
                originalImage.Compress(Bitmap.CompressFormat.Png, 100, ms);

                originalImage.Recycle();

                return SaveToFile(ms.ToArray(), nameImg);
            }
        }

        private string SaveToFile(byte[] bitmapImg, string name)
        {
            // var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = System.IO.Path.Combine(FileSystem.AppDataDirectory, name);
            var stream = new FileStream(path, FileMode.Create);
            stream.Write(bitmapImg, 0, bitmapImg.Length);
            stream.Flush();
            stream.Close();

            return path;
        }
        
        private Bitmap Rotate(Bitmap bitmap, string imagePath)
        {
            Matrix matrix = new Matrix();

            if (bitmap.Width > bitmap.Height)
            {
                matrix.SetRotate(GetRotation(imagePath));
            }

            return Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
        }

        private int GetRotation(string filePath)
        {
            using (ExifInterface ei = new ExifInterface(filePath))
            {
                Orientation orientation = (Android.Media.Orientation)ei.GetAttributeInt(ExifInterface.TagOrientation, 
                                    (int)Android.Media.Orientation.Normal);

                switch (orientation)
                {
                    case Android.Media.Orientation.Rotate90:
                        return 90;
                    case Android.Media.Orientation.Rotate180:
                        return 180;
                    case Android.Media.Orientation.Rotate270:
                        return 270;
                    default:
                        return 0;
                }
            }
        }


    }
}