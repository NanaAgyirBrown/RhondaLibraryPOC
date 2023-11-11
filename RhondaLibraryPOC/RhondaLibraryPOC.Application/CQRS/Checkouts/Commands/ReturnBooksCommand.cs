
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;

public class ReturnBooksCommand : IRequest<ErrorOr<CheckoutDTO>>
{
    public Guid Id { get; set; }
    public List<Guid> BookIds { get; set; }

    public ReturnBooksCommand(Guid id, Guid userId, List<Guid> bookIds)
    {
        Id = id;
        BookIds = bookIds;
    }
}

public class ReturnBooksHandler : IRequestHandler<ReturnBooksCommand, ErrorOr<CheckoutDTO>>
{
    private readonly ICheckoutRepository _checkoutRepository;

    public ReturnBooksHandler(ICheckoutRepository checkoutRepository)
    {
        _checkoutRepository = checkoutRepository;
    }

    public async Task<ErrorOr<CheckoutDTO>> Handle(ReturnBooksCommand command, CancellationToken cancellationToken)
    {
        var checkout = await _checkoutRepository.ReturnBooks(command, cancellationToken);

        return checkout;
    }
}