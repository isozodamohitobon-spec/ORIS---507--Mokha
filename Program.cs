using MyHttpServer;
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        // Читаем настройки из settings.json
        string settingsText =
            File.ReadAllText("settings.json");

        Settings settings =
            JsonSerializer.Deserialize<Settings>(settingsText)!;

        // Получаем единственный экземпляр сервера
        HttpServer httpServer =
            HttpServer.GetInstance(
                settings.Serverr.Host,
                settings.Serverr.Port,
                settings.Serverr.Path
            );

        // Запускаем сервер
        Task serverTask = httpServer.Start();

        Console.WriteLine("Введите stop для остановки сервера:");

        // Ждём команду пользователя
        while (true)
        {
            string? command = Console.ReadLine();

            if (command == "stop")
            {
                httpServer.Stop();
                break;
            }
        }

        await serverTask;
    }
}