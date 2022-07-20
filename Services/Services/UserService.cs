using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Models.DbModels;
using Models.ViewModels;
using Repository.Abstraction;
using Services.Abstraction;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public class UserService:IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepo;
        public UserService(IConfiguration configuration,IUserRepository userRepo)
        {
            _configuration = configuration;
            _userRepo = userRepo;
        }

        public async Task<TokenVM> AddUser(UserVM userVM)
        {
            try
            {
                var user = new User
                {
                    Name = userVM.Name,
                    Password = userVM.Password,
                    Username = userVM.UserName,
                    IsActive = true,
                    Role= new Role
                    {
                        Name=userVM.Role
                    }
                };

                var result = await _userRepo.CreateUser(user);
                return await GenerateJwtToken(result);
                 

            }
             catch(Exception ex)
            {
                throw;
            }
        }
        private async Task<TokenVM> GenerateJwtToken(User user)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:key"]));
            var cred = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {

                new Claim(ClaimTypes.Role,user.Role.Name),
                new Claim(JwtRegisteredClaimNames.NameId,user.Id.ToString())
            };
            var token = new JwtSecurityToken(
                  issuer: _configuration["Jwt:Issuer"],
                  audience: _configuration["Jwt:Audience"],
                  claims,
                  expires: DateTime.Now.AddMinutes(120),
                  signingCredentials: cred
                );
            var refreshToken = Guid.NewGuid();
            await _userRepo.UpdateToken(refreshToken, user.Id);
            var encode = new JwtSecurityTokenHandler().WriteToken(token);
            return new TokenVM
            {
                RefreshToken = refreshToken.ToString(),
                Token = encode

            };
        }

        public async Task<TokenVM> LoginUser(LoginVM loginVM)
        {
            try
            {
                var user = new User
                {
                    Username = loginVM.UserName,
                    Password = loginVM.Password
                };
                var result= await _userRepo.ValidateUser(user);
                return await GenerateJwtToken(result);
            }catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<List<string>> GetAllRoles()
        {
            try
            {
                List<string> roles = new List<string>();
                var result = await _userRepo.GetRoles();
                foreach (var item in result)
                {
                    roles.Add(item.Name);
                }
                return roles;
            }catch(Exception ex)
            {
                throw;
            }
        }

        public async Task<TokenVM> GetToken(string refreshToken)
        {
            try
            {

                var user=await _userRepo.ValidateRefreshToken(Guid.Parse(refreshToken));
                return await GenerateJwtToken(user);

            }catch(Exception ex)
            {
                throw;
            }
        }
    }
}
