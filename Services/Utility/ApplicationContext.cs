using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Utility
{
    public class ApplicationContext:IApplicationContext
    {
        public int UserId { get; }
        public string Token { get; }
        public ApplicationContext(IHttpContextAccessor httpContext)
        {
            var handler = new JwtSecurityTokenHandler();
            var authtoken = httpContext.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!String.IsNullOrEmpty(authtoken))
            {
                Token = authtoken;
                var token = handler.ReadJwtToken(authtoken) as JwtSecurityToken;
                // var Id = token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId).Value;
                UserId = Convert.ToInt32(token.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.NameId).Value);
            }
        }
    }
}
