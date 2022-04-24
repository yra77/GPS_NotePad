

using System.Collections.ObjectModel;


namespace GPS_NotePad.Models
{

    public class SubPublicTransport
    {
        public string Text { get; set; }
        public string Icon { get; set; }
    }

    public class PublicTransport : ObservableCollection<SubPublicTransport>
    {   
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public string DirectionNum { get; set; }
        public int Id { get; set; }
    }

}
