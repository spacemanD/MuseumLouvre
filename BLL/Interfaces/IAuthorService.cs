using DAL.EF.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAuthorService
    {
        public IEnumerable<Author> GetAllListAsync();

        public Task<Author> AddAsync(Author exemplar);

        public Task AddRangeAsync(IEnumerable<Author> range);

        public Task DeleteRangeAsync(IEnumerable<Author> range);

        public Task DeleteAsync(Author exemplar);

        public Task UpdateAsync(Author exemplar);

        public Task UpdateRangeAsync(IEnumerable<Author> range);

        public ICollection<PopAuthor> GetTop10PopularAuthors();

        public ICollection<PopAuthor> GetLast10PopularAuthors();

        public void UpdateStatistics(int authorId);
    }
}
