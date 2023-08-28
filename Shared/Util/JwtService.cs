using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Util
{
    public class JwtService
    {
        public string GenerateJWT(ClaimsPrincipal user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysecretkey"));
            var algorithm = SecurityAlgorithms.HmacSha256;

            var credentials = new SigningCredentials(key, algorithm);

            var handler = new JwtSecurityTokenHandler();

            var token = handler.CreateToken(new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(user.Claims), // Create a new ClaimsIdentity based on user.Claims
                SigningCredentials = credentials
            });

            return handler.WriteToken(token);
        }
    }
}
