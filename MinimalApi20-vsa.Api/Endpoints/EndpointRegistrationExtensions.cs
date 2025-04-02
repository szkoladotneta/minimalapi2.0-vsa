using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalApi20_vsa.Api.Endpoints;

public static class EndpointRegistrationExtensions
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var endpointTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IEndpoint).IsAssignableFrom(t));
            
        foreach (var type in endpointTypes)
        {
            services.AddTransient(typeof(IEndpoint), type);
        }
        
        var groupTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IEndpointGroup).IsAssignableFrom(t));
            
        foreach (var type in groupTypes)
        {
            services.AddTransient(typeof(IEndpointGroup), type);
        }
        
        return services;
    }
    
    public static WebApplication MapEndpoints(this WebApplication app)
    {
        var endpointGroups = app.Services.GetServices<IEndpointGroup>()
            .ToDictionary(g => g.GroupName);
            
        var endpoints = app.Services.GetServices<IEndpoint>().ToList();
        
        var groupedEndpoints = new Dictionary<string, List<IEndpoint>>();
        var ungroupedEndpoints = new List<IEndpoint>();
        
        foreach (var endpoint in endpoints)
        {
            var attribute = endpoint.GetType().GetCustomAttribute<EndpointGroupAttribute>();
            
            if (attribute != null)
            {
                if (!groupedEndpoints.ContainsKey(attribute.GroupName))
                {
                    groupedEndpoints[attribute.GroupName] = new List<IEndpoint>();
                }
                
                groupedEndpoints[attribute.GroupName].Add(endpoint);
            }
            else
            {
                ungroupedEndpoints.Add(endpoint);
            }
        }
        
        foreach (var group in groupedEndpoints)
        {
            string groupName = group.Key;
            List<IEndpoint> groupEndpoints = group.Value;
            
            string groupRoute = $"api/{groupName.ToLowerInvariant()}";
            var routeGroupBuilder = app.MapGroup(groupRoute);
            
            if (endpointGroups.TryGetValue(groupName, out var endpointGroup))
            {
                endpointGroup.ConfigureGroup(routeGroupBuilder);
            }
            
            foreach (var endpoint in groupEndpoints)
            {
                endpoint.MapEndpoint(routeGroupBuilder);
            }
        }
        
        foreach (var endpoint in ungroupedEndpoints)
        {
            endpoint.MapEndpoint(app);
        }
        
        return app;
    }
}