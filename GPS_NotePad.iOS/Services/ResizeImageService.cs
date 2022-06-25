

using GPS_NotePad.Services.Interfaces;

using System;
using System.Drawing;
using UIKit;

namespace GPS_NotePad.iOS.Services
{
    class ResizeImageService : IWorkingToImagesService
    {
        public string ResizeImage(string image, string nameImg, bool isGalery)
        {

            UIImage originalImage = UIImage.FromFile(image);

            float height = 1200;
            float width = 1200;

            UIGraphics.BeginImageContext(new SizeF(width, height));
            originalImage.Draw(new RectangleF(0, 0, width, height));
            var resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();


            using (var bytesImagen = resizedImage.AsPNG())
            {

                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var path = System.IO.Path.Combine(documents, nameImg);
                bytesImagen.Save(path, true);

                return path;
            }

        }

    }
}