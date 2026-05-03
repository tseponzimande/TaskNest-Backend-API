namespace TaskNest.API.Services
{
    public class BoardUserService(AppDbContext context, UserManager<ApplicationUser> userManager) : IBoardUserService
    {
        private readonly AppDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public async Task<IEnumerable<BoardUser>> GetBoardUsersAsync(Guid boardId)
        {
            try
            {
                return await _context.BoardUsers.Include(bu => bu.ApplicationUser).Where(bu => bu.BoardId == boardId).ToListAsync();
            }
            catch
            {
                return Enumerable.Empty<BoardUser>();
            }
        }

        public async Task<BoardUser?> GetBoardUserAsync(Guid boardId, string userId)
        {
            try
            {
                return await _context.BoardUsers.FirstOrDefaultAsync(bu => bu.BoardId == boardId && bu.ApplicationUserId == userId);
            }
            catch
            {
                return null;
            }
        }

        public async Task<BoardUser> AddUserToBoardAsync(Guid boardId, string userEmail, BoardUserRole role)
        {
            try
            {
                if (string.IsNullOrEmpty(userEmail))
                {
                    throw new ArgumentNullException(nameof(userEmail), "User email cannot be null or empty.");
                }

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user == null)
                {
                    throw new Exception($"User with email '{userEmail}' not found.");
                }

                var existingBoardUser = await _context.BoardUsers
                    .FirstOrDefaultAsync(bu => bu.BoardId == boardId && bu.ApplicationUserId == user.Id);
                if (existingBoardUser != null)
                {
                    throw new Exception($"User '{userEmail}' is already a member of this board.");
                }

                var boardUser = new BoardUser
                {
                    Id = Guid.NewGuid(),
                    BoardId = boardId,
                    ApplicationUserId = user.Id,
                    Role = role
                };

                await _context.BoardUsers.AddAsync(boardUser);
                await _context.SaveChangesAsync();
                return boardUser;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to add user to board: {ex.Message}");
            }
        }

        public async Task<bool> UpdateBoardUserRoleAsync(Guid boardId, string userId, BoardUserRole role)
        {
            try
            {
                var boardUser = await _context.BoardUsers.FirstOrDefaultAsync(bu => bu.BoardId == boardId && bu.ApplicationUserId == userId);

                if (boardUser == null)
                {
                    return false;
                }

                boardUser.Role = role;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RemoveUserFromBoardAsync(Guid boardId, string userId)
        {
            try
            {
                var boardUser = await _context.BoardUsers
                    .FirstOrDefaultAsync(bu => bu.BoardId == boardId && bu.ApplicationUserId == userId);
                if (boardUser == null)
                {
                    return false;
                }

                _context.BoardUsers.Remove(boardUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> IsUserInBoardAsync(Guid boardId, string userId)
        {
            try
            {
                return await _context.BoardUsers.AnyAsync(bu => bu.BoardId == boardId && bu.ApplicationUserId == userId);
            }
            catch
            {
                return false;
            }
        }

        public async Task<BoardUserRole?> GetUserRoleInBoardAsync(Guid boardId, string userId)
        {
            try
            {
                var boardUser = await _context.BoardUsers.FirstOrDefaultAsync(bu => bu.BoardId == boardId && bu.ApplicationUserId == userId);

                return boardUser?.Role;
            }
            catch
            {
                return null;
            }
        }
    }
}