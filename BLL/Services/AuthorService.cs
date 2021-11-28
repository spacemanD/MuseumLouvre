using BLL.Interfaces;
using DAL;
using DAL.EF.Entities;
using DAL.Interfaces;
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
        public async Task<IEnumerable<Author>> GetAllListAsync()
        {
            var list = await _repos.GetRangeAsync<Author>(false, x => x != null);
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

        public async Task<Author> AddAsync(Author exemplar)
        {
            return await Task.Run(() => _repos.AddAsync(exemplar));
        }

        public async Task AddRangeAsync(IEnumerable<Author> range)
        {
            await Task.Run(() => _repos.AddRangeAsync(range));
        }

        public void DeleteRangeAsync(IEnumerable<Author> range)
        {
            _repos.DeleteRangeAsync(range);
        }

        public void DeleteAsync(Author exemplar)
        {
            _repos.DeleteAsync(exemplar);
        }

        public void UpdateAsync(Author exemplar)
        {
            _repos.UpdateAsync(exemplar);
        }

        public void UpdateRangeAsync(IEnumerable<Author> range)
        {
            _repos.UpdateRangeAsync(range);
        }

        public ICollection<PopAuthor> GetTop10PopularAuthors()
        {
            using (_context)
            {
                var users = _context.PopAuthors.OrderBy(x => x.Rate).Take(10).ToList();
                return users;
            }
        }

        public ICollection<PopAuthor> GetLast10PopularAuthors()
        {
            using (_context)
            {
                var users = _context.PopAuthors.OrderByDescending(x => x.Rate).Take(10).ToList();
                return users;
            }
        }

        public void UpdateStatistics(int authorId)
        {
            using (_context)
            {
                var statAuth = _context.PopAuthors.FirstOrDefault(x => x.AuthorId == authorId);
                var author = _context.Authors.FirstOrDefault(x => x.AuthorId == authorId);
                if (statAuth != default || statAuth != null)
                {
                    statAuth.Rate++;
                    _context.PopAuthors.Update(statAuth);
                    _context.SaveChanges();
                }
                else if (author != default || author != null)
                {
                    var newUser = new PopAuthor() { Author = author, AuthorId = author.AuthorId, Rate = 1 };
                    _context.PopAuthors.Add(newUser);
                    _context.SaveChanges();
                }
            }
        }
    }
}
