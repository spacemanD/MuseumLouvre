using DAL.EF.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IExhibitService
    {
        public Task<IEnumerable<Exhibit>> GetAllListAsync();

        public Task<Exhibit> AddAsync(Exhibit exemplar);

        public Task AddRangeAsync(IEnumerable<Exhibit> range);

        public void DeleteRangeAsync(IEnumerable<Exhibit> range);

        public void DeleteAsync(Exhibit exemplar);

        public void UpdateAsync(Exhibit exemplar);

        public void UpdateRangeAsync(IEnumerable<Exhibit> range);

        public ICollection<PopExhibit> GetTop10PopularExhibits();

        public ICollection<PopExhibit> GetLast10PopularExhibits();

        public void UpdateStatistics(int exhibitId);
    }
}
