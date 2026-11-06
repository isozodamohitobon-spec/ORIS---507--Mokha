using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace LeadManager
{
    public class Server
    {
        private readonly LeadService _service;
        public Server(LeadService service) => _service = service;

        public async Task HandleRequest(HttpListenerContext context)
        {
            var req = context.Request;
            var res = context.Response;

            try
            {
                // 1. GET /leads
                if (req.HttpMethod == "GET" && req.Url!.AbsolutePath == "/leads")
                {
                    var source = req.QueryString["source"];
                    var name = req.QueryString["name"];
                    var leads = _service.GetLeads(source, name);

                    string html = "<html><body><h1>Список клиентов</h1>";
                    
                    // Форма фильтрации
                    html += @"<form method='GET' action='/leads'>
                                Источник: <input type='text' name='source'>
                                Имя: <input type='text' name='name'>
                                <button type='submit'>Фильтр</button>
                              </form><ul>";
                    
                    foreach (var l in leads)
                        html += $"<li>{l.Id}. {WebUtility.HtmlEncode(l.Name)} (Source: {l.Source}, Status: {l.Status}) <a href='/leads/details?id={l.Id}'>Подробнее</a></li>";
                    
                    html += "</ul><a href='/leads/create'>Добавить клиента</a></body></html>";
                    await SendHtml(res, 200, html);
                }
                
                // 2. GET /leads/details?id=1
                else if (req.HttpMethod == "GET" && req.Url!.AbsolutePath == "/leads/details")
                {
                    if (int.TryParse(req.QueryString["id"], out int id))
                    {
                        var lead = _service.GetLeadById(id);
                        if (lead != null)
                        {
                            string html = $"<html><body><h1>Клиент {lead.Id}</h1>" +
                                          $"<p>Имя: {WebUtility.HtmlEncode(lead.Name)}</p>" +
                                          $"<p>Источник: {lead.Source}</p>" +
                                          $"<p>Статус: {lead.Status}</p>" +
                                          $"<a href='/leads'>Назад</a></body></html>";
                            await SendHtml(res, 200, html);
                        }
                        else await SendHtml(res, 404, "<h1>404 Not Found</h1>");
                    }
                    else await SendHtml(res, 400, "<h1>400 Bad Request</h1>");
                }

                // 3. GET /leads/create (Отображение формы)
                else if (req.HttpMethod == "GET" && req.Url!.AbsolutePath == "/leads/create")
                {
                    string html = @"<html><body><h1>Новый клиент</h1>
                        <form method='POST' action='/leads/create'>
                            Имя: <input type='text' name='name' required>

                            Источник: 
                            <select name='source'>
                                <option value='website'>website</option>
                                <option value='social'>social</option>
                                <option value='recommendation'>recommendation</option>
                            </select>

                            <button type='submit'>Создать</button>
                        </form></body></html>";
                    await SendHtml(res, 200, html);
                }

                // 3. POST /leads/create (Создание)
                else if (req.HttpMethod == "POST" && req.Url!.AbsolutePath == "/leads/create")
                {
                    var form = await ParseFormAsync(req);
                    if (form.TryGetValue("name", out var name) && form.TryGetValue("source", out var srcStr) && Enum.TryParse<LeadSource>(srcStr, out var source))
                    {
                        _service.CreateLead(name, source);
                        // Успешный POST завершаем редиректом 303 See Other
                        res.StatusCode = 303;
                        res.RedirectLocation = "/leads";
                    }
                    else await SendHtml(res, 400, "<h1>400 Bad Request: Неверные данные формы</h1>");
                }

                // 4. PUT /leads/update?id=1
                else if (req.HttpMethod == "PUT" && req.Url!.AbsolutePath == "/leads/update")
                {
                    if (!int.TryParse(req.QueryString["id"], out int id))
                    {
                        await SendHtml(res, 400, "<h1>400 Bad Request: Нет ID</h1>");
                        return;
                    }

                    var form = await ParseFormAsync(req);
                    // Для PUT передаем полный набор изменяемых полей (name, source, status)
                    if (form.TryGetValue("name", out var name) && 
                        form.TryGetValue("source", out var srcStr) && Enum.TryParse<LeadSource>(srcStr, out var source) &&
                        form.TryGetValue("status", out var statStr) && Enum.TryParse<LeadStatus>(statStr, out var status))
                    {
                        if (_service.UpdateLead(id, name, source, status))
                        {
                            // Успешный PUT — ответ 204 No Content
                            res.StatusCode = 204;
                        }
                        else await SendHtml(res, 404, "<h1>404 Not Found</h1>");
                    }
                    else await SendHtml(res, 400, "<h1>400 Bad Request: Неверные данные</h1>");
                }
                else
                {
                    await SendHtml(res, 404, "<h1>404 Not Found</h1>");
                }
            }
            catch (ArgumentException ex)
            {
                // Ошибки валидации из сервиса
                await SendHtml(res, 400, $"<h1>400 Bad Request</h1><p>{WebUtility.HtmlEncode(ex.Message)}</p>");
            }
            catch (Exception)
            {
                await SendHtml(res, 500, "<h1>500 Internal Server Error</h1>");
            }
            finally
            {
                res.OutputStream.Close();
            }
        }

        // Хелпер для отправки HTML
        private async Task SendHtml(HttpListenerResponse res, int statusCode, string html)
        {
            res.StatusCode = statusCode;
            res.ContentType = "text/html; charset=utf-8";
            byte[] buffer = Encoding.UTF8.GetBytes(html);
            res.ContentLength64 = buffer.Length;
            await res.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        // Хелпер для парсинга application/x-www-form-urlencoded
        private async Task<Dictionary<string, string>> ParseFormAsync(HttpListenerRequest req)
        {
            var dict = new Dictionary<string, string>();
            if (!req.HasEntityBody) return dict;

            using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
            var body = await reader.ReadToEndAsync();
            //Парсинг строки
            foreach (var pair in body.Split('&', StringSplitOptions.RemoveEmptyEntries))
            {
                var parts = pair.Split('=', 2);
                if (parts.Length == 2)
                {
                    dict[WebUtility.UrlDecode(parts[0])] = WebUtility.UrlDecode(parts[1]);
                }
            }
            return dict;
        }
    }
}