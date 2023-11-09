
using RhondaLibraryPOC.Application.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Users.Queries;
using RhondaLibraryPOC.Application.Users;

namespace RhondaLibraryPOC.Application.Interfaces;

public interface IUserRepository
{
    Task<UserDTO> GetUserById(GetUserDetailsQuery query);
    Task<UserDTO> AddUser(AddUserCommand command);
    Task<UserDTO> UpdateUser(UpdateUserCommand command);
}
