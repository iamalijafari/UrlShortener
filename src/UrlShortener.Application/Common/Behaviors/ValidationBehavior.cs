using FluentValidation;
using MediatR;
using ApplicationValidationException = UrlShortener.Application.Common.Exceptions.ValidationException;

namespace UrlShortener.Application.Common.Behaviors;

public sealed class ValidationBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = await Task.WhenAll(
            _validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        var errors = failures
            .SelectMany(f => f.Errors)
            .Where(e => e is not null)
            .ToList();

        if (errors.Count != 0)
        {
            throw new ApplicationValidationException(errors);
        }

        return await next();
    }
}