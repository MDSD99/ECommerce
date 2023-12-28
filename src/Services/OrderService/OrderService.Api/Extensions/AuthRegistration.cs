using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

#nullable disable

namespace OrderService.Api.Extensions
{
	public static class AuthRegistration
	{
		public static IServiceCollection ConfigureAuth(this IServiceCollection services,IConfiguration configuration)
		{
			//var authConfig = configuration.GetSection("AuthConfig");
			var signingkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AuthConfig:Secret"]));
			services.AddAuthentication((x) =>
			{
				x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(s =>
			{
				s.RequireHttpsMetadata = false;
				s.SaveToken = true;
				s.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey=true,
					IssuerSigningKey=signingkey,
					ValidateIssuer=false,
					ValidateAudience=false,
					ClockSkew=TimeSpan.Zero,
					RequireExpirationTime=true
				};
			});
			return services;
		}
	}
}

