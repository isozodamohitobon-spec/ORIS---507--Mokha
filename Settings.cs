namespace MyHttpServer;

public class Settings
{
    public Server Serverr { get; set; }

    public class Server
    {
        public string Host { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 9999;

        public string Path { get; set; } = "/";
    }
}