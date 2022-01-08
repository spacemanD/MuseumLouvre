using DAL.EF.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace DAL.EF.Entities
{
    public class Exhibit
    {
        public int ExhibitId { get; set; }
        public string Name { get; set; }
        public int? AuthorId { get; set; }
        public int? CollectionId { get; set; }

        [DataType(DataType.Date)]
        public int? CreationYear { get; set; }
        public string Description { get; set; }
        public ExhibitType Type { get; set; }

        [DataType(DataType.Date)]
        public long? Cost { get; set; }
        public Enums.ArtDirection? Direction { get; set; }
        public string Materials { get; set; }
        public CountryList? Country { get; set; }
        public Author Author { get; set; }
        public Collection Collection { get; set; }
    }
}
