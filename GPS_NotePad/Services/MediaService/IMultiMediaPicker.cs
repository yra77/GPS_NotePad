

using GPS_NotePad.Models;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace GPS_NotePad.Services.MediaService
{
    public interface IMultiMediaPicker
    {
        
        event EventHandler<IList<MediaFile>> OnMediaPickedCompleted;

        Task<IList<MediaFile>> PickPhotosAsync();
        void Clean();
    }
}
