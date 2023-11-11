
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Queries;

public class GetBooksNotReturnedQuery : IRequest<ErrorOr<CheckoutDTO>>
{
    public Guid UserId { get; set; }

    public GetBooksNotReturnedQuery(Guid userId)
    {
        UserId = userId;
    }
}

public class GetBooksNotReturnedQueryHandler : IRequestHandler<GetBooksNotReturnedQuery, ErrorOr<CheckoutDTO>>
{
    private readonly ICheckoutRepository _checkoutRepository;

    public GetBooksNotReturnedQueryHandler(ICheckoutRepository checkoutRepository)
    {
        _checkoutRepository = checkoutRepository;
    }

    public async Task<ErrorOr<CheckoutDTO>> Handle(GetBooksNotReturnedQuery request, CancellationToken cancellationToken)
    {
        var checkout = await _checkoutRepository.BooksNotReturned(request, cancellationToken);
        return checkout;
    }
}
