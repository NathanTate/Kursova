namespace API.Helpers;

public class NoteParams : PaginationParams
{
    public string OrderBy { get; set; } = "lastCreated";
}
