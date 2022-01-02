using DAL.EF.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface ICollectionService
    {
        public IEnumerable<Collection> GetAllListAsync();

        public Task<Collection> AddAsync(Collection exemplar);

        public Task AddRangeAsync(IEnumerable<Collection> range);

        public Task DeleteRangeAsync(IEnumerable<Collection> range);

        public Task DeleteAsync(Collection exemplar);

        public Task UpdateAsync(Collection exemplar);

        public Task UpdateRangeAsync(IEnumerable<Collection> range);

        public ICollection<PopCollection> GetTop10PopularCollections();

        public ICollection<PopCollection> GetLast10PopularCollections();
        
        public void UpdateStatistics(int exhibitId);
    }
}
