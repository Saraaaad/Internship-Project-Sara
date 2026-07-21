using AutoMapper;

public static class Mapper
{
    private static IMapper mapper;

    public static void Initialize()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<UserRequestDto, User>();
            cfg.CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
            .ForMember(dest => dest.Tasks, opt => opt.MapFrom(src => src.Tasks));

            cfg.CreateMap<DepartmentRequestDto, Department>();
            cfg.CreateMap<Department, DepartmentResponseDto>()
            .ForMember(dest => dest.EmployeeCount, opt => opt.MapFrom(src => src.Employees.Count))
            .ForMember(dest => dest.EmployeeNames, opt => opt.MapFrom(src => src.Employees.Select(e => e.FullName).ToList()));
            cfg.CreateMap<NoteRequestDto, Note>();
            cfg.CreateMap<Note, NoteResponseDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src =>src.Employee != null ? src.Employee.FullName : null));
            cfg.CreateMap<TaskRequestDto, Tasks>();
            cfg.CreateMap<Tasks, TaskResponseDto>()
            .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.FullName : null));;
            cfg.CreateMap<RegistrationRequestDto, User>();
            cfg.CreateMap<User, RegistrationResponseDto>();
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