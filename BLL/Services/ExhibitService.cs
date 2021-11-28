using BLL.Interfaces;
using DAL;
using DAL.EF.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class ExhibitService : IExhibitService
    {
        private readonly IRepository _repos;

        private readonly ILogger _logger;

        private readonly AppDbContext _context;
        public ExhibitService(ILogger<ExhibitService> logger, IRepository repository, AppDbContext context)
        {
            _logger = logger;
            _repos = repository;
            _context = context;
        }
        public async Task<IEnumerable<Exhibit>> GetAllListAsync()
        {
            var list = await _repos.GetRangeAsync<Exhibit>(false, x => x != null);
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

        public async Task<Exhibit> AddAsync(Exhibit exemplar)
        {
            return await Task.Run(() => _repos.AddAsync(exemplar));
        }

        public async Task AddRangeAsync(IEnumerable<Exhibit> range)
        {
            await Task.Run(() => _repos.AddRangeAsync(range));
        }

        public void DeleteRangeAsync(IEnumerable<Exhibit> range)
        {
            _repos.DeleteRangeAsync(range);
        }

        public void DeleteAsync(Exhibit exemplar)
        {
            _repos.DeleteAsync(exemplar);
        }

        public void UpdateAsync(Exhibit exemplar)
        {
            _repos.UpdateAsync(exemplar);
        }

        public void UpdateRangeAsync(IEnumerable<Exhibit> range)
        {
            _repos.UpdateRangeAsync(range);
        }

        public ICollection<PopExhibit> GetTop10PopularExhibits()
        {
            using (_context)
            {
                var users = _context.PopExhibits.OrderBy(x => x.Rate).Take(10).ToList();
                return users;
            }
        }

        public ICollection<PopExhibit> GetLast10PopularExhibits()
        {
            using (_context)
            {
                var users = _context.PopExhibits.OrderByDescending(x => x.Rate).Take(10).ToList();
                return users;
            }
        }

        public void UpdateStatistics(int exhibitId)
        {
            using (_context)
            {
                var statExhibit = _context.PopExhibits.FirstOrDefault(x => x.ExhibitId == exhibitId);
                var exhibit = _context.Exhibits.FirstOrDefault(x => x.ExhibitId == exhibitId);
                if (statExhibit != default || statExhibit != null)
                {
                    statExhibit.Rate++;
                    _context.PopExhibits.Update(statExhibit);
                    _context.SaveChanges();
                }
                else if (exhibit != default || exhibit != null)
                {
                    var newExhibit = new PopExhibit() { Exhibit = exhibit, ExhibitId = exhibit.ExhibitId, Rate = 1 };
                    _context.PopExhibits.Add(newExhibit);
                    _context.SaveChanges();
                }
            }
        }
    }
}
