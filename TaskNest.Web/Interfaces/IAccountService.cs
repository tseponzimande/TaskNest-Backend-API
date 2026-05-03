namespace TaskNest.API.Interfaces
{
    public interface IAccountService
    {
        Task<IdentityResult> RegisterAsync(RegisterDto model);
        Task<string?> LoginAsync(LoginDto model);
        Task<bool> ConfirmEmailAsync(string userId, string token);
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
    }
}