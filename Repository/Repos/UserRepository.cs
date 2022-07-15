using Microsoft.EntityFrameworkCore;
using Models.DbModels;
using Repository.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repos
{
    public class UserRepository:IUserRepository
    {
        private readonly UserDbContext _dbContext;
        public UserRepository(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User> CreateUser(User user)
        {
            try
            {
                var result = await _dbContext.Users.FirstOrDefaultAsync(x => x.Username == user.Username);
                if (result == null)
                {

                    user.RoleId = (await _dbContext.Roles.FirstOrDefaultAsync(x => x.Name == user.Role.Name)).Id;
                    user.Role = null;
                    _dbContext.Users.Add(user);
                    await _dbContext.SaveChangesAsync();
                    return user;
                }
                throw new Exception("Username already exist")
            }catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<User> ValidateUser(User user)
        {
            try
            {
                var res = await _dbContext.Users.Include("Role")
                       .FirstOrDefaultAsync(u => u.Username == user.Username);
                if (res != null && res.Password == user.Password)
                {
                    return res;
                }
                else if (res != null && res.Password != user.Password)
                {
                    throw new Exception("Password is Incorrect");
                }
                else
                {
                    throw new Exception("User Not found");
                }

            }catch(Exception ex)
            {
                throw;
            }
            

        }

        public async Task<List<Role>> GetRoles()
        {
            try
            {
                return await _dbContext.Roles.ToListAsync();

            }catch(Exception ex)
            {
                throw;
            }
        }
    }
}
