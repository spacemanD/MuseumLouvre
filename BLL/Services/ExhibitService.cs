using BLL.Interfaces;
using DAL;
using DAL.EF.Entities;
using DAL.EF.Entities.Enums;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
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
        public IEnumerable<Exhibit> GetAllListAsync()
        {
            var list = _repos.GetRange<Exhibit>(false, x => x != null,
                include: source => source
                .Include(a => a.Collection)
                .Include(a => a.Author)).ToList();

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

        public IEnumerable<Exhibit> GetAllListAsyncNonAuthors()
        {
            var list = _repos.GetRange<Exhibit>(false, x => x != null,
                include: source => source
                .Include(a => a.Collection)).ToList();

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

        public Task<Exhibit> AddAsync(Exhibit exemplar)
        {
            return _repos.AddAsync(exemplar);
        }

        public Task AddRangeAsync(IEnumerable<Exhibit> range)
        {
            return _repos.AddRangeAsync(range);
        }

        public Task DeleteRangeAsync(IEnumerable<Exhibit> range)
        {
            return _repos.DeleteRangeAsync(range);
        }

        public Task DeleteAsync(Exhibit exemplar)
        {
            return _repos.DeleteAsync(exemplar);
        }

        public Task UpdateAsync(Exhibit exemplar)
        {
            return _repos.UpdateAsync(exemplar);
        }

        public Task UpdateRangeAsync(IEnumerable<Exhibit> range)
        {
            return _repos.UpdateRangeAsync(range);
        }

        public ICollection<PopExhibit> GetTop10PopularExhibits()
        {
            var exhibits = _repos.GetRangeAsync<PopExhibit>(false, x => x != null,
            include: source => source
            .Include(a => a.Exhibit)
            .ThenInclude(z => z.Author)).Result.OrderByDescending(x => x.Rate).Take(10).ToList();
            return exhibits;
        }

        public ICollection<PopExhibit> GetLast10PopularExhibits()
        {
            var exhibits = _repos.GetRangeAsync<PopExhibit>(false, x => x != null,
            include: source => source
            .Include(a => a.Exhibit)
            .ThenInclude(z => z.Author)).Result.OrderBy(x => x.Rate).Take(10).ToList();
            return exhibits;

        }

        public void UpdateStatistics(int exhibitId)
        {
            using (_context)
            {
                var statExhibit = _context.PopExhibits.FirstOrDefault(x => x.Exhibit.ExhibitId == exhibitId);
                var exhibit = _context.Exhibits.FirstOrDefault(x => x.ExhibitId == exhibitId);
                if (statExhibit != default || statExhibit != null)
                {
                    statExhibit.Rate++;
                    _context.PopExhibits.Update(statExhibit);
                    _context.SaveChanges();
                }
                else if (exhibit != default || exhibit != null)
                {
                    var newExhibit = new PopExhibit() { Exhibit = exhibit, Rate = 1 };
                    _context.PopExhibits.Add(newExhibit);
                    _context.SaveChanges();
                }
            }
        }

        public Task ProccessFile()
        {
            var path =@"D:\test.txt";
            string[] lines = File.ReadAllLines(path);
            var col = new string [10];
            var exhibits = new List<Exhibit>();
            foreach (string line in lines)
            {
                col = line.Split('|');
                var exhibit = new Exhibit()
                {
                    Name = col[0],
                    AuthorId = Convert.ToInt32(col[1]),
                    CreationYear = Convert.ToInt32(col[3]),
                    Description = col[4],
                    Type = (ExhibitType)Enum.Parse(typeof(ExhibitType), col[5]),
                    Cost = Convert.ToInt32(col[6]),
                    Direction = (ArtDirection)Enum.Parse(typeof(ArtDirection), col[7]),
                    Materials = col[8],
                    Country = (CountryList)Enum.Parse(typeof(CountryList), col[9])
                };
                exhibits.Add(exhibit);
            }
            
            return AddRangeAsync(exhibits);
        }
    }
}
