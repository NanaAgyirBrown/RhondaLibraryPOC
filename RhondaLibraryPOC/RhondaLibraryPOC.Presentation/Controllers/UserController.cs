using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RhondaLibraryPOC.Application.Checkouts.Commands;
using RhondaLibraryPOC.Application.CQRS.Users.Queries;
using RhondaLibraryPOC.Presentation.Common;
using System.Net;

namespace RhondaLibraryPOC.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : ControllerBase    
{
    private readonly ILogger<UserController> _logger;
    private readonly IMediator _mediatR;

    public UserController(ILogger<UserController> logger, IMediator mediatR)
    {
        _logger = logger;
        _mediatR = mediatR;
    }

    [HttpPost(Name = "RegisterUser")]
    public async Task<IActionResult> AddUser([FromBody]AddUserCommand command)
    {
        _logger.LogInformation("Adding a new user");
        var result = await _mediatR.Send(command);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }

    [HttpGet("{Id}", Name = "GetUserById")]
    public async Task<IActionResult> GetUserById([FromRoute] GetUserDetailsQuery query)
    {
        _logger.LogInformation("Getting user by id");
        var result = await _mediatR.Send(query);
        return ActionHandler.HandleActionResult(result, HttpStatusCode.NotFound);
    }
}
