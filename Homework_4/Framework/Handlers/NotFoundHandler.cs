using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer;

public class NotFoundHandler : Handler
{
    public override async Task HandleAsync(HttpListenerContext context)
    {
        context.Response.StatusCode = 404;
        context.Response.ContentType = "text/plain; charset=utf-8";

        byte[] body = Encoding.UTF8.GetBytes("404 - Не найдено");
        context.Response.ContentLength64 = body.Length;

        await context.Response.OutputStream.WriteAsync(body);
        context.Response.OutputStream.Close();
    }
}