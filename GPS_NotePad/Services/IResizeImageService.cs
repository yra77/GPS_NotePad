using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_NotePad.Services
{
    public interface IResizeImageService
    {
        string ResizeImage(string image, string nameImg);
    }
}
