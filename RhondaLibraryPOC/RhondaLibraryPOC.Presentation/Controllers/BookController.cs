using MediatR;
using Microsoft.AspNetCore.Mvc;
using RhondaLibraryPOC.Application.CQRS.Books.Commands;
using RhondaLibraryPOC.Application.CQRS.Books.Queries;
using System.Net;
using RhondaLibraryPOC.Presentation.Common;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize]
    public async Task<IActionResult> AddBook([FromBody] AddBookCommand command)
    {
        _logger.LogInformation("Adding a new book");
        var result = await _mediatR.Send(command);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }

    [HttpGet(Name = "GetAllBooks")]
    public async Task<IActionResult> GetAllBooks()
    {
        _logger.LogInformation("Getting all books");
        var query = new GetAllBooksQuery();
        var result = await _mediatR.Send(query);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }

    [HttpGet("{Isbn}", Name = "GetBookByIsbn")]
    public async Task<IActionResult> GetBookByIsbn([FromRoute]GetBookDetailsQuery query)
    {
        _logger.LogInformation("Getting book by isbn");
        var result = await _mediatR.Send(query);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }
 
    [HttpPut(Name = "UpdateBook")]
    [Authorize("Admin")]
    public async Task<IActionResult> UpdateBook([FromBody] UpdateBookCommand command)
    {
        _logger.LogInformation("Updating book");
        var result = await _mediatR.Send(command);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }

    [HttpDelete("{Isbn}" ,Name = "DeleteBook")]
    [Authorize("Admin")]
    public async Task<IActionResult> DeleteBook([FromRoute] DeleteBookCommand command)
    {
        _logger.LogInformation("Deleting book");
        var result = await _mediatR.Send(command);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }
}