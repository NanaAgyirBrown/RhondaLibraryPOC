
using RhondaLibraryPOC.Application.CQRS.Checkouts;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Queries;

namespace RhondaLibraryPOC.Application.Interfaces;

public interface ICheckoutRepository
{
    Task<CheckoutDTO> CheckoutBooks(CheckoutBooksCommand command);
    Task<CheckoutDTO> ReturnBooks(ReturnBooksCommand command);
    Task<CheckoutDTO> BooksNotReturned(GetBooksNotReturnedQuery query);
}