using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace MyHttpServer;

[Controller("home-work")]
public class HomeworkController
{
    [HttpGet("send")]
    public void Send(string to)
    {
        if (string.IsNullOrEmpty(to))
        {
            Console.WriteLine("Не передан параметр to");
            return;
        }

        // === ЗАМЕНИ НА СВОИ ДАННЫЕ ===
        string group = "11-507";
        string fullName = "Исозода Мохитобон";
        // =============================

        string zipPath = Path.Combine(Directory.GetCurrentDirectory(), "project.zip");

        if (!File.Exists(zipPath))
        {
            Console.WriteLine($"Файл {zipPath} не найден");
            return;
        }

        try
        {
            MailAddress from = new MailAddress("isozodamohitobon@mail.ru", "Homework Bot");
            MailAddress recipient = new MailAddress(to);

            MailMessage mail = new MailMessage(from, recipient);
            mail.Subject = $"Домашнее задание — {group} — {fullName}";
            mail.Body = $"Группа: {group}\nФИО: {fullName}\n\nВо вложении архив с проектом.";
            mail.Attachments.Add(new Attachment(zipPath));

            SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
            smtp.Credentials = new NetworkCredential("isozodamohitobon@mail.ru", "LxpIo0oplazpldcbWncL");
            smtp.EnableSsl = true;

            smtp.Send(mail);

            Console.WriteLine($"Письмо с архивом отправлено на {to}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка отправки: {ex.Message}");
            Console.WriteLine($"Внутренняя ошибка: {ex.InnerException?.Message}");
        }
    }
}