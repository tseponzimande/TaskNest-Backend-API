namespace TaskNest.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/tasks/{taskId}/attachments")]
    public class TaskAttachmentsController(
        ITaskAttachmentService attachmentService,
        ITaskItemService taskService,
        IBoardUserService boardUserService,
        IMapper mapper,
        ILogger<TaskAttachmentsController> logger) : ControllerBase
    {
        private readonly ITaskAttachmentService _attachmentService = attachmentService;
        private readonly ITaskItemService _taskService = taskService;
        private readonly IBoardUserService _boardUserService = boardUserService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<TaskAttachmentsController> _logger = logger;

        /// <summary>
        /// Get all attachments for a task
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskAttachmentDto>>> GetAttachments(Guid taskId)
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

                var attachments = await _attachmentService.GetAttachmentsByTaskIdAsync(taskId);
                var attachmentDtos = attachments.Select(a => new TaskAttachmentDto
                {
                    Id = a.Id,
                    FileName = a.FileName,
                    OriginalFileName = a.OriginalFileName,
                    ContentType = a.ContentType,
                    FileSize = a.FileSize,
                    UploadedAt = a.UploadedAt,
                    TaskItemId = a.TaskItemId,
                    ApplicationUserId = a.ApplicationUserId,
                    UserEmail = a.User?.Email
                });

                return Ok(attachmentDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting attachments for task {taskId}");
                return StatusCode(500, new { message = "Error retrieving attachments" });
            }
        }

        /// <summary>
        /// Upload an attachment to a task
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TaskAttachmentDto>> UploadAttachment(Guid taskId, [FromForm] IFormFile file)
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

                var attachment = await _attachmentService.UploadAttachmentAsync(taskId, file, userId!);

                var attachmentDto = new TaskAttachmentDto
                {
                    Id = attachment.Id,
                    FileName = attachment.FileName,
                    OriginalFileName = attachment.OriginalFileName,
                    ContentType = attachment.ContentType,
                    FileSize = attachment.FileSize,
                    UploadedAt = attachment.UploadedAt,
                    TaskItemId = attachment.TaskItemId,
                    ApplicationUserId = attachment.ApplicationUserId,
                    UserEmail = attachment.User?.Email
                };

                return CreatedAtAction(nameof(GetAttachments), new { taskId }, attachmentDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading attachment for task {taskId}");
                return StatusCode(500, new { message = "Error uploading attachment" });
            }
        }

        /// <summary>
        /// Download an attachment
        /// </summary>
        [HttpGet("{attachmentId}/download")]
        public async Task<IActionResult> DownloadAttachment(Guid taskId, Guid attachmentId)
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

                var result = await _attachmentService.DownloadAttachmentAsync(attachmentId);

                if (result == null)
                {
                    return NotFound("Attachment not found");
                }

                return File(result.Value.fileStream, result.Value.contentType, result.Value.fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading attachment {attachmentId}");
                return StatusCode(500, new { message = "Error downloading attachment" });
            }
        }

        /// <summary>
        /// Delete an attachment
        /// </summary>
        [HttpDelete("{attachmentId}")]
        public async Task<IActionResult> DeleteAttachment(Guid taskId, Guid attachmentId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var deleted = await _attachmentService.DeleteAttachmentAsync(attachmentId, userId!);

                return deleted ? NoContent() : NotFound("Attachment not found");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting attachment {attachmentId}");
                return StatusCode(500, new { message = "Error deleting attachment" });
            }
        }

        /// <summary>
        /// Preview an image attachment
        /// </summary>
        [HttpGet("{attachmentId}/preview")]
        public async Task<IActionResult> PreviewAttachment(Guid taskId, Guid attachmentId)
        {
            try
            {
                var attachment = await _attachmentService.GetAttachmentByIdAsync(attachmentId);

                if (attachment == null || !attachment.ContentType.StartsWith("image/"))
                {
                    return NotFound("Attachment not found or not an image");
                }

                var result = await _attachmentService.DownloadAttachmentAsync(attachmentId);

                if (result == null)
                {
                    return NotFound("Attachment not found");
                }

                return File(result.Value.fileStream, result.Value.contentType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error previewing attachment {attachmentId}");
                return StatusCode(500, new { message = "Error previewing attachment" });
            }
        }
    }
}