namespace TaskNest.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BoardColumnsController(
        IBoardColumnService columnService,
        IMapper mapper,
        INotificationService notificationService) : ControllerBase
    {
        private readonly IBoardColumnService _columnService = columnService;
        private readonly IMapper _mapper = mapper;
        private readonly INotificationService _notificationService = notificationService;

        [HttpGet("byBoard/{boardId}")]
        public async Task<ActionResult<IEnumerable<BoardColumnDto>>> GetColumns(Guid boardId)
        {
            var columns = await _columnService.GetColumnsAsync(boardId);
            var columnsDto = _mapper.Map<IEnumerable<BoardColumnDto>>(columns);
            return Ok(columnsDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BoardColumnDto>> GetColumn(Guid id)
        {
            var column = await _columnService.GetByIdAsync(id);

            if (column == null)
            {
                return NotFound();
            }

            var columnDto = _mapper.Map<BoardColumnDto>(column);
            return Ok(columnDto);
        }

        [HttpPost]
        public async Task<ActionResult<BoardColumnDto>> Create(BoardColumnDto columnDto)
        {
            var column = _mapper.Map<BoardColumn>(columnDto);
            column.Id = Guid.NewGuid();

            var createdColumn = await _columnService.CreateAsync(column);
            var createdDto = _mapper.Map<BoardColumnDto>(createdColumn);

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown";
            await _notificationService.NotifyColumnCreatedAsync(columnDto.BoardId, columnDto.Name, userEmail);

            return CreatedAtAction(nameof(GetColumn), new { id = createdDto.Id }, createdDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, BoardColumnDto columnDto)
        {
            if (id != columnDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            var column = _mapper.Map<BoardColumn>(columnDto);
            var updated = await _columnService.UpdateAsync(column);

            if (!updated) return NotFound();

            var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown";
            await _notificationService.NotifyColumnUpdatedAsync(columnDto.BoardId, columnDto.Name, userEmail);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var column = await _columnService.GetByIdAsync(id);

            if (column == null)
            {
                return NotFound();
            }

            var deleted = await _columnService.DeleteAsync(id);

            if (deleted)
            {
                var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown";
                await _notificationService.NotifyColumnDeletedAsync(column.BoardId, column.Name, userEmail);
            }

            return deleted ? NoContent() : NotFound();
        }

        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderColumns([FromBody] List<ColumnOrderDto> columnOrders)
        {
            try
            {
                if (!columnOrders.Any())
                {
                    return BadRequest("No columns to reorder");
                }

                var boardId = columnOrders.First().BoardId;
                var success = await _columnService.ReorderColumnsAsync(columnOrders);

                if (success)
                {
                    var userEmail = User.FindFirstValue(ClaimTypes.Email) ?? "Unknown";
                    await _notificationService.NotifyColumnUpdatedAsync(boardId, "Columns", userEmail);
                }

                return success ? NoContent() : BadRequest("Failed to reorder columns");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reordering columns", detail = ex.Message });
            }
        }
    }
}

