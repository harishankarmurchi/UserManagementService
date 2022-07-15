using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.ViewModels;
using Services.Abstraction;
using System.Net;

namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<Response<string>> Login([FromBody] LoginVM loginVM)
        {
            var response = new Response<string>();
            try
            {
                var result= await _userService.LoginUser(loginVM);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = result;

            }
            catch(Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpPost]
        [Route("register")]
        public async Task<Response<string>> CreateUser([FromBody] UserVM uservm)
        {
            var response = new Response<string>();
            try
            {
                var result = await _userService.AddUser(uservm);
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = result;

            }
            catch (Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = ex.Message;
            }
            return response;
        }

        [HttpGet]
        [Route("getroles")]
        public async Task<Response<List<string>>> GetRoles()
        {
            var response = new Response<List<string>>();
            try
            {
                var result = await _userService.GetAllRoles();
                response.StatusCode = (int)HttpStatusCode.OK;
                response.Data = result;
            }
            catch(Exception ex)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
