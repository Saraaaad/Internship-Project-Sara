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
            throw new Exception("Note not found");

        return note.ToDto<Note, NoteResponseDto>();
    }

    public List<NoteResponseDto> GetByEmployeeId(int employeeId)
    {
        var notes = nrepository.GetByEmployeeId(employeeId);
        return notes.ToDtoList<Note, NoteResponseDto>();
    }

    public NoteResponseDto Create(NoteRequestDto dto)
    {
        var note = dto.ToEntity<NoteRequestDto, Note>();
        var employee = uRepository.GetById(note.EmployeeId);
        if (employee == null) 
            throw new Exception("Employee not found");
            
        nrepository.Add(note);
        _context.SaveChanges();
        return note.ToDto<Note, NoteResponseDto>();
    }

    public void Delete(int id)
    {
        var note = nrepository.GetById(id);
        if (note == null) 
            throw new Exception("Note not found");

        nrepository.Delete(id);
        _context.SaveChanges();
    }

    public NoteResponseDto Update(int id, NoteRequestDto dto)
    {
        var note = nrepository.GetById(id);
        if (note == null) 
            throw new Exception("Note not found");

        note.Title = dto.Title;
        note.Content = dto.Content;
        note.EmployeeId = dto.EmployeeId;


        nrepository.Update(note);
        _context.SaveChanges();
        return note.ToDto<Note, NoteResponseDto>();
    }
}