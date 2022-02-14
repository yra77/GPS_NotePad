using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_NotePad.Services
{
    public interface WorkingToImagesService
    {
        string ResizeImage(string image, string nameImg, bool isGalery);
    }
}
