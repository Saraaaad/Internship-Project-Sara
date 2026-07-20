using InternshipProjectSara.Data.Context;

public class NoteService : INoteService
{
    private readonly INoteRepository nrepository;
    private readonly IUserRepository uRepository;
    private readonly DatabaseContext _context;
    private readonly ILogger<NoteService> logger;
    private readonly ILogService logService;

    public NoteService(INoteRepository repository, DatabaseContext context, IUserRepository userRepository, ILogger<NoteService> logger, ILogService logService)
    {
        nrepository = repository;
        uRepository = userRepository;
        _context = context;
        this.logger = logger;
        this.logService = logService;
    }

    public NoteResponseDto GetById(int id)
    {
        logger.LogInformation("Retrieving note with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Retrieving note with ID: {id}");
        var note = nrepository.GetById(id);
        if (note == null)
        {
            logger.LogWarning("Note with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"Note with ID: {id} not found");
            throw new NotFoundException("Note", id);
        }
        logger.LogInformation("Note with ID: {id} retrieved successfully", id);
        logService.Log(LogLevel.Information, $"Note with ID: {id} retrieved successfully");
        return note.ToDto<Note, NoteResponseDto>();
    }

    public List<NoteResponseDto> GetByEmployeeId(int employeeId)
    {
        logger.LogInformation("Retrieving notes for employee with ID: {employeeId}", employeeId);
        logService.Log(LogLevel.Information, $"Retrieving notes for employee with ID: {employeeId}");
        var employee = uRepository.GetById(employeeId);
        if (employee == null)
        {
            logger.LogWarning("Employee with ID: {employeeId} not found", employeeId);
            logService.Log(LogLevel.Warning, $"Employee with ID: {employeeId} not found");
            throw new NotFoundException("Employee", employeeId);
        }
        var notes = nrepository.GetByEmployeeId(employeeId);
        if (notes == null || notes.Count == 0)
        {
            logService.Log(LogLevel.Warning, $"No notes found for employee with ID: {employeeId}");
            logger.LogInformation("No notes found for employee with ID: {employeeId}", employeeId);
        }

        logService.Log(LogLevel.Information, $"Notes retrieved successfully for employee with ID: {employeeId}");
        logger.LogInformation("Notes retrieved successfully for employee with ID: {employeeId}", employeeId);
        return notes.ToDtoList<Note, NoteResponseDto>();
    }

    public NoteResponseDto Create(NoteRequestDto dto)
    {
        logger.LogInformation("Creating note");
        logService.Log(LogLevel.Information, "Creating note");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var note = dto.ToEntity<NoteRequestDto, Note>();
        var employee = uRepository.GetById(note.EmployeeId);
        if (employee == null)
        {
            logService.Log(LogLevel.Warning, $"Employee with ID: {note.EmployeeId} not found");
            logger.LogWarning("Employee with ID: {employeeId} not found", note.EmployeeId);
            throw new NotFoundException("Employee", note.EmployeeId);
        }
        nrepository.Add(note);
        _context.SaveChanges();
        logger.LogInformation("Note created successfully with ID: {id}", note.Id);
        logService.Log(LogLevel.Information, $"Note created successfully with ID: {note.Id}");
        return note.ToDto<Note, NoteResponseDto>();
    }

    public void Delete(int id)
    {
        logger.LogInformation("Deleting note with ID: {id}", id);
        logService.Log(LogLevel.Information, $"Deleting note with ID: {id}");
        var note = nrepository.GetById(id);
        if (note == null)
        {
            logger.LogWarning("Note with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"Note with ID: {id} not found");
            throw new NotFoundException("Note", id);
        }
        nrepository.Delete(id);
        _context.SaveChanges();
        logger.LogInformation("Note with ID: {id} deleted successfully", id);
        logService.Log(LogLevel.Information, $"Note with ID: {id} deleted successfully");
    }

    public NoteResponseDto Update(int id, NoteRequestDto dto)
    {
        logService.Log(LogLevel.Information, $"Updating note with ID: {id}");
        logger.LogInformation("Updating note with ID: {id}", id);
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var note = nrepository.GetById(id);
        if (note == null)
        {
            logger.LogWarning("Note with ID: {id} not found", id);
            logService.Log(LogLevel.Warning, $"Note with ID: {id} not found");
            throw new NotFoundException("Note", id);
        }
        var employee = uRepository.GetById(dto.EmployeeId);
        if (employee == null)
        {
            logger.LogWarning("Employee with ID: {employeeId} not found", dto.EmployeeId);
            logService.Log(LogLevel.Warning, $"Employee with ID: {dto.EmployeeId} not found");
            throw new NotFoundException("Employee", dto.EmployeeId);
        }
        note.Title = dto.Title;
        note.Content = dto.Content;
        note.EmployeeId = dto.EmployeeId;

        nrepository.Update(note);
        _context.SaveChanges();
        
        logger.LogInformation("Note with ID: {id} updated successfully", note.Id);
        logService.Log(LogLevel.Information, $"Note with ID: {note.Id} updated successfully");
        return note.ToDto<Note, NoteResponseDto>();
    }
}