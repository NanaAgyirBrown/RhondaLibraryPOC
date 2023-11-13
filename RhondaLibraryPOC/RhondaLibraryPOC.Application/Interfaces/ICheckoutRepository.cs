﻿using ErrorOr;
using RhondaLibraryPOC.Application.CQRS.Checkouts;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Queries;

namespace RhondaLibraryPOC.Application.Interfaces;

public interface ICheckoutRepository
{
    Task<ErrorOr<CheckoutRecord>> CheckoutBooks(CheckoutBooksCommand command, CancellationToken cancellationToken);
    Task<CheckoutDTO> ReturnBooks(ReturnBooksCommand command, CancellationToken cancellationToken);
    Task<CheckoutDTO> BooksNotReturned(GetBooksNotReturnedQuery query, CancellationToken cancellationToken);
}