using System;
using System.IO;
using System.Text.Json;

namespace MyHttpServer;

public class ConfigurationManager
{
    private static ConfigurationManager? _instance;
    private static readonly object _lock = new();

    public SettingsModel Settings { get; }

    private ConfigurationManager(string filePath)
    {
        string json = File.ReadAllText(filePath);

        Settings = JsonSerializer.Deserialize<SettingsModel>(json)
                   ?? throw new Exception("Не удалось загрузить настройки");
    }

    public static ConfigurationManager GetInstance(string filePath = "settings.json")
    {
        if (_instance is null)
        {
            lock (_lock)
            {
                if (_instance is null)
                {
                    _instance = new ConfigurationManager(filePath);
                }
            }
        }

        return _instance;
    }
}