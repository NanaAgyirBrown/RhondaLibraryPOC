
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.CQRS.Books.Commands;

public class AddBookCommand : IRequest<ErrorOr<Book>>
{
    public BookDTO BookDTO { get; set; }
    public AddBookCommand(BookDTO bookDTO)
    {
        BookDTO = bookDTO;
    }
}


public class AddBookHandler : IRequestHandler<AddBookCommand, ErrorOr<Book>>
{
    private readonly IBookRepository _bookRepository;
    public AddBookHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    public async Task<ErrorOr<Book>> Handle(AddBookCommand command, CancellationToken cancellationToken)
    {
        var book = new Book
        {
            Title = command.BookDTO.Title,
            Author = command.BookDTO.Author,
            ISBN = command.BookDTO.ISBN,
            Publisher = command.BookDTO.Publisher,
            Genre = command.BookDTO.Genre,
            IsAvailable = command.BookDTO.IsAvailable
        };

        var result = await _bookRepository.AddBook(book);

        return result;
    }
}