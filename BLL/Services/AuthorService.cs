using BLL.Interfaces;
using DAL;
using DAL.EF.Entities;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IRepository _repos;

        private readonly ILogger _logger;

        private readonly AppDbContext _context;

        public AuthorService(ILogger<AuthorService> logger, IRepository repository, AppDbContext context)
        {
            _logger = logger;
            _repos = repository;
            _context = context;
        }
        public IEnumerable<Author> GetAllListAsync()
        {
            var list = _repos.GetRangeAsync<Author>(false, x => x != null,
            include: source => source
            .Include(a => a.Exhibits)).Result.ToList();

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

        public Task<Author> AddAsync(Author exemplar)
        {
            return  Task.Run(() => _repos.AddAsync(exemplar));
        }

        public Task AddRangeAsync(IEnumerable<Author> range)
        {
            return  _repos.AddRangeAsync(range);
        }

        public Task DeleteRangeAsync(IEnumerable<Author> range)
        {
            return _repos.DeleteRangeAsync(range);
        }

        public Task DeleteAsync(Author exemplar)
        {
            return _repos.DeleteAsync(exemplar);
        }

        public Task UpdateAsync(Author exemplar)
        {
            return _repos.UpdateAsync(exemplar);
        }

        public Task UpdateRangeAsync(IEnumerable<Author> range)
        {
            return _repos.UpdateRangeAsync(range);
        }

        public ICollection<PopAuthor> GetTop10PopularAuthors()
        {
            var users = _repos.GetRangeAsync<PopAuthor>(false, x => x != null,
            include: source => source
            .Include(a => a.Author)
            .ThenInclude(z => z.Exhibits)).Result.OrderBy(x => x.Rate).Take(10).ToList();
            return users;
        }

        public ICollection<PopAuthor> GetLast10PopularAuthors()
        {

            var users = _repos.GetRangeAsync<PopAuthor>(false, x => x != null,
            include: source => source
            .Include(a => a.Author)
            .ThenInclude(z => z.Exhibits)).Result.OrderByDescending(x => x.Rate).Take(10).ToList();
                return users;
            
        }

        public void UpdateStatistics(int authorId)
        {
            using (_context)
            {
                var statAuth = _context.PopAuthors.FirstOrDefault(x => x.Author.AuthorId == authorId);
                var author = _context.Authors.FirstOrDefault(x => x.AuthorId == authorId);
                if (statAuth != default || statAuth != null)
                {
                    statAuth.Rate++;
                    _context.PopAuthors.Update(statAuth);
                    _context.SaveChanges();
                }
                else if (author != default || author != null)
                {
                    var newUser = new PopAuthor() { Author = author, Rate = 1 };
                    _context.PopAuthors.Add(newUser);
                    _context.SaveChanges();
                }
            }
        }
    }
}
