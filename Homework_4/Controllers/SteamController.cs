using System;
using System.Net;
using System.Net.Mail;

namespace MyHttpServer;

[Controller("steam")]
public class SteamController
{
    [HttpGet("send")]
    public void SendMessage(string to, string hero)
    {
        if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(hero))
        {
            Console.WriteLine("Не переданы параметры to или hero");
            return;
        }

        // Получаем список предметов под героя
        string[] items = GetItemsForHero(hero);

        // Формируем тело письма
        string body = $"Герой: {hero}\n";
        body += "Вещь на площадке:\n";

        for (int i = 0; i < items.Length; i++)
        {
            body += $"{i + 1}. {items[i]}\n";
        }

        // Отправляем письмо
        try
        {
            MailAddress from = new MailAddress("isozodamohitobon@mail.ru", "Dota 2 Bot");
            MailAddress recipient = new MailAddress(to);

            MailMessage mail = new MailMessage(from, recipient);
            mail.Subject = $"Dota 2 — {hero}";
            mail.Body = body;

            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 465);
            smtp.Credentials = new NetworkCredential("isozodamohitobon@mail.ru", "LxpIo0oplazpldcbWncL");
            smtp.EnableSsl = true;

            smtp.Send(mail);

            Console.WriteLine($"Письмо отправлено на {to}, герой: {hero}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки: {ex.Message}");
            Console.WriteLine($"Внутренняя ошибка: {ex.InnerException?.Message}");
 
        }
    }

    // Предметы под каждого героя
    private string[] GetItemsForHero(string hero)
    {
        return hero.ToUpper() switch
        {
            "ABBADON" => new[] { "Blink Dagger", "Black King Bar", "Aghanim's Scepter", "Guardian Greaves" },
            "PUDGE"   => new[] { "Blink Dagger", "Aghanim's Shard", "Force Staff", "Black King Bar" },
            "INVOKER" => new[] { "Aghanim's Scepter", "Octarine Core", "Blink Dagger", "Black King Bar" },
            "SF"      => new[] { "Manta Style", "Satanic", "Black King Bar", "Sange and Yasha" },
            _         => new[] { "Blink Dagger", "Black King Bar" }  // если герой не из списка
        };
    }
}