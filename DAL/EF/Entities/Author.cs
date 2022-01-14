using DAL.EF.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.EF.Entities
{
    public class Author : IValidatableObject
    {
        public int AuthorId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string MiddleName { get; set; }

        public DateTime? BirthDate { get; set; }
        public DateTime? DeathDate { get; set; }

        public CountryList? Country { get; set; }

        public virtual ICollection<Exhibit> Exhibits { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.DeathDate <= this.BirthDate)
                yield return new ValidationResult("It died before birth", new[] { nameof(this.DeathDate) });

            if (this.BirthDate >= this.DeathDate)
                yield return new ValidationResult("It born after death", new[] { nameof(this.BirthDate) });
        }
    }
}
