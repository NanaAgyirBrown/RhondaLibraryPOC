
using ErrorOr;
using MediatR;
using RhondaLibraryPOC.Application.Interfaces;

namespace RhondaLibraryPOC.Application.CQRS.Books.Queries;

public class GetAllBooksQuery : IRequest<ErrorOr<IEnumerable<BookDTO>>>
{
    public GetAllBooksQuery()
    {
    }
}

public class GetAllBooksHandler : IRequestHandler<GetAllBooksQuery, ErrorOr<IEnumerable<BookDTO>>>
{
    private readonly IBookRepository _bookRepository;

    public GetAllBooksHandler(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    async Task<ErrorOr<IEnumerable<BookDTO>>> IRequestHandler<GetAllBooksQuery, ErrorOr<IEnumerable<BookDTO>>>.Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
       return await _bookRepository.GetBooks(cancellationToken);
    }
}