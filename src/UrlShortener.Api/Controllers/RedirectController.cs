using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Features.ShortUrls.Redirect;

namespace UrlShortener.Api.Controllers;

/// <summary>
/// Redirects incoming short URLs to their original destination.
/// </summary>
[ApiController]
[Route("/")]
public sealed class RedirectController : ControllerBase
{
    private readonly IMediator _mediator;

    public RedirectController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Redirects to the original URL associated with the specified short code.
    /// </summary>
    /// <param name="code">The unique short code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A redirect response if the short code exists.</returns>
    /// <response code="302">The client is redirected to the original URL.</response>
    /// <response code="404">The specified short code does not exist or is unavailable.</response>
    [HttpGet("{code}")]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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