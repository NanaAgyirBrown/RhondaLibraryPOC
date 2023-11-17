using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;
using System.Linq.Expressions;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;

public class CheckoutBookCommand : IRequest<ErrorOr<UserCheckout>>
{
    public CheckoutBookList CheckoutBookList { get; set; }
    public CheckoutBookCommand(CheckoutBookList checkoutBookList)
    {
        CheckoutBookList = checkoutBookList;
    }
}

public class CheckoutBookHandler : IRequestHandler<CheckoutBookCommand, ErrorOr<UserCheckout>>
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly Extras.Extras _extras;

    public CheckoutBookHandler(ICheckoutRepository checkoutRepository, Extras.Extras extras)
    {
        _checkoutRepository = checkoutRepository;
        _extras = extras;
    }

    async Task<ErrorOr<UserCheckout>> IRequestHandler<CheckoutBookCommand, ErrorOr<UserCheckout>>.Handle(CheckoutBookCommand request, CancellationToken cancellationToken)
    {
        int max = _extras.GetMaxCheckoutBooks();

        if (request.CheckoutBookList.BookList.Count() > max)
        {
            return Error.Validation(
                code: $"MaxBookExceeded",
                description: $"You can only checkout {max} books"
            );
        }

        // Validating if all Books are available
        foreach (var book in request.CheckoutBookList.BookList)
        {
            var bookExists = await _extras.GetBookExists(book.BookId, cancellationToken);

            if (bookExists.IsError)
            {
                return Error.Validation(
                        code: $"BookDoesNotExist",
                        description: $"Book with ISBN {book.BookId} does not exist"
                );
            }
            book.Returned = false;
            book.ExpectedReturnDate = await _extras.CalculateReturnDate(DateTime.Now, book.BookId);
        }

        return await _checkoutRepository.CheckoutBooks(request, cancellationToken);
    }
}
