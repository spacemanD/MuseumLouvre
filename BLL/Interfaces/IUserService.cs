using DAL.EF.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IUserService
    {
        public Task Create(User user);

        public Task<IEnumerable<User>> GetAllListAsync();

        public Task AddRangeAsync(IEnumerable<User> range);

        public void DeleteRangeAsync(IEnumerable<User> range);

        public void DeleteAsync(User exemplar);

        public void UpdateAsync(User exemplar);

        public void UpdateRangeAsync(IEnumerable<User> range);

        public Task<User> FindByLogin(string login);
    }
}
