
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application.CQRS.Books.Commands;

public class DeleteBookCommand : IRequest<ErrorOr<BookDTO>>
{
    public string? Isbn { get; set; }
}

public class DeleteBookHandler : IRequestHandler<DeleteBookCommand, ErrorOr<BookDTO>>
{
    private readonly IBookRepository _bookRepository;

    public DeleteBookHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<ErrorOr<BookDTO>> Handle(DeleteBookCommand command, CancellationToken cancellationToken)
    {
        var result = await _bookRepository.DeleteBook(command, cancellationToken);
        return result;
    }
}
