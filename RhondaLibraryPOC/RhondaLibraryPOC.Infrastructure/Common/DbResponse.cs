
namespace RhondaLibraryPOC.Infrastructure.Common;

public class DbResponse
{
    public int StatusCode { get; set; }
    public string? Status { get; set; }
    public string? Message { get; set; }
    public string? Details { get; set; }
}
