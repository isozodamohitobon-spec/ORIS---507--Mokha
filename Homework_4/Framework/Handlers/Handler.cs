using System.Net;
using System.Threading.Tasks;

namespace MyHttpServer;

public abstract class Handler
{
    private Handler? _next;

    public Handler SetNext(Handler next)
    {
        _next = next;
        return next;
    }

    public abstract Task HandleAsync(HttpListenerContext context);

    protected Task CallNextAsync(HttpListenerContext context)
    {
        if (_next is null)
        {
            return Task.CompletedTask;
        }

        return _next.HandleAsync(context);
    }
}