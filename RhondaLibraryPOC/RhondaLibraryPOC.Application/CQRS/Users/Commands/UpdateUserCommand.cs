
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;
using RhondaLibraryPOC.Application.Users;
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.Checkouts.Commands;

public class UpdateUserCommand : IRequest<ErrorOr<UserDTO>>
{
    public Guid Id { get; set; }
    public UserDTO UserDTO { get; set; }

    public UpdateUserCommand(Guid id, UserDTO userDTO)
    {
        Id = id;
        UserDTO = userDTO;
    }
}

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, ErrorOr<UserDTO>>
{
    private readonly IUserRepository _userRepository;
    public UpdateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<UserDTO>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var user = new User
        {
            Id = command.Id,
            FullName = command.UserDTO.FullName,
            Username = command.UserDTO.Username,
            Email = command.UserDTO.Email,
            Address = command.UserDTO.Address,
            RegistrationDate = command.UserDTO.RegistrationDate
        };

        var result = await _userRepository.UpdateUser(user, cancellationToken);

        return result;
    }
}
