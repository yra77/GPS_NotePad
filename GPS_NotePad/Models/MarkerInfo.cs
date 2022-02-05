using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.Maps;

namespace GPS_NotePad.Models
{
    [Table("MarkerInfo")]
    class MarkerInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        [Unique]
        [MaxLength(40)]
        public string email { get; set; }
        [MaxLength(40)]
        public string Label { get; set; }
        [MaxLength(70)]
        public string Address { get; set; }
        public Position Position { get; set; }
        [MaxLength(200)]
        public string ImagePath { get; set; }

    }
}
