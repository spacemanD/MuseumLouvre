
using System.ComponentModel.DataAnnotations;

namespace DAL.EF.Entities
{
    public class PopAuthor
    {
        [Key]
        public int AuthorId { get; set; }

        public int Rate { get; set; }

        public Author Author { get; set; }
    }
}
