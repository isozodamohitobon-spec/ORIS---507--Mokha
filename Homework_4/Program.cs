using MyHttpServer;

class Program
{
    static async Task Main(string[] args)
    {
        const string settingsFile = "settings.json";

        if (!File.Exists(settingsFile))
        {
            Console.WriteLine("Файл настроек settings.json не найден. Сервер не запущен.");
            return;
        }

        ConfigurationManager configuration;

        try
        {
            configuration = ConfigurationManager.GetInstance(settingsFile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка в файле настроек: {ex.Message}");
            Console.WriteLine("Сервер не запущен.");
            return;
        }

        SettingsModel settings = configuration.Settings;

        string staticFolder = Path.Combine(
            Directory.GetCurrentDirectory(),
            settings.StaticPath
        );

        string indexPath = Path.Combine(staticFolder, "index.html");

        if (!File.Exists(indexPath))
        {
            Console.WriteLine($"Не найден файл {indexPath}. Сервер не запущен.");
            return;
        }

        string prefix = settings.Serverr.Path;

        // Собираем цепочку
        Handler staticHandler = new StaticFilesHandler(staticFolder, prefix);
        Handler controllersHandler = new ControllersHandler(prefix);
        Handler notFoundHandler = new NotFoundHandler();

        staticHandler
            .SetNext(controllersHandler)
            .SetNext(notFoundHandler);

        HttpServer server = new(
            settings.Serverr.Host,
            settings.Serverr.Port,
            prefix,
            staticHandler
        );

        Task serverTask = server.Start();

        Console.WriteLine("Введите stop для остановки сервера:");

        while (true)
        {
            string? command = Console.ReadLine();

            if (command == "stop")
            {
                server.Stop();
                break;
            }
        }

        await serverTask;
    }
}