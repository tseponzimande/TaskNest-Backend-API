namespace TaskNest.API.Interfaces
{
    public interface IBoardService
    {
        Task<IEnumerable<Board>> GetAllAsync();
        Task<Board?> GetByIdAsync(Guid id);
        Task<Board> CreateAsync(Board board);
        Task<bool> UpdateAsync(Board board);
        Task<bool> DeleteAsync(Guid id);
    }
}