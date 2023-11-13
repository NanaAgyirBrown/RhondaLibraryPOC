using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;

public class CheckoutBooksCommand : IRequest<ErrorOr<CheckoutRecord>>
{
    public CheckoutBookList _checkoutDetails { get; set; }

    public CheckoutBooksCommand(CheckoutBookList checkoutBookList)
    {
        _checkoutDetails = checkoutBookList;
    }
}

public class CheckoutBooksHandler : IRequestHandler<CheckoutBooksCommand, ErrorOr<CheckoutRecord>>
{
    private readonly ICheckoutRepository _checkoutRepository;
    private readonly Extras.Extras _extras;

    public CheckoutBooksHandler(ICheckoutRepository checkoutRepository, Extras.Extras extras)
    {
        _checkoutRepository = checkoutRepository;
        _extras = extras;
    }

    public async Task<ErrorOr<CheckoutRecord>> Handle(CheckoutBooksCommand command, CancellationToken cancellationToken)
    {
        if (command._checkoutDetails.BookList.Count() > 5)
        {
            return Error.Validation(
                    code: "TooManyBooks",
                    description: "You can only checkout 5 books at a time."
                );
        }

        // Validing if all books are available
        foreach (var book in command._checkoutDetails.BookList)
        {
            var bookResult = await _extras.GetBookExists(book.BookId, cancellationToken);

            if (bookResult.IsError)
                return Error.Validation(
                       code: "BookNotFound",
                       description: $"Book with ISBN - {book.BookId} not found."
                    );
        }

        foreach (var book in command._checkoutDetails.BookList)
        {
            book.ExpectedReturnDate = await _extras.CalculateReturnDate(DateTime.Now, book.BookId);
        }

        var result = await _checkoutRepository.CheckoutBooks(command, cancellationToken);

        return result;
    }
}