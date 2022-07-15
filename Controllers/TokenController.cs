using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApiIdentity.Application.Token.Commands.SignIn;
using WebApiIdentity.Application.Token.Commands.SignOut;

namespace WebApiIdentity.Controllers;

[ApiController]
[Route("oauth/tokens")]
public class TokenController : ControllerBase
{
    private readonly IMediator _mediator;

    public TokenController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost]
    public async Task<IActionResult> SignInAsync(SignInCommand command)
    {
        var value = await _mediator.Send(command);
        return Ok(value);
    }

    [HttpDelete]
    public async Task<IActionResult> SignOutAsync([FromQuery]SignOutCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> GetListAsync()
    {
        await Task.Delay(0);
        return Ok();
    }
}