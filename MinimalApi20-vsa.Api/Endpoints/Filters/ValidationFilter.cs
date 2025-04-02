using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using MinimalApi20_vsa.Api.Common.Exceptions;

namespace MinimalApi20_vsa.Api.Endpoints.Filters;

public class ValidationFilter<TRequest>(IValidator<TRequest> validator) : EndpointFilterBase
{
    protected override async ValueTask<object?> OnBeforeExecutionAsync(EndpointFilterInvocationContext context)
    {
        var request = context.Arguments.FirstOrDefault(a => a?.GetType() == typeof(TRequest));
        
        if (request == null)
        {
            var error = new List<string> { "Invalid request format" };
            throw new Common.Exceptions.ValidationException(error);
        }

        var validationResult = await validator.ValidateAsync((TRequest)request);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(e => $"{e.PropertyName}: {e.ErrorMessage}")
                .ToList();
                
            throw new Common.Exceptions.ValidationException(errors);
        }

        return null;
    }
}