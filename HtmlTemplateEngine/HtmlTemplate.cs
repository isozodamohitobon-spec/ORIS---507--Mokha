using System.Reflection;
using System.Text.RegularExpressions;

namespace Workers;

public class HtmlTemplate : IHtmlTemplateRenderer
{
    
    public string RenderFromString(string htmlTemplate, object dataModel)
{
    // 1. Регулярное выражение: ищет всё, что находится между ${ и }
    string pattern = @"\$\{([^}]+)\}";

    // 2. Regex.Replace проходит по ВСЕМ совпадениям в шаблоне
    string result = Regex.Replace(htmlTemplate, pattern, match =>
    {
        // match.Groups[1].Value — это то, что внутри скобок (например, "Name" или " user.Age ")
        string propertyName = match.Groups[1].Value.Trim();

        // 3. Получаем тип нашей модели (User, Book, Car и т.д.)
        var type = dataModel.GetType();

        // 4. Проверяем, есть ли точка в имени свойства (вложенные объекты)
        if (propertyName.Contains('.'))
        {
            string[] parts = propertyName.Split('.');
            object currentObject = dataModel;

            foreach (var part in parts)
            {
                if (currentObject == null) break;

                // НОВОЕ: Если это первая часть и она равна названию класса (bag для Bag) - пропускаем её
                if (currentObject == dataModel && part.Equals(type.Name, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var currentProperty = currentObject.GetType().GetProperty(part,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);

                if (currentProperty != null)
                {
                    currentObject = currentProperty.GetValue(currentObject);
                }
                else
                {
                    currentObject = null;
                    break;
                }
            }

            return currentObject?.ToString() ?? string.Empty;
        }
        else
        {
            // 5. Если свойства нет (или оно простое), ищем его напрямую
            var property = type.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            
            if (property != null)
            {
                var value = property.GetValue(dataModel);
                return value?.ToString() ?? string.Empty;
            }
        }

        return string.Empty; // Свойство не найдено — заменяем на пустую строку
    });

    return result;
}

    // Остальные методы пока можно оставить пустыми или выбросить NotImplementedException
    public string RenderFromFile(string filePath, object dataModel) => throw new NotImplementedException();
    public string RenderToFile(string inputFilePath, string outputFilePath, object dataModel) => throw new NotImplementedException();
}