namespace MinimalApi20_vsa.Api.Endpoints;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class EndpointGroupAttribute : Attribute
{
    public string GroupName { get; }
    
    public EndpointGroupAttribute(string groupName)
    {
        GroupName = groupName;
    }
}