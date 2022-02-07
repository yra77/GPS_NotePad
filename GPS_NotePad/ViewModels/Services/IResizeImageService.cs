using System;
using System.Collections.Generic;
using System.Text;

namespace GPS_NotePad.ViewModels.Services
{
    public interface IResizeImageService
    {
        string ResizeImage(string image, string nameImg);
    }
}
