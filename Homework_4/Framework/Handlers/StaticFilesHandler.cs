using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace MyHttpServer;

public class StaticFilesHandler : Handler
{
    private readonly string _staticFolder;
    private readonly string _prefix;

    public StaticFilesHandler(string staticFolder, string prefix)
    {
        _staticFolder = staticFolder;
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

        // Если путь пустой или "/" — отдаём index.html
        if (string.IsNullOrEmpty(requestPath) || requestPath == "/")
        {
            requestPath = "/index.html";
        }

        string fileName = requestPath.TrimStart('/');
        string filePath = Path.Combine(_staticFolder, fileName);

        if (!File.Exists(filePath))
        {
            await CallNextAsync(context);
            return;
        }

        byte[] buffer = await File.ReadAllBytesAsync(filePath);

        context.Response.StatusCode = 200;
        context.Response.ContentType = GetContentType(filePath);
        context.Response.ContentLength64 = buffer.Length;

        await context.Response.OutputStream.WriteAsync(buffer);
        context.Response.OutputStream.Close();
    }

    private string GetContentType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".html" => "text/html; charset=utf-8",
            ".css"  => "text/css; charset=utf-8",
            ".js"   => "text/javascript; charset=utf-8",
            ".png"  => "image/png",
            ".jpg"  => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif"  => "image/gif",
            ".svg"  => "image/svg+xml",
            ".ico"  => "image/x-icon",
            _       => "application/octet-stream"
        };
    }
}