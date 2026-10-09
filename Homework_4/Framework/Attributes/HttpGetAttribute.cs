using System;

namespace MyHttpServer;

[AttributeUsage(AttributeTargets.Method)]
public class HttpGetAttribute : Attribute
{
    public string Route { get; }

    public HttpGetAttribute(string route)
    {
        Route = route;
    }
}