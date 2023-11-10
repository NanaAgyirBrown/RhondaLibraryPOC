
namespace RhondaLibraryPOC.Application.CQRS.Books;

public class BookDTO
{
    public string Title { get; set; }
    public string Author { get; set; }
    public string ISBN { get; set; }
    public string Publisher { get; set; }
    public string Genre { get; set; }
    public bool IsAvailable { get; set; }
}
