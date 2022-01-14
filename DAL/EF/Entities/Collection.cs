using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DAL.EF.Entities
{
    public class Collection : IValidatableObject
    {
        public int CollectionId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public List<Exhibit> Exhibits { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (this.EndTime <= this.StartTime)
                yield return new ValidationResult("It ends before start", new[] { nameof(this.EndTime) });

            if (this.StartTime >= this.EndTime)
                yield return new ValidationResult("It starts after ends", new[] { nameof(this.StartTime) });
        }
    }
}

