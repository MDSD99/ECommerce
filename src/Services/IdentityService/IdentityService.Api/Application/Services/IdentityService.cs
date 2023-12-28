using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IdentityService.Api.Application.Models;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Api.Application.Services
{
    public class IdentityService : IIdentityService
    {
        public Task<LoginResponseModel> Login(LoginRequestModel requestModel)
        {
            var claims = new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier,requestModel.UserName),
                new Claim(ClaimTypes.Name,"Mehmet Doğan")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("MehmetSoftwareDeMehmetSoftwareDe"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.Now.AddDays(1);

            var token = new JwtSecurityToken(claims:claims,expires: expiry,signingCredentials: creds,notBefore: DateTime.Now);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(token);
             
            LoginResponseModel response = new()
            {
                UserName=requestModel.UserName,
                UserToken=encodedJwt
            };
             
            return Task.FromResult(response);
        }
    }
}

