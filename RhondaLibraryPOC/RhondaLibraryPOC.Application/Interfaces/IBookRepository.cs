using ErrorOr;
using RhondaLibraryPOC.Application.CQRS.Books;
using RhondaLibraryPOC.Application.CQRS.Books.Commands;
using RhondaLibraryPOC.Application.CQRS.Books.Queries;
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.Interfaces;

public interface IBookRepository
{
    Task<ErrorOr<BookDTO?>> AddBook(Book book, CancellationToken cancellation);
    Task<ErrorOr<IEnumerable<BookDTO>>> GetBooks(CancellationToken cancellation);
    Task<ErrorOr<BookDTO>> GetBookById(GetBookDetailsQuery query, CancellationToken cancellation);
    Task<ErrorOr<BookDTO>> UpdateBook(UpdateBookCommand command, CancellationToken cancellation);
    Task<ErrorOr<BookDTO>> DeleteBook(DeleteBookCommand command, CancellationToken cancellation);
}
