namespace TaskNest.API.Controllers
{
    [Authorize]
    //[AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]

    public class AccountController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

        /// <summary>
        /// Register a new user
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        [ProducesResponseType(Status422UnprocessableEntity)]
        [ProducesResponseType(Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            var result = await _accountService.RegisterAsync(model);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("User registered successfully.");
        }


        /// <summary>
        /// Login a user and return a JWT token
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        [ProducesResponseType(Status422UnprocessableEntity)]
        [ProducesResponseType(Status500InternalServerError)]
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto model)
        {
            var token = await _accountService.LoginAsync(model);
            if (token == null)
            {
                return Unauthorized("Invalid credentials.");
            }

            return Ok(new { Token = token });
        }


        /// <summary>
        /// Confirmation
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        [ProducesResponseType(Status422UnprocessableEntity)]
        [ProducesResponseType(Status500InternalServerError)]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Invalid parameters.");
            }

            var success = await _accountService.ConfirmEmailAsync(userId, token);
            if (success)
            {
                return Ok();
            }
            return BadRequest("Email confirmation failed.");
        }


        /// <summary>
        /// Change Password
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status409Conflict)]
        [ProducesResponseType(Status422UnprocessableEntity)]
        [ProducesResponseType(Status500InternalServerError)]
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var result = await _accountService.ChangePasswordAsync(userId, model.CurrentPassword, model.NewPassword);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok();
        }
    }
}