using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Abstraction
{
    public interface IUserService
    {
        Task<string> AddUser(UserVM userVM);
        Task<string> LoginUser(LoginVM loginVM);
        Task<List<string>> GetAllRoles();
    }
}
