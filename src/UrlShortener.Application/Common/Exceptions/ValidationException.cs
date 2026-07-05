using FluentValidation.Results;

namespace UrlShortener.Application.Common.Exceptions;

public sealed class ValidationException : Exception
{
    public IReadOnlyCollection<ValidationFailure> Errors { get; }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : base("Validation failed.")
    {
        Errors = failures.ToList().AsReadOnly();
    }

    public ValidationException(string message)
        : base(message)
    {
        Errors = [];
    }
}