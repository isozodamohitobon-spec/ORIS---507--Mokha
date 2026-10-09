using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MyHttpServer;

public class HttpServer
{
    private readonly HttpListener _server;
    private readonly string _host;
    private readonly int _port;
    private readonly string _path;

    private readonly Handler _chain;

    public HttpServer(string host, int port, string path, Handler chain)
    {
        _host = host;
        _port = port;
        _path = path.Trim('/');
        _chain = chain;

        _server = new HttpListener();
    }

    public async Task Start()
    {
        _server.Prefixes.Add($"http://{_host}:{_port}/{_path}/");
        _server.Start();

        Console.WriteLine($"Сервер запущен: http://{_host}:{_port}/{_path}/");

        while (true)
        {
            HttpListenerContext context;

            try
            {
                context = await _server.GetContextAsync();
            }
            catch (HttpListenerException) { break; }
            catch (ObjectDisposedException) { break; }

            _ = Task.Run(async () =>
            {
                try
                {
                    await _chain.HandleAsync(context);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при обработке запроса: {ex}");

                    try
                    {
                        context.Response.StatusCode = 500;
                        byte[] err = Encoding.UTF8.GetBytes("500 - Внутренняя ошибка");
                        context.Response.ContentLength64 = err.Length;
                        await context.Response.OutputStream.WriteAsync(err);
                        context.Response.OutputStream.Close();
                    }
                    catch
                    {
                        // Клиент мог уже отвалиться — игнорируем
                    }
                }
            });
        }
    }

    public void Stop()
    {
        _server.Stop();
        Console.WriteLine("Сервер остановлен");
    }
}