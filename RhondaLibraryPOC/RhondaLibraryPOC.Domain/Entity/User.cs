
using RhondaLibraryPOC.Domain.Common;

namespace RhondaLibraryPOC.Domain.Entity;

public class User : BaseAuditableEntity
{
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public DateTime? RegistrationDate { get; set; }
}
