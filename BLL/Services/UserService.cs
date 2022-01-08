using BLL.Interfaces;
using DAL.EF.Entities;
using DAL.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository _repos;

        private readonly ILogger _logger;

        public UserService(ILogger<UserService> logger, IRepository repository)
        {
            _logger = logger;
            _repos = repository;
        }

        public async Task Create(User user)
        {
            var userunhecked = await FindByLogin(user.Login);
            if (userunhecked == null)
            {
                await Task.Run(() => _repos.AddAsync(user));
                _logger.LogInformation("User successfully created");
            }
            else
            {
                _logger.LogInformation("User is already created");
            }
        }

        public async Task<IEnumerable<User>> GetAllListAsync()
        {
            var list = await Task.Run(() => _repos.GetRangeAsync<User>(false, x => x != null));
            return list;
        }

        public async Task AddRangeAsync(IEnumerable<User> range)
        {
            await Task.Run(() => _repos.AddRangeAsync(range));
        }

        public async void DeleteRangeAsync(IEnumerable<User> range)
        {
            await Task.Run(() => _repos.DeleteRangeAsync(range));
        }

        public async void DeleteAsync(User exemplar)
        {
            await Task.Run(() => _repos.DeleteAsync(exemplar));
        }

        public void UpdateAsync(User exemplar)
        {
            _repos.UpdateAsync(exemplar);
        }

        public async void UpdateRangeAsync(IEnumerable<User> range)
        {
            await Task.Run(() => _repos.UpdateRangeAsync(range));
        }

        public async Task<User> FindByLogin(string login)
        {
            var userunhecked = await Task.Run(() => _repos.GetAsync<User>(false, x => x.Login == login));
            if (userunhecked == null)
            {
                _logger.LogInformation("User is not found");
            }
            else
            {
                _logger.LogInformation("User is found");
            }
            return userunhecked;
        }
    }
}
