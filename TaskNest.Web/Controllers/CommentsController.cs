namespace TaskNest.API.Controllers
{
    //[AllowAnonymous]
    [Authorize]
    [ApiController]
    [Route("api/tasks/{taskId}/comments")]
    public class CommentsController(ICommentService commentService, ITaskItemService taskService, IBoardUserService boardUserService, IMapper mapper) : ControllerBase
    {
        private readonly ICommentService _commentService = commentService;
        private readonly ITaskItemService _taskService = taskService;
        private readonly IBoardUserService _boardUserService = boardUserService;
        private readonly IMapper _mapper = mapper;

        /// <summary>
        /// Get all comments for a task
        /// </summary>
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(Status403Forbidden)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CommentDto>>> GetComments(Guid taskId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var task = await _taskService.GetByIdAsync(taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                var isUserInBoard = await _boardUserService.IsUserInBoardAsync(task.BoardId, userId!);

                if (!isUserInBoard)
                {
                    return Forbid("You don't have access to this task");
                }

                var comments = await _commentService.GetCommentsByTaskIdAsync(taskId);
                var commentDtos = comments.Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt,
                    TaskItemId = c.TaskItemId,
                    ApplicationUserId = c.ApplicationUserId,
                    UserEmail = c.User?.Email
                });

                return Ok(commentDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving comments", detail = ex.Message });
            }
        }

        /// <summary>
        /// Create a new comment
        /// </summary>
        [ProducesResponseType(Status201Created)]
        [ProducesResponseType(Status400BadRequest)]
        [HttpPost]
        public async Task<ActionResult<CommentDto>> CreateComment(Guid taskId, [FromBody] CreateCommentDto dto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var task = await _taskService.GetByIdAsync(taskId);

                if (task == null)
                {
                    return NotFound("Task not found");
                }

                var isUserInBoard = await _boardUserService.IsUserInBoardAsync(task.BoardId, userId!);

                if (!isUserInBoard)
                {
                    return Forbid("You don't have access to this task");
                }

                var comment = new Comment
                {
                    Content = dto.Content,
                    TaskItemId = taskId,
                    ApplicationUserId = userId!
                };

                var created = await _commentService.CreateCommentAsync(comment);

                var commentDto = new CommentDto
                {
                    Id = created.Id,
                    Content = created.Content,
                    CreatedAt = created.CreatedAt,
                    UpdatedAt = created.UpdatedAt,
                    TaskItemId = created.TaskItemId,
                    ApplicationUserId = created.ApplicationUserId,
                    UserEmail = created.User?.Email
                };

                return CreatedAtAction(nameof(GetComments), new { taskId }, commentDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error creating comment", detail = ex.Message });
            }
        }

        /// <summary>
        /// Delete a comment
        /// </summary>
        [ProducesResponseType(Status204NoContent)]
        [ProducesResponseType(Status403Forbidden)]
        [ProducesResponseType(Status404NotFound)]
        [HttpDelete("{commentId}")]
        public async Task<IActionResult> DeleteComment(Guid taskId, Guid commentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var comment = await _commentService.GetCommentByIdAsync(commentId);

                if (comment == null)
                {
                    return NotFound("Comment not found");
                }

                if (comment.ApplicationUserId != userId)
                {
                    return Forbid("You can only delete your own comments");
                }

                var deleted = await _commentService.DeleteCommentAsync(commentId);
                return deleted ? NoContent() : BadRequest("Failed to delete comment");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting comment", detail = ex.Message });
            }
        }
    }
}
