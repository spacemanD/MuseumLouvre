using BLL.Interfaces;
using DAL;
using DAL.EF.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class CollectionService : ICollectionService
    {
        private readonly IRepository _repos;

        private readonly ILogger _logger;

        private readonly AppDbContext _context;
        public CollectionService(ILogger<ExhibitService> logger, IRepository repository, AppDbContext context)
        {
            _logger = logger;
            _repos = repository;
            _context = context;
        }
        public Task<Collection> AddAsync(Collection exemplar)
        {
            return _repos.AddAsync(exemplar);
        }

        public Task AddRangeAsync(IEnumerable<Collection> range)
        {
            return _repos.AddRangeAsync(range);
        }

        public Task DeleteAsync(Collection exemplar)
        {
            return _repos.DeleteAsync(exemplar);
        }

        public Task DeleteRangeAsync(IEnumerable<Collection> range)
        {
            return _repos.DeleteRangeAsync(range);
        }

        public Collection GetById(int id)
        {
            return _repos.GetAsync<Collection>(false, x => x.CollectionId == id,
            include: source => source
            .Include(a => a.Exhibits)).Result;     
        }
        public IEnumerable<Collection> GetAllListAsync()
        {
            var list = _repos.GetRange<Collection>(false, x => x != null,
            include: source => source
            .Include(a => a.Exhibits))
            .ToList();

            if (list != null)
            {
                _logger.LogInformation("Successfully retrieved from DB");
            }
            else
            {
                _logger.LogInformation("Failed when this retrieved from DB");
            }
            return list;
        }

        public ICollection<PopCollection> GetTop10PopularCollections()
        {
            var users = _repos.GetRangeAsync<PopCollection>(false, x => x != null,
            include: source => source
            .Include(a => a.Collection)
            .ThenInclude(z => z.Exhibits)
            .ThenInclude(z => z.Author))
            .Result.OrderByDescending(x => x.Rate).Take(10).ToList();

            return users;
        }

        public ICollection<PopCollection> GetLast10PopularCollections()
        {

            var users = _repos.GetRangeAsync<PopCollection>(false, x => x != null,
            include: source => source
            .Include(a => a.Collection)
            .ThenInclude(z => z.Exhibits)
            .ThenInclude(z => z.Author))
            .Result.OrderBy(x => x.Rate).Take(10).ToList();

            return users;
        }

        public Task UpdateAsync(Collection exemplar)
        {
            return _repos.UpdateAsync(exemplar);
        }

        public Task UpdateRangeAsync(IEnumerable<Collection> range)
        {
            return _repos.UpdateRangeAsync(range);
        }

        public void UpdateStatistics(int exhibitId)
        {
            using (_context)
            {
                var statExhibit = _context.PopCollections.FirstOrDefault(x => x.Collection.CollectionId == exhibitId);
                var exhibit = _context.Collections.FirstOrDefault(x => x.CollectionId == exhibitId);
                if (statExhibit != default || statExhibit != null)
                {
                    statExhibit.Rate++;
                    _context.PopCollections.Update(statExhibit);
                    _context.SaveChanges();
                }
                else if (exhibit != default || exhibit != null)
                {
                    var newExhibit = new PopCollection() { Collection = exhibit, Rate = 1 };
                    _context.PopCollections.Add(newExhibit);
                    _context.SaveChanges();
                }
            }
        }
    }
}
