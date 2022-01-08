using DAL.EF.Entities;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IExhibitService
    {
        public IEnumerable<Exhibit> GetAllListAsync();

        public Task<Exhibit> AddAsync(Exhibit exemplar);

        public Task AddRangeAsync(IEnumerable<Exhibit> range);

        public Task DeleteRangeAsync(IEnumerable<Exhibit> range);

        public Task DeleteAsync(Exhibit exemplar);

        public Task UpdateAsync(Exhibit exemplar);

        public Task UpdateRangeAsync(IEnumerable<Exhibit> range);

        public ICollection<PopExhibit> GetTop10PopularExhibits();

        public ICollection<PopExhibit> GetLast10PopularExhibits();

        public void UpdateStatistics(int exhibitId);

        public Task ProccessFile(FileModel file);

        public IEnumerable<Exhibit> GetAllListAsyncNonAuthors();

        public IEnumerable<Exhibit> GetAllListAsyncWithNonCollection();
    }
}
