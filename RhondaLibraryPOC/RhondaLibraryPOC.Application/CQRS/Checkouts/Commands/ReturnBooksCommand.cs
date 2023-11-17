
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;

public class ReturnBookCommand : IRequest<ErrorOr<UserCheckout>>
{
    public string CheckoutId { get; set; }
    public string UserId { get; set; }
    public IEnumerable<string> Books { get; set; }

    public ReturnBookCommand(string UserId, string CheckoutId, IEnumerable<string> Books)
    {
        this.CheckoutId = CheckoutId;
        this.Books = Books;
        this.UserId = UserId;
    }
}

public class ReturnBookHandler : IRequestHandler<ReturnBookCommand, ErrorOr<UserCheckout>>
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly Extras.Extras _extras;

    public ReturnBookHandler(ICheckoutRepository checkoutRepository, Extras.Extras extras)
    {
        _checkoutRepository = checkoutRepository;
        _extras = extras;
    }

    async Task<ErrorOr<UserCheckout>> IRequestHandler<ReturnBookCommand, ErrorOr<UserCheckout>>.Handle(ReturnBookCommand command, CancellationToken cancellationToken)
    {
        IEnumerable<BookDetail> booksFine = new List<BookDetail>();

        // Pick each book and check if it is overdue
        foreach (var book in command.Books)
        {
            var returnedDetail = await _extras.GetBookCheckedOut(command.CheckoutId, book);

            if (returnedDetail.IsError)
            {
                return Error.Validation(
                    code: $"BookDoesNotExist",
                    description: $"Book with ISBN {book} does not exist in User's checkout books."
                );
            }

            BookDetail bookDetail = new()
            {
                Returned = true,
                CheckoutDate = returnedDetail.Value.CheckoutDate,
                Fine = _extras.IsOverdue(returnedDetail.Value.ExpectedReturnDate)
            };

            booksFine.Append(bookDetail);
        }

        return await _checkoutRepository.ReturnBooks((command.UserId, command.CheckoutId, booksFine), cancellationToken);
    }
}