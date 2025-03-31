using System;

namespace MinimalApi20_vsa.Api.Common.Exceptions;

public abstract class AppException : Exception
{
    public abstract int StatusCode { get; }

    protected AppException(string message) : base(message)
    {
    }
}

public class ValidationException : AppException
{
    public override int StatusCode => 400;
    public IEnumerable<string> Errors { get; }

    public ValidationException(IEnumerable<string> errors) : base("Validation failed")
    {
        Errors = errors;
    }
}

public class NotFoundException : AppException
{
    public override int StatusCode => 404;

    public NotFoundException(string message) : base(message)
    {
    }
}
