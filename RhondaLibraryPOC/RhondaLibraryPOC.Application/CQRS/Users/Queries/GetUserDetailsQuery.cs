
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;
using RhondaLibraryPOC.Application.Users;

namespace RhondaLibraryPOC.Application.CQRS.Users.Queries;

public class GetUserDetailsQuery : IRequest<ErrorOr<UserRecord>>
{
    public string Id { get; set; }
} 

public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, ErrorOr<UserRecord>>
{
    private readonly IUserRepository _userRepository;

    public GetUserDetailsQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<UserRecord>> Handle(GetUserDetailsQuery request, CancellationToken cancellationToken)
    {

        var user = await _userRepository.GetUserById(request, cancellationToken);

        return user;
    }
}