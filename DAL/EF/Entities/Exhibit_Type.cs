
using System.ComponentModel.DataAnnotations;

namespace DAL.EF.Entities
{
    public class Exhibit_Type
    {
        [Key]
        public int ExhibitTypeId { get; set; }

        public string Value { get; set; }
    }
}
