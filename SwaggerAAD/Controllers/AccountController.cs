using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace SwaggerAAD.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;
        private IHttpContextAccessor _httpContextAccessor;

        public AccountController(ILogger<AccountController> logger, 
            IHttpContextAccessor httpContextAccessor)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Get user info with token
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserInfoWithToken()
        {
            var givenName = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.GivenName);
            var surname = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Surname);
            var scope = _httpContextAccessor.HttpContext.User.FindFirstValue("http://schemas.microsoft.com/identity/claims/scope");
            var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            var user = new
            {
                Email = email,
                FullName = $"{givenName} {surname}",
                Scope = scope
            };

            _logger.LogInformation("User Info", user);

            return Ok(user);
        }
    }
}
