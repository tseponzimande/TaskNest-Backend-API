namespace TaskNest.API.Services
{
    public class AccountService(UserManager<ApplicationUser> userManager, IConfiguration configuration, ILogger<AccountService> logger) : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<AccountService> _logger = logger;

        public async Task<IdentityResult> RegisterAsync(RegisterDto model)
        {
            try
            {
                _logger.LogInformation("Registering user: {Email}", model.Email);

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    _logger.LogInformation("User registered successfully: {Email}", model.Email);
                }
                else
                {
                    _logger.LogWarning("User registration failed: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for {Email}", model.Email);
                var error = new IdentityError
                {
                    Code = "Exception",
                    Description = ex.Message
                };
                return IdentityResult.Failed(error);
            }
        }

        public async Task<string?> LoginAsync(LoginDto model)
        {
            try
            {
                _logger.LogInformation("Login attempt for: {Email}", model.Email);

                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    _logger.LogWarning("Login failed: Invalid credentials for {Email}", model.Email);
                    return null;
                }

                if (!user.IsEnabled)
                {
                    _logger.LogWarning("Login failed: User {Email} is disabled", model.Email);
                    return null;
                }

                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.UtcNow.AddDays(int.Parse(_configuration["JWT:ExpirationInDays"]!)),
                    claims: claims,
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                _logger.LogInformation("Login successful for {Email}, Token generated", model.Email);
                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Email}", model.Email);
                return null;
            }
        }


        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null) return false;

                var decoded = token;
                // If token was URL-encoded server-side, ensure decoding here.
                var result = await _userManager.ConfirmEmailAsync(user, decoded);
                return result.Succeeded;
            }
            catch (Exception ex)
            {
                throw new Exception($": {ex.Message}");
            }
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return IdentityResult.Failed(new IdentityError { Code = "UserNotFound", Description = "User not found." });
                }

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception($":{ex.Message}");
            }
        }
    }
}