namespace TaskNest.API.Interfaces
{
    public interface IUserManagementService
    {
        Task<IEnumerable<UserManagementDto>> GetAllUsersAsync();
        Task<UserManagementDto?> GetUserByIdAsync(string userId);
        Task<bool> ToggleUserStatusAsync(string userId, bool isEnabled);
        Task<bool> UpdateUserRolesAsync(string userId, List<string> roles);
        Task<bool> DeleteUserAsync(string userId);
    }
}