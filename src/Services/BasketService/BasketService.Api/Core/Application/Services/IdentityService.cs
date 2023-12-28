using System;
using System.Security.Claims;

#nullable disable

namespace BasketService.Api.Core.Application.Services
{
	public class IdentityService:IIdentityService
	{
        private readonly IHttpContextAccessor httpContextAccessor;

        public IdentityService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string GetUserName()
        {
            return httpContextAccessor.HttpContext.User.FindFirst(s => s.Type == ClaimTypes.NameIdentifier).Value;
        }
    }
}

