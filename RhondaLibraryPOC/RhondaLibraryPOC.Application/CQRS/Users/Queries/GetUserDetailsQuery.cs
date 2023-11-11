
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;
using RhondaLibraryPOC.Application.Users;

namespace RhondaLibraryPOC.Application.CQRS.Users.Queries;

public class GetUserDetailsQuery : IRequest<ErrorOr<UserDTO>>
{
    public Guid Id { get; set; }

    public GetUserDetailsQuery(Guid id)
    {
        Id = id;
    }
} 

public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, ErrorOr<UserDTO>>
{
    private readonly IUserRepository _userRepository;

    public GetUserDetailsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<UserDTO>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {

        var user = await _userRepository.GetUserById(request, cancellationToken);
        return user;
    }
}