
using RhondaLibraryPOC.Application.CQRS.Checkouts;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Queries;
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.Interfaces;

public interface ICheckoutRepository
{
    Task<CheckoutDTO> CheckoutBooks(Checkout command, CancellationToken cancellationToken);
    Task<CheckoutDTO> ReturnBooks(ReturnBooksCommand command, CancellationToken cancellationToken);
    Task<CheckoutDTO> BooksNotReturned(GetBooksNotReturnedQuery query, CancellationToken cancellationToken);
}