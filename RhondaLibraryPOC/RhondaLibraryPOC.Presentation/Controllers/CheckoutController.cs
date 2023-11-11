using MediatR;
using Microsoft.AspNetCore.Mvc;
using RhondaLibraryPOC.Application.CQRS.Checkouts;

namespace RhondaLibraryPOC.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class CheckoutController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(IMediator mediator, ILogger<CheckoutController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CheckoutDTO>>> Get()
    {
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CheckoutDTO>> Get(Guid id)
    {
        return Ok();
    }

    [HttpPost]
    public async Task<ActionResult<CheckoutDTO>> Post(CheckoutDTO checkout)
    {
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<CheckoutDTO>> Put(Guid id, CheckoutDTO checkout)
    {
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<CheckoutDTO>> Delete(Guid id)
    {
        return Ok();
    }   
}
