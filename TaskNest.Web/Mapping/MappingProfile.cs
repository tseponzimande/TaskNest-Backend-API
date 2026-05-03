namespace TaskNest.API.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Board, BoardDto>().ReverseMap();
            CreateMap<BoardColumn, BoardColumnDto>().ReverseMap();
            CreateMap<TaskItem, TaskItemDto>().ReverseMap();
            CreateMap<BoardUser, BoardUserDto>().ReverseMap();
            CreateMap<ApplicationUser, BoardUserDto>();
            CreateMap<ActivityLog, ActivityLogDto>().ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<userManagement, CommentDto>().ReverseMap();
            CreateMap<TaskAttachment, TaskAttachmentDto>().ReverseMap();
        }
    }
}