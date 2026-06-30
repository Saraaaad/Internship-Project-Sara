using InternshipProjectSara.Data.Context;

public class NoteService : INoteService
{
    private readonly INoteRepository nrepository;
    private readonly IUserRepository uRepository;
    private readonly DatabaseContext _context;

    public NoteService(INoteRepository repository, DatabaseContext context, IUserRepository userRepository)
    {
        nrepository = repository;
        uRepository = userRepository;
        _context = context;
    }

    public NoteResponseDto GetById(int id)
    {
        var note = nrepository.GetById(id);
        if (note == null)
            throw new NotFoundException("Note", id);

        return note.ToDto<Note, NoteResponseDto>();
    }

    public List<NoteResponseDto> GetByEmployeeId(int employeeId)
    {
        var employee = uRepository.GetById(employeeId);
        if (employee == null)
            throw new NotFoundException("Employee", employeeId);

        var notes = nrepository.GetByEmployeeId(employeeId);
        if (notes == null || notes.Count == 0)
            throw new NotFoundException($"No notes found for employee with ID {employeeId}");

        return notes.ToDtoList<Note, NoteResponseDto>();
    }

    public NoteResponseDto Create(NoteRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var note = dto.ToEntity<NoteRequestDto, Note>();
        var employee = uRepository.GetById(note.EmployeeId);
        if (employee == null)
            throw new NotFoundException("Employee", note.EmployeeId);

        nrepository.Add(note);
        _context.SaveChanges();
        return note.ToDto<Note, NoteResponseDto>();
    }

    public void Delete(int id)
    {
        var note = nrepository.GetById(id);
        if (note == null)
            throw new NotFoundException("Note", id);

        nrepository.Delete(id);
        _context.SaveChanges();
    }

    public NoteResponseDto Update(int id, NoteRequestDto dto)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        var note = nrepository.GetById(id);
        if (note == null)
            throw new NotFoundException("Note", id);

        note.Title = dto.Title;
        note.Content = dto.Content;
        note.EmployeeId = dto.EmployeeId;


        nrepository.Update(note);
        _context.SaveChanges();
        return note.ToDto<Note, NoteResponseDto>();
    }
}