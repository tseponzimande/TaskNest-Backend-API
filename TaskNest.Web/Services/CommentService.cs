namespace TaskNest.API.Services
{
    public class CommentService(AppDbContext context) : ICommentService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<Comment>> GetCommentsByTaskIdAsync(Guid taskId)
        {
            try
            {
                return await _context.Comments
                    .Include(c => c.User)
                    .Where(c => c.TaskItemId == taskId)
                    .OrderBy(c => c.CreatedAt)
                    .ToListAsync();
            }
            catch
            {
                return Enumerable.Empty<Comment>();
            }
        }

        public async Task<Comment?> GetCommentByIdAsync(Guid id)
        {
            try
            {
                return await _context.Comments
                    .Include(c => c.User)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch
            {
                return null;
            }
        }

        public async Task<Comment> CreateCommentAsync(Comment comment)
        {
            try
            {
                comment.Id = Guid.NewGuid();
                comment.CreatedAt = DateTime.UtcNow;

                await _context.Comments.AddAsync(comment);
                await _context.SaveChangesAsync();

               
                return await GetCommentByIdAsync(comment.Id) ?? comment;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating comment", ex);
            }
        }


        public async Task<bool> UpdateCommentAsync(Comment comment)
        {
            try
            {
                comment.UpdatedAt = DateTime.UtcNow;
                _context.Comments.Update(comment);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteCommentAsync(Guid id)
        {
            try
            {
                var comment = await _context.Comments.FindAsync(id);
                if (comment == null) return false;

                _context.Comments.Remove(comment);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch
            {
                return false;
            }
        }
    }
}