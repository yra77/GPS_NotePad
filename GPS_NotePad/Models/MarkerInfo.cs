﻿

using SQLite;


namespace GPS_NotePad.Models
{
    [Table("MarkerInfo")]
    public class MarkerInfo
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull]
        [MaxLength(40)]
        public string email { get; set; }

        [NotNull]
        [MaxLength(40)]
        public string Label { get; set; }

        [NotNull]
        [MaxLength(70)]
        public string Address { get; set; }

        [NotNull]
        [MaxLength(40)]
        public double Latitude { get; set; }

        [NotNull]
        [MaxLength(40)]
        public double Longitude { get; set; }

        public string ImagePath { get; set; }

        [NotNull]
        public string LikeImage { get; set; }

    }
}
