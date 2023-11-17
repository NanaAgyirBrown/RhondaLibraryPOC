using ErrorOr;
using RhondaLibraryPOC.Application.CQRS.Checkouts;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Queries;

namespace RhondaLibraryPOC.Application.Interfaces;

public interface ICheckoutRepository
{
    Task<ErrorOr<UserCheckout>> CheckoutBooks(CheckoutBookCommand command, CancellationToken cancellationToken);
    Task<ErrorOr<UserCheckout>> ReturnBooks((string UserId, string CheckoutId,IEnumerable<BookDetail> booksFine) command, CancellationToken cancellationToken);
    Task<ErrorOr<UserCheckout>> BooksNotReturned(GetBooksNotReturnedQuery query, CancellationToken cancellationToken);
}