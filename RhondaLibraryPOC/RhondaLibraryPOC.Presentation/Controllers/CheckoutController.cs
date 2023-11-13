using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhondaLibraryPOC.Application.CQRS.Checkouts.Commands;
using RhondaLibraryPOC.Presentation.Common;
using System.Net;

namespace RhondaLibraryPOC.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class CheckoutController : ControllerBase
{
    private readonly ILogger<CheckoutController> _logger;
    private readonly IMediator _mediatR;

    public CheckoutController(ILogger<CheckoutController> logger, IMediator mediatR)
    {
        _logger = logger;
        _mediatR = mediatR;
    }

    [HttpPost(Name = "Checkout")]
    public async Task<IActionResult> CheckoutBook([FromBody] CheckoutBooksCommand command)
    {
        _logger.LogInformation("Checking out a book");
        var result = await _mediatR.Send(command);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }
}
