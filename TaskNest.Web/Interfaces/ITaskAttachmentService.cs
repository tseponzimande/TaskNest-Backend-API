namespace TaskNest.API.Interfaces
{
    public interface ITaskAttachmentService
    {
        Task<IEnumerable<TaskAttachment>> GetAttachmentsByTaskIdAsync(Guid taskId);
        Task<TaskAttachment?> GetAttachmentByIdAsync(Guid id);
        Task<TaskAttachment> UploadAttachmentAsync(Guid taskId, IFormFile file, string userId);
        Task<bool> DeleteAttachmentAsync(Guid id, string userId);
        Task<(Stream fileStream, string contentType, string fileName)?> DownloadAttachmentAsync(Guid id);
    }
}
