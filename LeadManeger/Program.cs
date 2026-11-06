//Вариант 9 : учет потенциальных клиентов
using System;
using System.Net;
using System.Threading.Tasks;

namespace LeadManager
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var service = new LeadService();
            
            var server = new Server(service);

            HttpListener listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:8080/");
            listener.Start();
            
            Console.WriteLine("Сервер запущен на http://localhost:8080/");
            Console.WriteLine("Нажмите Ctrl+C для остановки.");

            // Бесконечный цикл приема запросов
            while (true)
            {
                var context = await listener.GetContextAsync();
                _ = Task.Run(() => server.HandleRequest(context));
            }
        }
    }
}