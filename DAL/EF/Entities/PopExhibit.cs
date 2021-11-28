
using System.ComponentModel.DataAnnotations;

namespace DAL.EF.Entities
{
    public class PopExhibit
    {
        [Key]
        public int ExhibitId { get; set; }

        public int Rate { get; set; }

        public Exhibit Exhibit { get; set; }
    }
}
