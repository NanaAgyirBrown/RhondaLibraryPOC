namespace RhondaLibraryPOC.Application.Interfaces.AuthService
{
    public interface IUserService
    {
        string GetRoleByUsername(string username);
        string GenerateJwtToken((string name, string role) user);
    }
}
