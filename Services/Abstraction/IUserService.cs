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
        Task<TokenVM> AddUser(UserVM userVM);
        Task<TokenVM> LoginUser(LoginVM loginVM);
        Task<List<string>> GetAllRoles();
        Task<TokenVM> GetToken(string refreshToken);
    }
}
