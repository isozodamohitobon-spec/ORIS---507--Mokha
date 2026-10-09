using System;

namespace MyHttpServer;

[AttributeUsage(AttributeTargets.Method)]
public class HttpPostAttribute : Attribute
{
    public string Route { get; }

    public HttpPostAttribute(string route)
    {
        Route = route;
    }
}