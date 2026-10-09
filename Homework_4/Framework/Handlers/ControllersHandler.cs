using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer;

public class ControllersHandler : Handler
{
    private readonly string _prefix;

    public ControllersHandler(string prefix)
    {
        _prefix = "/" + prefix.Trim('/');
    }

    public override async Task HandleAsync(HttpListenerContext context)
    {
        string requestPath = context.Request.Url?.AbsolutePath ?? "/";

        // Отрезаем префикс (например, "/connection")
        if (requestPath.StartsWith(_prefix))
        {
            requestPath = requestPath[_prefix.Length..];
        }

        string[] segments = requestPath.Trim('/').Split('/');

        if (segments.Length < 2)
        {
            await CallNextAsync(context);
            return;
        }

        string controllerName = segments[0];
        string methodRoute = segments[1];

        Type? controllerType = FindControllerType(controllerName);

        if (controllerType is null)
        {
            await CallNextAsync(context);
            return;
        }

        string httpMethod = context.Request.HttpMethod;
        MethodInfo? method = FindMethod(controllerType, httpMethod, methodRoute);

        if (method is null)
        {
            await CallNextAsync(context);
            return;
        }

        Dictionary<string, string> requestData = await CollectRequestDataAsync(context);

        ParameterInfo[] parameters = method.GetParameters();
        object?[] args = new object?[parameters.Length];

        for (int i = 0; i < parameters.Length; i++)
        {
            string paramName = parameters[i].Name ?? "";

            if (requestData.TryGetValue(paramName, out string? value))
            {
                args[i] = Convert.ChangeType(value, parameters[i].ParameterType);
            }
            else
            {
                args[i] = parameters[i].ParameterType.IsValueType
                    ? Activator.CreateInstance(parameters[i].ParameterType)
                    : null;
            }
        }

        object controllerInstance = Activator.CreateInstance(controllerType)!;
        object? result = method.Invoke(controllerInstance, args);

        if (result is Task task)
        {
            await task;
        }

        context.Response.StatusCode = 200;
        context.Response.ContentType = "text/plain; charset=utf-8";

        byte[] ok = Encoding.UTF8.GetBytes("OK");
        context.Response.ContentLength64 = ok.Length;
        await context.Response.OutputStream.WriteAsync(ok);
        context.Response.OutputStream.Close();
    }

    private Type? FindControllerType(string name)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            ControllerAttribute? attr = type.GetCustomAttribute<ControllerAttribute>();

            if (attr is not null &&
                string.Equals(attr.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return type;
            }
        }

        return null;
    }

    private MethodInfo? FindMethod(Type controllerType, string httpMethod, string route)
    {
        foreach (MethodInfo method in controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance))
        {
            if (httpMethod == "GET")
            {
                HttpGetAttribute? attr = method.GetCustomAttribute<HttpGetAttribute>();

                if (attr is not null &&
                    string.Equals(attr.Route, route, StringComparison.OrdinalIgnoreCase))
                {
                    return method;
                }
            }
            else if (httpMethod == "POST")
            {
                HttpPostAttribute? attr = method.GetCustomAttribute<HttpPostAttribute>();if (attr is not null &&
                    string.Equals(attr.Route, route, StringComparison.OrdinalIgnoreCase))
                {
                    return method;
                }
            }
        }

        return null;
    }

    private async Task<Dictionary<string, string>> CollectRequestDataAsync(HttpListenerContext context)
    {
        Dictionary<string, string> data = new();

        string? query = context.Request.Url?.Query;

        if (!string.IsNullOrEmpty(query))
        {
            ParseKeyValues(query.TrimStart('?'), data);
        }

        if (context.Request.HasEntityBody)
        {
            using StreamReader reader = new(context.Request.InputStream, context.Request.ContentEncoding);
            string body = await reader.ReadToEndAsync();

            if (!string.IsNullOrEmpty(body))
            {
                ParseKeyValues(body, data);
            }
        }

        return data;
    }

    private void ParseKeyValues(string source, Dictionary<string, string> target)
    {
        string[] pairs = source.Split('&', StringSplitOptions.RemoveEmptyEntries);

        foreach (string pair in pairs)
        {
            int eq = pair.IndexOf('=');

            if (eq < 0)
            {
                continue;
            }

            string key = Uri.UnescapeDataString(pair[..eq].Replace('+', ' '));
            string value = Uri.UnescapeDataString(pair[(eq + 1)..].Replace('+', ' '));

            target[key] = value;
        }
    }
}