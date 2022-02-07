using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GPS_NotePad.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace GPS_NotePad.Droid.Services
{
    class ResizeImageService : IResizeImageService
    {
        public string ResizeImage(string imagePath, string nameImg)
        {
            float height = 600;
            float width = 400;
            float newHeight = 0;
            float newWidth = 0;

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InSampleSize = 10;

            Bitmap originalImage = BitmapFactory.DecodeFile(imagePath, options);


            var originalHeight = originalImage.Height;
            var originalWidth = originalImage.Width;

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

            Bitmap resizedImg = Bitmap.CreateScaledBitmap(originalImage, (int)width, (int)height, false);//(int)newWidth, (int)newHeight, false);

            Bitmap resizedImage = Rotate(resizedImg);

            resizedImg.Recycle();
            originalImage.Recycle();

            using (MemoryStream ms = new MemoryStream())
            {
                resizedImage.Compress(Bitmap.CompressFormat.Png, 10, ms);

                resizedImage.Recycle();

                return SaveToFile(ms.ToArray(), nameImg);
            }
        }

        string SaveToFile(byte[] bitmapImg, string name)
        {
            // var path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var path = System.IO.Path.Combine(FileSystem.AppDataDirectory, name);
            var stream = new FileStream(path, FileMode.Create);
            stream.Write(bitmapImg, 0, bitmapImg.Length);
            stream.Flush();
            stream.Close();

            return path;
        }
        Bitmap Rotate(Bitmap bitmap)
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