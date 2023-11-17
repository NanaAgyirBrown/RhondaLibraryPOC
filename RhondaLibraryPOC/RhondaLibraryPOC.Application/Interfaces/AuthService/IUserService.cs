using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RhondaLibraryPOC.Application.Interfaces.AuthService
{
    public interface IUserService
    {
        string GetRoleByUsername(string username);
        string GenerateJwtToken((string name, string role) user);
    }
}
