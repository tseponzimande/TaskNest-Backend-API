using TaskNest.API.Interfaces;

namespace TaskNest.API.Services
{
    public class TaskAttachmentService : ITaskAttachmentService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TaskAttachmentService> _logger;
        private readonly string _uploadPath;
        private readonly long _maxFileSize;
        private readonly string[] _allowedExtensions;

        public TaskAttachmentService(
            AppDbContext context,
            IWebHostEnvironment environment,
            IConfiguration configuration,
            ILogger<TaskAttachmentService> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;


            var uploadConfig = configuration.GetSection("FileUpload");


            int maxFileSizeInMB = uploadConfig.GetValue<int?>("MaxFileSizeInMB") ?? 10;
            _maxFileSize = maxFileSizeInMB * 1024 * 1024;

            
            _allowedExtensions = uploadConfig.GetSection("AllowedExtensions").Get<string[]>()
                                ?? new[] { ".pdf", ".doc", ".docx", ".xls", ".xlsx",
                                           ".txt", ".jpg", ".jpeg", ".png", ".gif", ".zip", ".rar" };

            
            var uploadPath = uploadConfig["UploadPath"] ?? "Uploads/TaskAttachments";
            _uploadPath = Path.Combine(_environment.ContentRootPath, uploadPath);


            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }

            _logger.LogInformation($"TaskAttachmentService initialized. MaxFileSize: {maxFileSizeInMB}MB, UploadPath: {_uploadPath}");
        }

        public async Task<IEnumerable<TaskAttachment>> GetAttachmentsByTaskIdAsync(Guid taskId)
        {
            try
            {
                return await _context.TaskAttachments
                    .Include(a => a.User)
                    .Where(a => a.TaskItemId == taskId)
                    .OrderByDescending(a => a.UploadedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting attachments for task {taskId}");
                return Enumerable.Empty<TaskAttachment>();
            }
        }

        public async Task<TaskAttachment?> GetAttachmentByIdAsync(Guid id)
        {
            try
            {
                return await _context.TaskAttachments
                    .Include(a => a.User)
                    .Include(a => a.TaskItem)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting attachment {id}");
                return null;
            }
        }

        public async Task<TaskAttachment> UploadAttachmentAsync(Guid taskId, IFormFile file, string userId)
        {
            try
            {
                if (file == null || file.Length == 0)
                    throw new ArgumentException("File is empty");

                if (file.Length > _maxFileSize)
                    throw new ArgumentException($"File size exceeds maximum allowed size of {_maxFileSize / 1024 / 1024}MB");

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_allowedExtensions.Contains(extension))
                    throw new ArgumentException($"File type {extension} is not allowed");

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(_uploadPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var attachment = new TaskAttachment
                {
                    Id = Guid.NewGuid(),
                    FileName = uniqueFileName,
                    OriginalFileName = file.FileName,
                    ContentType = file.ContentType,
                    FileSize = file.Length,
                    FilePath = filePath,
                    TaskItemId = taskId,
                    ApplicationUserId = userId,
                    UploadedAt = DateTime.UtcNow
                };

                await _context.TaskAttachments.AddAsync(attachment);
                await _context.SaveChangesAsync();

                return await GetAttachmentByIdAsync(attachment.Id) ?? attachment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading attachment for task {taskId}");
                throw;
            }
        }

        public async Task<bool> DeleteAttachmentAsync(Guid id, string userId)
        {
            try
            {
                var attachment = await _context.TaskAttachments.FindAsync(id);
                if (attachment == null) return false;

                if (attachment.ApplicationUserId != userId)
                    throw new UnauthorizedAccessException("You can only delete your own attachments");

                if (File.Exists(attachment.FilePath))
                    File.Delete(attachment.FilePath);

                _context.TaskAttachments.Remove(attachment);
                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting attachment {id}");
                throw;
            }
        }

        public async Task<(Stream fileStream, string contentType, string fileName)?> DownloadAttachmentAsync(Guid id)
        {
            try
            {
                var attachment = await GetAttachmentByIdAsync(id);
                if (attachment == null || !File.Exists(attachment.FilePath))
                    return null;

                var memory = new MemoryStream();
                using (var stream = new FileStream(attachment.FilePath, FileMode.Open, FileAccess.Read))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;

                return (memory, attachment.ContentType, attachment.OriginalFileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading attachment {id}");
                return null;
            }
        }
    }
}