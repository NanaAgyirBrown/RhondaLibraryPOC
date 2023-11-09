namespace RhondaLibraryPOC.Domain.Entity;

public class Librarian
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public Role Role { get; set; }
}


public enum Role
{
    Admin,
    Librarian
}