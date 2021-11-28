using DAL.EF.Entities.Enums;
using System;
using System.Collections.Generic;

namespace DAL.EF.Entities
{
    public class Author
    {
        public int AuthorId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string MiddleName { get; set; }

        public DateTime? BirthDate { get; set; }

        public DateTime? DeathDate { get; set; }

        public CountryList? Country { get; set; }

        public int CountryId { get; set; }

        public virtual ICollection<Exhibit> Pictures { get; set; }
    }
}
