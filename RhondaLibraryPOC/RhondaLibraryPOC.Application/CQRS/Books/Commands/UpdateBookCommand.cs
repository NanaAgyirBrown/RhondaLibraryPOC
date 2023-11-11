using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;
using RhondaLibraryPOC.Domain.Entity;

namespace RhondaLibraryPOC.Application.CQRS.Books.Commands;

public class UpdateBookCommand : IRequest<ErrorOr<BookDTO>>
{
    public string? Isbn { get; set; }
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Publisher { get; set; }
    public string? Genre { get; set; }
    public bool? IsAvailable { get; set; }

    public UpdateBookCommand(string? isbn, string? title, string? author, string? publisher, string? genre, bool? isAvailable)
    {
        Isbn = isbn;
        Title = title;
        Author = author;
        Publisher = publisher;
        Genre = genre;
        IsAvailable = isAvailable;
    }
}

public class UpdateBookHandler : IRequestHandler<UpdateBookCommand, ErrorOr<BookDTO>>
{
    private readonly IBookRepository _bookRepository;

    public UpdateBookHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<ErrorOr<BookDTO>> Handle(UpdateBookCommand command, CancellationToken cancellationToken)
    {
        var result = await _bookRepository.UpdateBook(command, cancellationToken);
        return result;
    }
}