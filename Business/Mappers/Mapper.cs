using AutoMapper;

public static class Mapper
{
    private static IMapper mapper;

    public static void Initialize()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserRequestDto, User>();
            cfg.CreateMap<User, UserResponseDto>();
            cfg.CreateMap<DepartmentRequestDto, Department>();
            cfg.CreateMap<Department, DepartmentResponseDto>();
            cfg.CreateMap<NoteRequestDto, Note>();
            cfg.CreateMap<Note, NoteResponseDto>();
            cfg.CreateMap<TaskRequestDto, Tasks>();
            cfg.CreateMap<Tasks, TaskResponseDto>();
        });

        mapper = config.CreateMapper();
    }

    public static TDto ToDto<TEntity, TDto>(this TEntity entity)
    {
        return mapper.Map<TDto>(entity);
    }

    public static TEntity ToEntity<TDto, TEntity>(this TDto dto)
    {
        return mapper.Map<TEntity>(dto);
    }

    public static List<TDto> ToDtoList<TEntity, TDto>(this IEnumerable<TEntity> entities)
    {
        return mapper.Map<List<TDto>>(entities);
    }
}