using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Features.ShortUrls.Create;
using UrlShortener.Application.Features.ShortUrls.GetByCode;

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

        return CreatedAtAction(
            nameof(GetByCode),
            new { code = result.ShortCode },
            result);
    }

    [HttpGet("/api/shorturls/{code}")]
    public async Task<ActionResult<GetShortUrlByCodeResponse>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetShortUrlByCodeQuery(code),
            cancellationToken);

        return Ok(result);
    }
}