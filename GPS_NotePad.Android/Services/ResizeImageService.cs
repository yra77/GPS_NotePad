

using GPS_NotePad.Services.Interfaces;
using Android.Graphics;
using System.IO;
using Xamarin.Essentials;


namespace GPS_NotePad.Droid.Services
{
    class ResizeImageService : IWorkingToImagesService
    {
        public string ResizeImage(string imagePath, string nameImg, bool isGalery)
        {
            float height = 1200;
            float width = 1200;
           // float newHeight = 0;
          //  float newWidth = 0;

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InSampleSize = 1;

            Bitmap originalImage = BitmapFactory.DecodeFile(imagePath, options);
            if (isGalery)
                originalImage = Rotate(originalImage);

            //var originalHeight = originalImage.Height;
            //var originalWidth = originalImage.Width;

            //if (originalHeight > originalWidth)
            //{
            //    newHeight = height;
            //    float ratio = originalHeight / height;
            //    newWidth = originalWidth / ratio;
            //}
            //else
            //{
            //    newWidth = width;
            //    float ratio = originalWidth / width;
            //    newHeight = originalHeight / ratio;
            //}

            Bitmap resizedImg = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);//(int)newWidth, (int)newHeight, false);//

            //if (isGalery)
            //    resizedImg = Rotate(resizedImg);


            originalImage.Recycle();

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImg.Compress(Bitmap.CompressFormat.Png, 100, ms);

                resizedImg.Recycle();

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

        private Bitmap Rotate(Bitmap bitmap)
        {
            Matrix matrix = new Matrix();
            if (bitmap.Width > bitmap.Height)
            {
                matrix.SetRotate(90);
            }

            return Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);
        }
    }
}