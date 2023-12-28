using IdentityService.Api.Application.Models;
using IdentityService.Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthController:ControllerBase
	{
		private readonly IIdentityService identityService;

        public AuthController(IIdentityService identityService)
        {
            this.identityService = identityService;
        }

        [HttpPost] 
        public async Task<IActionResult> Login([FromBody] LoginRequestModel loginModel)
        {
            var result = await identityService.Login(loginModel);
            return Ok(result);
        }
    }
}

