using MediatR;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Application.Features.ShortUrls.Create;
using UrlShortener.Application.Features.ShortUrls.Disable;
using UrlShortener.Application.Features.ShortUrls.GetByCode;

namespace UrlShortener.Api.Controllers;

/// <summary>
/// Manages URL shortening operations.
/// </summary>
[ApiController]
[Route("api/shorturls")]
public class ShortUrlsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ShortUrlsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new shortened URL.
    /// </summary>
    /// <param name="command">The URL information used to create a shortened URL.</param>
    /// <returns>The created short URL details.</returns>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/shorturls
    ///     {
    ///         "originalUrl": "https://github.com"
    ///     }
    ///
    /// </remarks>
    /// <response code="201">The shortened URL was created successfully.</response>
    /// <response code="400">The request is invalid.</response>
    [HttpPost]
    [ProducesResponseType(typeof(CreateShortUrlResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create(CreateShortUrlCommand command)
    {
        var result = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetByCode),
            new { code = result.ShortCode },
            result);
    }

    /// <summary>
    /// Retrieves information about a shortened URL.
    /// </summary>
    /// <param name="code">The unique short code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The shortened URL information.</returns>
    /// <response code="200">The shortened URL was found.</response>
    /// <response code="404">The specified short code was not found.</response>
    [HttpGet("{code}")]
    [ProducesResponseType(typeof(GetShortUrlByCodeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetShortUrlByCodeResponse>> GetByCode(
        string code,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new GetShortUrlByCodeCommand(code),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Disable a shortened URL.
    /// </summary>
    /// <param name="code">The unique short code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="204">The shortened URL was disabled.</response>
    /// <response code="404">The specified short code was not found.</response>
    [HttpPatch("{code}/disable")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DisableByCode(
        string code,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(
            new DisableShortUrlCommand(code),
            cancellationToken);

        return NoContent();
    }
}