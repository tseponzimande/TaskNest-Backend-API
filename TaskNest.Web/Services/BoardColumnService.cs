namespace TaskNest.API.Services
{
    public class BoardColumnService(IGenericRepository<BoardColumn> repository) : IBoardColumnService
    {
        private readonly IGenericRepository<BoardColumn> _repository = repository;

        public async Task<IEnumerable<BoardColumn>> GetColumnsAsync(Guid boardId)
        {
            if (_repository is GenericRepository<BoardColumn> repo)
            {
                return await repo.Query()
                    .Where(c => c.BoardId == boardId)
                    .OrderBy(c => c.Order)
                    .ToListAsync();
            }

            return Enumerable.Empty<BoardColumn>();
        }

        public Task<BoardColumn?> GetByIdAsync(Guid id) => _repository.GetByIdAsync(id);

        public Task<BoardColumn> CreateAsync(BoardColumn column) => _repository.CreateAsync(column);

        public Task<bool> UpdateAsync(BoardColumn column) => _repository.UpdateAsync(column);

        public Task<bool> DeleteAsync(Guid id) => _repository.DeleteAsync(id);

        public async Task<bool> ReorderColumnsAsync(List<ColumnOrderDto> columnOrders)
        {
            try
            {
                if (_repository is GenericRepository<BoardColumn> repo)
                {
                    foreach (var order in columnOrders)
                    {
                        var column = await repo.Context.Set<BoardColumn>().FindAsync(order.Id);
                        if (column != null)
                        {
                            column.Order = order.Order;
                        }
                    }

                    await repo.Context.SaveChangesAsync();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}