using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Queries;
using RhondaLibraryPOC.Presentation.Common;
using System.Net;

namespace RhondaLibraryPOC.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize(Roles = "Admin, Librarian")]
public class CheckoutController : ControllerBase
{
    private readonly ILogger<CheckoutController> _logger;
    private readonly IMediator _mediatR;

    public CheckoutController(ILogger<CheckoutController> logger, IMediator mediatR)
    {
        _logger = logger;
        _mediatR = mediatR;
    }

    [HttpPost(Name = "CheckoutBook")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutBookCommand checkoutBook)
    {
        _logger.LogInformation("Checking out a book");
        var result = await _mediatR.Send(checkoutBook);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }

    [HttpPost("GetCheckoutRecord")]
    public async Task<IActionResult> GetCheckoutRecord([FromBody] GetBooksNotReturnedQuery query)
    {
        _logger.LogInformation("Getting checkout record");
        var result = await _mediatR.Send(query);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }

    [HttpPost("ReturnBook")]
    public async Task<IActionResult> ReturnBook([FromBody] ReturnBookCommand returnBook)
    {
        _logger.LogInformation("Returning a book");
        var result = await _mediatR.Send(returnBook);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }
}
