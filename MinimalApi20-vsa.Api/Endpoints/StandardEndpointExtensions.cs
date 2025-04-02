using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using MinimalApi20_vsa.Api.Common.Models;
using MinimalApi20_vsa.Api.Endpoints.Filters;

namespace MinimalApi20_vsa.Api.Endpoints;

public static class StandardEndpointExtensions
{
    /// <summary>
    /// Maps a POST endpoint with standardized response documentation.
    /// </summary>
    /// <typeparam name="TRequest">The request body type</typeparam>
    /// <typeparam name="TResponse">The response body type for successful requests</typeparam>
    /// <param name="builder">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The endpoint handler</param>
    /// <param name="configureEndpoint">Optional additional endpoint configuration</param>
    /// <returns>A RouteHandlerBuilder that can be used to further customize the endpoint</returns>
    public static RouteHandlerBuilder MapStandardPost<TRequest, TResponse>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        var routeHandler = builder.MapPost(pattern, handler)
            .Produces<TResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        configureEndpoint?.Invoke(routeHandler);
        
        return routeHandler;
    }

    /// <summary>
    /// Maps a GET endpoint with standardized response documentation.
    /// </summary>
    /// <typeparam name="TResponse">The response body type for successful requests</typeparam>
    /// <param name="builder">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The endpoint handler</param>
    /// <param name="configureEndpoint">Optional additional endpoint configuration</param>
    /// <returns>A RouteHandlerBuilder that can be used to further customize the endpoint</returns>
    public static RouteHandlerBuilder MapStandardGet<TResponse>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        var routeHandler = builder.MapGet(pattern, handler)
            .Produces<TResponse>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        configureEndpoint?.Invoke(routeHandler);
        
        return routeHandler;
    }

    /// <summary>
    /// Maps a POST endpoint with standardized response documentation and automatic validation.
    /// </summary>
    /// <typeparam name="TRequest">The request body type</typeparam>
    /// <typeparam name="TResponse">The response body type for successful requests</typeparam>
    /// <param name="builder">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The endpoint handler</param>
    /// <param name="configureEndpoint">Optional additional endpoint configuration</param>
    /// <returns>A RouteHandlerBuilder that can be used to further customize the endpoint</returns>
    public static RouteHandlerBuilder MapStandardPostWithValidation<TRequest, TResponse>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        return builder.MapStandardPost<TRequest, TResponse>(pattern, handler, 
            builder => {
                builder.AddEndpointFilter<ValidationFilter<TRequest>>();
                configureEndpoint?.Invoke(builder);
            });
    }

    /// <summary>
    /// Maps a POST endpoint with standardized response documentation, validation, and HATEOAS support.
    /// </summary>
    /// <typeparam name="TRequest">The request body type</typeparam>
    /// <typeparam name="TResponse">The response body type for successful requests</typeparam>
    /// <param name="builder">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The endpoint handler</param>
    /// <param name="configureEndpoint">Optional additional endpoint configuration</param>
    /// <returns>A RouteHandlerBuilder that can be used to further customize the endpoint</returns>
    public static RouteHandlerBuilder MapStandardPostWithHateoas<TRequest, TResponse>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        return builder.MapStandardPost<TRequest, HateoasResponse<TResponse>>(pattern, handler, 
            builder => {
                builder.AddEndpointFilter<ValidationFilter<TRequest>>();
                configureEndpoint?.Invoke(builder);
            });
    }

    /// <summary>
    /// Maps a GET endpoint with standardized response documentation and HATEOAS support.
    /// </summary>
    /// <typeparam name="TResponse">The response body type for successful requests</typeparam>
    /// <param name="builder">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The endpoint handler</param>
    /// <param name="configureEndpoint">Optional additional endpoint configuration</param>
    /// <returns>A RouteHandlerBuilder that can be used to further customize the endpoint</returns>
    public static RouteHandlerBuilder MapStandardGetWithHateoas<TResponse>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        return builder.MapStandardGet<HateoasResponse<TResponse>>(pattern, handler, configureEndpoint);
    }

    /// <summary>
    /// Maps a PUT endpoint with standardized response documentation.
    /// </summary>
    /// <typeparam name="TRequest">The request body type</typeparam>
    /// <param name="builder">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The endpoint handler</param>
    /// <param name="configureEndpoint">Optional additional endpoint configuration</param>
    /// <returns>A RouteHandlerBuilder that can be used to further customize the endpoint</returns>
    public static RouteHandlerBuilder MapStandardPut<TRequest>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        var routeHandler = builder.MapPut(pattern, handler)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        configureEndpoint?.Invoke(routeHandler);
        
        return routeHandler;
    }

    /// <summary>
    /// Maps a PUT endpoint with standardized response documentation and automatic validation.
    /// </summary>
    /// <typeparam name="TRequest">The request body type</typeparam>
    /// <param name="builder">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The endpoint handler</param>
    /// <param name="configureEndpoint">Optional additional endpoint configuration</param>
    /// <returns>A RouteHandlerBuilder that can be used to further customize the endpoint</returns>
    public static RouteHandlerBuilder MapStandardPutWithValidation<TRequest>(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        return builder.MapStandardPut<TRequest>(pattern, handler, 
            builder => {
                builder.AddEndpointFilter<ValidationFilter<TRequest>>();
                configureEndpoint?.Invoke(builder);
            });
    }

    /// <summary>
    /// Maps a DELETE endpoint with standardized response documentation.
    /// </summary>
    /// <param name="builder">The endpoint route builder</param>
    /// <param name="pattern">The route pattern</param>
    /// <param name="handler">The endpoint handler</param>
    /// <param name="configureEndpoint">Optional additional endpoint configuration</param>
    /// <returns>A RouteHandlerBuilder that can be used to further customize the endpoint</returns>
    public static RouteHandlerBuilder MapStandardDelete(
        this IEndpointRouteBuilder builder,
        string pattern,
        Delegate handler,
        Action<RouteHandlerBuilder>? configureEndpoint = null)
    {
        var routeHandler = builder.MapDelete(pattern, handler)
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        configureEndpoint?.Invoke(routeHandler);
        
        return routeHandler;
    }
}
