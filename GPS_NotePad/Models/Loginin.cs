
using SQLite;
using System;

namespace GPS_NotePad.Models
{
        [Table("Loginin")]
        public class Loginin
        {
            [PrimaryKey, AutoIncrement]
            public int Id { get; set; }

        [NotNull]
        [MaxLength(20)]
        public string name { get; set; }

            [NotNull]
            [Unique]
            [MaxLength(40)]
            public string email { get; set; }
            [NotNull]
            [MaxLength(20)]
            public string password { get; set; }

            [NotNull]
            public DateTime DateCreated { get; set; }
        }
}
