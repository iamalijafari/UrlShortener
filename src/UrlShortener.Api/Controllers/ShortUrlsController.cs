using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Features.ShortUrls.Create;
using UrlShortener.Application.Features.ShortUrls.Redirect;

namespace UrlShortener.Api.Controllers;

[ApiController]
[Route("api/shorturls")]
public class ShortUrlsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShortUrlsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateShortUrlCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
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