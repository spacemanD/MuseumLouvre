using DAL.EF.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IAuthorService
    {
        public Task<IEnumerable<Author>> GetAllListAsync();

        public Task<Author> AddAsync(Author exemplar);

        public Task AddRangeAsync(IEnumerable<Author> range);

        public void DeleteRangeAsync(IEnumerable<Author> range);

        public void DeleteAsync(Author exemplar);

        public void UpdateAsync(Author exemplar);

        public void UpdateRangeAsync(IEnumerable<Author> range);

        public ICollection<PopAuthor> GetTop10PopularAuthors();

        public ICollection<PopAuthor> GetLast10PopularAuthors();

        public void UpdateStatistics(int authorId);
    }
}
