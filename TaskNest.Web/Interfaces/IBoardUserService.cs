namespace TaskNest.API.Interfaces
{
    public interface IBoardUserService
    {
        Task<IEnumerable<BoardUser>> GetBoardUsersAsync(Guid boardId);
        Task<BoardUser?> GetBoardUserAsync(Guid boardId, string userId);
        Task<BoardUser> AddUserToBoardAsync(Guid boardId, string userEmail, BoardUserRole role);
        Task<bool> UpdateBoardUserRoleAsync(Guid boardId, string userId, BoardUserRole role);
        Task<bool> RemoveUserFromBoardAsync(Guid boardId, string userId);
        Task<bool> IsUserInBoardAsync(Guid boardId, string userId);
        Task<BoardUserRole?> GetUserRoleInBoardAsync(Guid boardId, string userId);
    }
}
