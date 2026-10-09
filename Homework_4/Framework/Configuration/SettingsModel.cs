namespace MyHttpServer;

public class SettingsModel
{
    public Server Serverr { get; set; } = new();

    // Путь к папке со статикой (по умолчанию — "static")
    public string StaticPath { get; set; } = "static";

    public class Server
    {
        public string Host { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 8888;

        public string Path { get; set; } = "connection/";
    }
}