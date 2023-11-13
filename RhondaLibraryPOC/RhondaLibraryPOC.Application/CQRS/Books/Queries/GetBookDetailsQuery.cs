
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application.CQRS.Books.Queries;

public class GetBookDetailsQuery : IRequest<ErrorOr<BookDTO>>
{
    public string? Isbn { get; set; }
}

public class GetBookDetailsHandler : IRequestHandler<GetBookDetailsQuery, ErrorOr<BookDTO>>
{
    private readonly IBookRepository _bookRepository;

    public GetBookDetailsHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    async Task<ErrorOr<BookDTO>> IRequestHandler<GetBookDetailsQuery, ErrorOr<BookDTO>>.Handle(GetBookDetailsQuery request, CancellationToken cancellationToken)
    {
        return await _bookRepository.GetBookById(request, cancellationToken);
    }
}