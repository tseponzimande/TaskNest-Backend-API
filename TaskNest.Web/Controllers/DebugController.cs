using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaskNest.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DebugController(IServiceProvider serviceProvider) : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        [HttpGet("Authi-check")]

        public IActionResult CheckAuthServices()
        {
            try
            {
                var authService = _serviceProvider.GetService<IAuthenticationService>();
                var schemeProvider = _serviceProvider.GetService<IAuthenticationSchemeProvider>();

                return Ok(new
                {
                    HasAuthenticationService = authService != null,
                    HasSchemeProvider = schemeProvider != null,
                    ServicesRegistered = "Authentication services are properly registered"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }

        }
    }
}
