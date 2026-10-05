using System.Net;
using System.Text;

namespace MyHttpServer;

public class  HttpServer
{
    // Singleton — создаём только один экземпляр сервера
    private static HttpServer? _instance;

    private readonly string _host;
    private readonly int _port;
    private readonly string _path;

    private readonly HttpListener _server;

    // Приватный конструктор
    private HttpServer(string host, int port, string path)
    {
        _host = host;
        _port = port;
        _path = path;

        _server = new HttpListener();
    }

    // Получаем единственный экземпляр сервера
    public static HttpServer GetInstance(
        string host,
        int port,
        string path)
    {
        if (_instance == null)
        {
            _instance = new HttpServer(host, port, path);
        }

        return _instance;
    }

    // Запуск сервера
    public async Task Start()
    {
        _server.Prefixes.Add(
            $"http://{_host}:{_port}/{_path}"
        );

        _server.Start();

        Console.WriteLine(
            $"Сервер запущен: http://{_host}:{_port}/{_path}"
        );

        // Постоянно ждём новые запросы
        while (true)
        {
            try
            {
                HttpListenerContext context =
                    await _server.GetContextAsync();

                HandleRequest(context);
            }
            catch (HttpListenerException)
            {
                break;
            }
            catch (ObjectDisposedException)
            {
                break;
            }
        }
    }

    // Обработка запроса
    private void HandleRequest(HttpListenerContext context)
    {
        string requestPath = context.Request.Url?.AbsolutePath ?? "/";

        // Убираем /connection/ из адреса
        string relativePath = requestPath
            .Replace($"/{_path}", "")
            .TrimStart('/');

        // Если открыли главную страницу
        if (string.IsNullOrEmpty(relativePath))
        {
            relativePath = "index.html";
        }

        // Путь к файлу
        string filePath = Path.Combine(
            Directory.GetCurrentDirectory(),
            relativePath
        );

        // Если файл существует — отправляем его
        if (File.Exists(filePath))
        {
            byte[] buffer = File.ReadAllBytes(filePath);

            HttpListenerResponse response = context.Response;

            response.ContentType = GetContentType(filePath);
            response.ContentLength64 = buffer.Length;

            using Stream output = response.OutputStream;

            output.Write(buffer);
            output.Flush();

            Console.WriteLine(
                $"Запрос обработан: {relativePath}"
            );

            return;
        }

        // Если файл не найден
        context.Response.StatusCode = 404;

        byte[] errorBuffer =
            Encoding.UTF8.GetBytes("404 - Файл не найден");

        context.Response.ContentLength64 =
            errorBuffer.Length;

        using Stream errorOutput =
            context.Response.OutputStream;

        errorOutput.Write(errorBuffer);
        errorOutput.Flush();
    }

    // Определяем тип файла
    private string GetContentType(string filePath)
    {
        string extension =
            Path.GetExtension(filePath).ToLower();

        return extension switch
        {
            ".html" => "text/html; charset=utf-8",
            ".css" => "text/css; charset=utf-8",
            ".js" => "text/javascript; charset=utf-8",
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".gif" => "image/gif",
            ".svg" => "image/svg+xml",
            ".ico" => "image/x-icon",
            _ => "application/octet-stream"
        };
    }

    // Остановка сервера
    public void Stop()
    {
        _server.Stop();

        Console.WriteLine("Сервер остановлен");
    }
}