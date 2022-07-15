using Models.DbModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Abstraction
{
    public interface IUserRepository
    {
        Task<User> CreateUser(User user);
        Task<User> ValidateUser(User user);
        Task<List<Role>> GetRoles();
    }
}
