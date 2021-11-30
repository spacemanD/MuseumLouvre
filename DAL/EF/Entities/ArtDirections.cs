using System.ComponentModel.DataAnnotations;

namespace DAL.EF.Entities
{
    public class ArtDirections
    {
        [Key]
        public int ArtDirectionId { get; set; }

        public string Value { get; set; }
    }
}
