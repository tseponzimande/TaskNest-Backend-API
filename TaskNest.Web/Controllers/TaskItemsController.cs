namespace TaskNest.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class TaskItemsController(ITaskItemService taskService, IMapper mapper) : ControllerBase
    {
        private readonly ITaskItemService _taskService = taskService;
        private readonly IMapper _mapper = mapper;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        /// <summary>
        /// GetTaskItems
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status500InternalServerError)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTaskItems()
        {
            var tasks = await _taskService.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }

        /// <summary>
        /// GetTaskItem
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status500InternalServerError)]
        [ProducesResponseType(Status400BadRequest)]
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetTaskItem(Guid id)
        {
            var task = await _taskService.GetByIdAsync(id);
            if (task == null) return NotFound();
            return Ok(_mapper.Map<TaskItemDto>(task));
        }

        /// <summary>
        /// GetTasksByColumn
        /// </summary>
        /// <param name="columnId"></param>
        /// <returns></returns>
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status500InternalServerError)]
        [HttpGet("byColumn/{columnId}")]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetTasksByColumn(Guid columnId)
        {
            var tasks = await _taskService.GetByColumnAsync(columnId);
            return Ok(_mapper.Map<IEnumerable<TaskItemDto>>(tasks));
        }

        /// <summary>
        /// CreateTaskItem
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status500InternalServerError)]
        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTaskItem(TaskItemDto dto)
        {
            var task = _mapper.Map<TaskItem>(dto);
            var createdTask = await _taskService.CreateAsync(task, UserId);

            var createdDto = _mapper.Map<TaskItemDto>(createdTask);
            return CreatedAtAction(nameof(GetTaskItem), new { id = createdDto.Id }, createdDto);
        }

        /// <summary>
        /// UpdateTaskItem
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status500InternalServerError)]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTaskItem(Guid id, TaskItemDto dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var task = _mapper.Map<TaskItem>(dto);
            var updated = await _taskService.UpdateAsync(task, UserId);

            return updated ? NoContent() : NotFound();
        }

        /// <summary>
        /// DeleteTaskItem
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status500InternalServerError)]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskItem(Guid id)
        {
            var deleted = await _taskService.DeleteAsync(id, UserId);
            return deleted ? NoContent() : NotFound();
        }

        /// <summary>
        /// MoveTask
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moveDto"></param>
        /// <returns></returns>
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status404NotFound)]
        [ProducesResponseType(Status400BadRequest)]
        [ProducesResponseType(Status500InternalServerError)]
        [HttpPut("{id}/move")]
        public async Task<IActionResult> MoveTask(Guid id, [FromBody] MoveTaskDto moveDto)
        {
            var moved = await _taskService.MoveTaskAsync(id, moveDto.ColumnId, moveDto.Position, UserId);
            return moved ? NoContent() : NotFound();
        }
    }
}
