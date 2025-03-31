using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MinimalApi20_vsa.Api.Common.Exceptions;

namespace MinimalApi20_vsa.Api.Endpoints.Filters;

public class ValidationFilter<TRequest> : EndpointFilterBase
{
    private readonly IValidator<TRequest> _validator;

    public ValidationFilter(IValidator<TRequest> validator)
    {
        _validator = validator;
    }

    protected override async ValueTask<object?> OnBeforeExecutionAsync(EndpointFilterInvocationContext context)
    {
        // Try to find the request to validate
        var request = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TRequest));
        
        if (request == null)
        {
            var error = new List<string> { "Invalid request format" };
            throw new Common.Exceptions.ValidationException(error);
        }

        var validationResult = await _validator.ValidateAsync((TRequest)request);

        if (!validationResult.IsValid)
        {
            // Convert FluentValidation errors to string collection for our custom ValidationException
            var errors = validationResult.Errors
                .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                .ToList();
                
            // Throw our custom ValidationException to be handled by ErrorHandlingMiddleware
            throw new Common.Exceptions.ValidationException(errors);
        }

        return null; // Proceed with the request
    }
}