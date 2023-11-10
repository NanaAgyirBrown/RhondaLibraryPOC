using ErrorOr;
using RhondaLibraryPOC.Application.CQRS.Books;
using RhondaLibraryPOC.Application.CQRS.Books.Commands;
using RhondaLibraryPOC.Application.CQRS.Books.Queries;
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<BookDTO>> GetBooks(GetAllBooksQuery query);
    Task<BookDTO> GetBookById(GetBookDetailsQuery query);
    Task<ErrorOr<Book?>> AddBook(Book book);
    Task<BookDTO> UpdateBook(UpdateBookCommand command);
    Task<BookDTO> DeleteBook(DeleteBookCommand command);
}
