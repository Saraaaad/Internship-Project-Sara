public interface INoteService
{
    NoteResponseDto GetById(int id);
    List<NoteResponseDto> GetByEmployeeId(int employeeId);
    NoteResponseDto Create(NoteRequestDto dto);
    NoteResponseDto Update(int id, NoteRequestDto dto);
    void Delete(int id);
}