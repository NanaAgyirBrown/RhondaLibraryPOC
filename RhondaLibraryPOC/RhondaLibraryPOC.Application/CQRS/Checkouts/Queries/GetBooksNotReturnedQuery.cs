
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Queries;

public class GetBooksNotReturnedQuery : IRequest<ErrorOr<UserCheckout>>
{
    public string UserId { get; set; }
    public string? CheckoutId { get; set; }

    public GetBooksNotReturnedQuery(string UserId, string? CheckoutId)
    {
        this.UserId = UserId;
        this.CheckoutId = CheckoutId;
    }
}

public class GetBooksNotReturnedHandler : IRequestHandler<GetBooksNotReturnedQuery, ErrorOr<UserCheckout>>
{
    private readonly ICheckoutRepository _checkoutRepository;

    public GetBooksNotReturnedHandler(ICheckoutRepository checkoutRepository)
    {
        _checkoutRepository = checkoutRepository;
    }

    public async Task<ErrorOr<UserCheckout>> Handle(GetBooksNotReturnedQuery request, CancellationToken cancellationToken)
    {
        return await _checkoutRepository.BooksNotReturned(request, cancellationToken);
    }
}
