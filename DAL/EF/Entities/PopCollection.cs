using System.ComponentModel.DataAnnotations;

namespace DAL.EF.Entities
{
    public class PopCollection
    {
        [Key]
        public int CollectionId { get; set; }

        public int Rate { get; set; }

        public Collection Collection { get; set; }
    }
}
