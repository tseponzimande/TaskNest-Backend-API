namespace TaskNest.API.Services
{
    public class UserManagementService(UserManager<ApplicationUser> userManager, AppDbContext appDbContext, ILogger<UserManagementService> logger) : IUserManagementService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly AppDbContext _appDbContext = appDbContext;
        private readonly ILogger<UserManagementService> _logger = logger;

        public async Task<IEnumerable<UserManagementDto>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.Include(u => u.BoardUsers).OrderByDescending(u => u.CreatedAt).ToListAsync();

                var userDtos = new List<UserManagementDto>();

                foreach (var user in users)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    userDtos.Add(new UserManagementDto
                    {
                        Id = user.Id,
                        Email = user.Email ?? string.Empty,
                        UserName = user.UserName,
                        IsEnabled = user.IsEnabled,
                        EmailConfirmed = user.EmailConfirmed,
                        CreatedAt = user.CreatedAt,
                        LastLoginAt = user.LastLoginAt,
                        Roles = roles.ToList(),
                        BoardCount = user.BoardUsers?.Count ?? 0
                    });
                }

                return userDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all users");
                return Enumerable.Empty<UserManagementDto>();
            }
        }

        public async Task<UserManagementDto?> GetUserByIdAsync(string userId)
        {
            try
            {
                var user = await _userManager.Users.Include(u => u.BoardUsers).FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null)
                    return null;

                var roles = await _userManager.GetRolesAsync(user);

                return new UserManagementDto
                {
                    Id = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName,
                    IsEnabled = user.IsEnabled,
                    EmailConfirmed = user.EmailConfirmed,
                    CreatedAt = user.CreatedAt,
                    LastLoginAt = user.LastLoginAt,
                    Roles = roles.ToList(),
                    BoardCount = user.BoardUsers?.Count ?? 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user {userId}");
                return null;
            }
        }

        public async Task<bool> ToggleUserStatusAsync(string userId, bool isEnabled)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return false;

                user.IsEnabled = isEnabled;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {user.Email} status changed to {(isEnabled ? "enabled" : "disabled")}");
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error toggling user status for {userId}");
                return false;
            }
        }

        public async Task<bool> UpdateUserRolesAsync(string userId, List<string> roles)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return false;
                }

                var currentRoles = await _userManager.GetRolesAsync(user);

                var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

                if (!removeResult.Succeeded)
                {
                    return false;
                }

                var addResult = await _userManager.AddToRolesAsync(user, roles);

                if (addResult.Succeeded)
                {
                    _logger.LogInformation($"User {user.Email} roles updated to: {string.Join(", ", roles)}");
                }

                return addResult.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating user roles for {userId}");
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return false;
                }

                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {user.Email} deleted");
                }

                return result.Succeeded;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting user {userId}");
                return false;
            }
        }
    }
}