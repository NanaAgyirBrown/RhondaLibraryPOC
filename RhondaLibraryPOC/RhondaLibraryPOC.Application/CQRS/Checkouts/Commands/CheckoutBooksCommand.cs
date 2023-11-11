
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;

public class CheckoutBooksCommand : IRequest<ErrorOr<CheckoutDTO>>
{
    public CheckoutDTO CheckoutDTO { get; set; }
    
    public CheckoutBooksCommand(CheckoutDTO checkoutDTO)
    {
        CheckoutDTO = checkoutDTO;
    }
}

public class CheckoutBooksHandler : IRequestHandler<CheckoutBooksCommand, ErrorOr<CheckoutDTO>>
{
    private readonly ICheckoutRepository _checkoutRepository;
    public CheckoutBooksHandler(ICheckoutRepository checkoutRepository)
    {
        _checkoutRepository = checkoutRepository;
    }

    public async Task<ErrorOr<CheckoutDTO>> Handle(CheckoutBooksCommand command, CancellationToken cancellationToken)
    {
        var checkout = new Checkout
        {
            UserId = command.CheckoutDTO.Userid,
            BookId = command.CheckoutDTO.BookId,
            CheckoutDate = command.CheckoutDTO.CheckoutDate,
            ReturnDate = Extras.Extras.CalculateReturnDate(DateTime.Now, command.CheckoutDTO.Book?.Genre),
            IsReturned = command.CheckoutDTO.Returned,
            Fine = 0.00m
        };

        var result = await _checkoutRepository.CheckoutBooks(checkout, cancellationToken);

        return result;
    }
}   