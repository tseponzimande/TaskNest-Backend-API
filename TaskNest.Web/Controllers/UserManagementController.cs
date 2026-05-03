namespace TaskNest.API.Controllers
{
    [Authorize(Roles = "Admin")]
    //[AllowAnonymous]
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementController(IUserManagementService userManagementService, ILogger<UserManagementController> logger) : ControllerBase
    {
        private readonly IUserManagementService _userManagementService = userManagementService;
        private readonly ILogger<UserManagementController> _logger = logger;

        /// <summary>
        /// Get all users (Admin only)
        /// </summary>
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status403Forbidden)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserManagementDto>>> GetAllUsers()
        {
            try
            {
                var users = await _userManagementService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return StatusCode(500, new { message = "Error retrieving users" });
            }
        }

        /// <summary>
        /// Get user by ID (Admin only)
        /// </summary>
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [HttpGet("{userId}")]
        public async Task<ActionResult<UserManagementDto>> GetUser(string userId)
        {
            try
            {
                var user = await _userManagementService.GetUserByIdAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user {userId}");
                return StatusCode(500, new { message = "Error retrieving user" });
            }
        }

        /// <summary>
        /// Toggle user enabled/disabled status (Admin only)
        /// </summary>
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [HttpPut("{userId}/toggle-status")]
        public async Task<IActionResult> ToggleUserStatus(string userId, [FromBody] ToggleUserStatusDto dto)
        {
            try
            {
                var success = await _userManagementService.ToggleUserStatusAsync(userId, dto.IsEnabled);

                if (!success)
                    return NotFound("User not found");

                return Ok(new { message = $"User {(dto.IsEnabled ? "enabled" : "disabled")} successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling user status for {userId}");
                return StatusCode(500, new { message = "Error updating user status" });
            }
        }

        /// <summary>
        /// Update user roles (Admin only)
        /// </summary>
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status404NotFound)]
        [HttpPut("{userId}/roles")]
        public async Task<IActionResult> UpdateUserRoles(string userId, [FromBody] UpdateUserRolesDto dto)
        {
            try
            {
                var success = await _userManagementService.UpdateUserRolesAsync(userId, dto.Roles);

                if (!success)
                    return NotFound("User not found");

                return Ok(new { message = "User roles updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user roles for {userId}");
                return StatusCode(500, new { message = "Error updating user roles" });
            }
        }

        /// <summary>
        /// Delete user (Admin only)
        /// </summary>
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            try
            {
                var success = await _userManagementService.DeleteUserAsync(userId);

                if (!success)
                    return NotFound("User not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {userId}");
                return StatusCode(500, new { message = "Error deleting user" });
            }
        }
    }
}