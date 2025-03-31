using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MinimalApi20_vsa.Api.Endpoints.Filters;

public abstract class EndpointFilterBase : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Pre-processing
        var result = await OnBeforeExecutionAsync(context);
        if (result != null)
        {
            return result;
        }

        // Execute the endpoint
        var response = await next(context);

        // Post-processing
        return await OnAfterExecutionAsync(context, response);
    }

    protected virtual ValueTask<object?> OnBeforeExecutionAsync(EndpointFilterInvocationContext context)
    {
        return ValueTask.FromResult<object?>(null);
    }

    protected virtual ValueTask<object?> OnAfterExecutionAsync(EndpointFilterInvocationContext context, object? response)
    {
        return ValueTask.FromResult(response);
    }
}