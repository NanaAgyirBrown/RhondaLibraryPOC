
using System.ComponentModel;

namespace RhondaLibraryPOC.Application.Users;

public class UserDTO
{    
    public Guid Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Address { get; set; } = null!;
    public DateTime? RegistrationDate { get; set; }

}
