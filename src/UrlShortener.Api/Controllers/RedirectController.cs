using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Features.ShortUrls.Redirect;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("/")]
public sealed class RedirectController : ControllerBase
{
    private readonly IMediator _mediator;

    public RedirectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{code}")]
    public async Task<IActionResult> Redirect(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new RedirectShortUrlCommand(code), cancellationToken);

        if (!result.IsSuccess)
            return NotFound(result.Error);

        return Redirect(result.Value!);
    }
}