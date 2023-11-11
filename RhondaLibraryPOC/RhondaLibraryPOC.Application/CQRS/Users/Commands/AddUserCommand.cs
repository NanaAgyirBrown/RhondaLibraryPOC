
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;
using RhondaLibraryPOC.Application.Users;
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.Checkouts.Commands;

public class AddUserCommand : IRequest<ErrorOr<UserDTO>>
{
    public UserDTO UserDTO { get; set; }
    
    public AddUserCommand(UserDTO userDTO)
    {
        UserDTO = userDTO;
    }
}

public class AddUserHandler : IRequest<ErrorOr<UserDTO>>
{
    private readonly IUserRepository _userRepository;
    public AddUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<UserDTO>> Handle(AddUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            FullName = command.UserDTO.FullName,
            Username = command.UserDTO.Username,
            Email = command.UserDTO.Email,
            Address = command.UserDTO.Address,
            RegistrationDate = command.UserDTO.RegistrationDate
        };

        var result = await _userRepository.AddUser(user, cancellationToken);

        return result;
    }
}