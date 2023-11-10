using MediatR;
using Microsoft.AspNetCore.Mvc;
using RhondaLibraryPOC.Application.CQRS.Books.Commands;
using RhondaLibraryPOC.Application.CQRS.Books.Queries;

namespace RhondaLibraryPOC.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class BookController : ControllerBase
{
    private readonly ILogger<BookController> _logger;
    private readonly IMediator _mediatR;
    public BookController(ILogger<BookController> logger, IMediator mediatR)
    {
        _logger = logger;
        _mediatR = mediatR;
    }

    [HttpPost(Name = "AddBook")]
    public async Task<IActionResult> AddBook([FromBody]AddBookCommand command)
    {
       var result = await _mediatR.Send(command);

        return result.Match<IActionResult>
            (book => 
                new CreatedAtRouteResult(
                 "AddBook", new { 
                     id = book.Id }, book),
                     error => new BadRequestObjectResult(error));
    }

    [HttpGet(Name = "GetAllBooks")]
    public async Task<IActionResult> GetAllBooks()
    {
        var query = new GetAllBooksQuery();
        var result = await _mediatR.Send(query);

        return result.Match<IActionResult>
            (books => new OkObjectResult(books),
                       error => new BadRequestObjectResult(error));
    }   
}
