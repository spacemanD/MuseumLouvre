using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DAL.EF.Entities
{
    public class Country
    {
        [Key]
        public int CountryId { get; set; }

        public string Value { get; set; }
    }
}
