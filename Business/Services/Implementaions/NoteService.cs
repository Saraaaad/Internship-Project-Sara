using InternshipProjectSara.Data.Context;

public class NoteService : INoteService
{
    private readonly INoteRepository nrepository;
    private readonly IUserRepository uRepository;
    private readonly DatabaseContext _context;
    private readonly ILogger<NoteService> logger;

    public NoteService(INoteRepository repository, DatabaseContext context, IUserRepository userRepository, ILogger<NoteService> logger)
    {
        nrepository = repository;
        uRepository = userRepository;
        _context = context;
        this.logger = logger;
    }

    public NoteResponseDto GetById(int id)
    {
        logger.LogInformation("Retrieving note with ID: {id}", id);
        var note = nrepository.GetById(id);
        if (note == null)
        {
            logger.LogWarning("Note with ID: {id} not found", id);
            throw new NotFoundException("Note", id);
        }
        logger.LogInformation("Note with ID: {id} retrieved successfully", id);
        return note.ToDto<Note, NoteResponseDto>();
    }

    public List<NoteResponseDto> GetByEmployeeId(int employeeId)
    {
        logger.LogInformation("Retrieving notes for employee with ID: {employeeId}", employeeId);
        var employee = uRepository.GetById(employeeId);
        if (employee == null)
        {
            logger.LogWarning("Employee with ID: {employeeId} not found", employeeId);
            throw new NotFoundException("Employee", employeeId);
        }
        var notes = nrepository.GetByEmployeeId(employeeId);
        if (notes == null || notes.Count == 0)
            logger.LogInformation("No notes found for employee with ID: {employeeId}", employeeId);

        logger.LogInformation("Notes retrieved successfully for employee with ID: {employeeId}", employeeId);
        return notes.ToDtoList<Note, NoteResponseDto>();
    }

    public NoteResponseDto Create(NoteRequestDto dto)
    {
        logger.LogInformation("Creating note");
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var note = dto.ToEntity<NoteRequestDto, Note>();
        var employee = uRepository.GetById(note.EmployeeId);
        if (employee == null)
        {
            logger.LogWarning("Employee with ID: {employeeId} not found", note.EmployeeId);
            throw new NotFoundException("Employee", note.EmployeeId);
        }
        nrepository.Add(note);
        _context.SaveChanges();
        logger.LogInformation("Note created successfully with ID: {id}", note.Id);
        return note.ToDto<Note, NoteResponseDto>();
    }

    public void Delete(int id)
    {
        logger.LogInformation("Deleting note with ID: {id}", id);
        var note = nrepository.GetById(id);
        if (note == null)
        {
            logger.LogWarning("Note with ID: {id} not found", id);
            throw new NotFoundException("Note", id);
        }
        nrepository.Delete(id);
        _context.SaveChanges();
        logger.LogInformation("Note with ID: {id} deleted successfully", id);
    }

    public NoteResponseDto Update(int id, NoteRequestDto dto)
    {
        logger.LogInformation("Updating note with ID: {id}", id);
        ArgumentNullException.ThrowIfNull(dto, nameof(dto));

        var note = nrepository.GetById(id);
        if (note == null)
        {
            logger.LogWarning("Note with ID: {id} not found", id);
            throw new NotFoundException("Note", id);
        }
        var employee = uRepository.GetById(dto.EmployeeId);
        if (employee == null)
        {
            logger.LogWarning("Employee with ID: {employeeId} not found", dto.EmployeeId);
            throw new NotFoundException("Employee", dto.EmployeeId);
        }
        note.Title = dto.Title;
        note.Content = dto.Content;
        note.EmployeeId = dto.EmployeeId;


        nrepository.Update(note);
        _context.SaveChanges();
        logger.LogInformation("Note with ID: {id} updated successfully", note.Id);
        return note.ToDto<Note, NoteResponseDto>();
    }
}