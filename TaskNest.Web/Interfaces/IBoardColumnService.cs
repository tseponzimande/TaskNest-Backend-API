namespace TaskNest.API.Interfaces
{
    public interface IBoardColumnService
    {
        Task<IEnumerable<BoardColumn>> GetColumnsAsync(Guid boardId);
        Task<BoardColumn?> GetByIdAsync(Guid id);
        Task<BoardColumn> CreateAsync(BoardColumn column);
        Task<bool> UpdateAsync(BoardColumn column);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ReorderColumnsAsync(List<ColumnOrderDto> columnOrders);
    }
}