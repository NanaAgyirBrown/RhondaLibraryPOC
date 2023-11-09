using RhondaLibraryPOC.Application.CQRS.Books;
using RhondaLibraryPOC.Application.CQRS.Books.Commands;
using RhondaLibraryPOC.Application.CQRS.Books.Queries;

namespace RhondaLibraryPOC.Application.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<BookDTO>> GetBooks(GetAllBooksQuery query);
    Task<BookDTO> GetBookById(GetBookDetailsQuery query);
    Task<BookDTO> AddBookAsync(AddBookCommand command);
    Task<BookDTO> UpdateBook(UpdateBookCommand command);
    Task<BookDTO> DeleteBook(DeleteBookCommand command);
}
