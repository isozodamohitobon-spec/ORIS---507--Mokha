using Microsoft.VisualStudio.TestTools.UnitTesting;
using Workers; // Подключаем твои классы

namespace Workers
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void UserConstructor_ShouldSetPropertiesCorrectly()
        {
            // 1. Arrange (Подготовка)
            string expectedName = "Иван";
            string expectedGroup = "ПИ-21";
            int expectedAge = 20;
            bool expectedGender = true; 

            // 2. Act (Действие)
            // Создаем пользователя через твой конструктор
            var user = new User(expectedName, expectedGroup, expectedAge, expectedGender);

            // 3. Assert (Проверка)
            // Проверяем, что все данные записались правильно
            Assert.AreEqual(expectedName, user.Name);
            Assert.AreEqual(expectedGroup, user.Group);
            Assert.AreEqual(expectedAge, user.Age);
            Assert.AreEqual(expectedGender, user.Gender);
        }
    }
    [TestClass]
    public class HtmlTemplateTests
    {
        [TestMethod]
        public void RenderFromString_ReplacesProperties_IgnoringCase()
        {
            // Arrange
            var renderer = new HtmlTemplate();
            var user = new User("Вася", "11-507", 19, true);
            // Пишем name и age с маленькой буквы, чтобы проверить IgnoreCase
            string template = "<p>Имя:${name}</p><p>Возраст:${age}</p>";

            // Act
            string result = renderer.RenderFromString(template, user);

            // Assert
            Assert.AreEqual("<p>Имя:Вася</p><p>Возраст:19</p>", result);
        }

        [TestMethod]
        public void RenderFromString_HandlesNestedProperties()
        {
            // Arrange
            var renderer = new HtmlTemplate();
            var brand = new Brand { Name = "Chanel", Country = "France" };
            var bag = new Bag(new Size { Width = 30, Height = 20, Depth = 10 }, "Сумка", brand, "Черный");
            string template = "Бренд: ${bag.Brand.Name}, Ширина: ${bag.Size.Width}";

            // Act
            string result = renderer.RenderFromString(template, bag);

            // Assert
            Assert.AreEqual("Бренд: Chanel, Ширина: 30", result);
        }

        [TestMethod]
        public void RenderFromString_ReturnsEmptyString_WhenPropertyNotFound()
        {
            // Arrange
            var renderer = new HtmlTemplate();
            var user = new User("Петя", "10-1", 18, true);
            string template = "<p>Имя:${Name}</p><p>Отчество:${Patronymic}</p>"; // Отчества нет в классе User

            // Act
            string result = renderer.RenderFromString(template, user);

            // Assert
            // Ожидаем, что на месте несуществующего свойства будет пустая строка
            Assert.AreEqual("<p>Имя:Петя</p><p>Отчество:</p>", result);
        }
    }
    [TestClass]
    public class CarTests
    {
        [TestMethod]
        public void CarConstructor_ShouldSetProperties()
        {
            // Arrange & Act
            var car = new Car(2027, "Машина", "Ferrari");

            // Assert
            Assert.AreEqual(2027, car.Year);
            Assert.AreEqual("Машина", car.Name);
            Assert.AreEqual("Ferrari", car.Marka);
        }
    }

    [TestClass]
    public class BookTests
    {
        [TestMethod]
        public void BookConstructor_ShouldSetProperties()
        {
            // Arrange & Act
            var book = new Book("Война и мир", "Толстой", 1869);

            // Assert
            Assert.AreEqual("Война и мир", book.Title);
            Assert.AreEqual("Толстой", book.Autor);
            Assert.AreEqual(1869, book.Year);
        }
    }

    [TestClass]
    public class BagTests
    {
        [TestMethod]
        public void BagConstructor_ShouldSetAllProperties_IncludingNestedObjects()
        {
            // Arrange
            var brand = new Brand { Name = "Chanel", Country = "France" };
            var size = new Size { Width = 30, Height = 20, Depth = 10 };

            // Act
            var bag = new Bag(size, "Сумка", brand, "Черный");

            // Assert (проверяем и обычные свойства, и вложенные объекты)
            Assert.AreEqual("Сумка", bag.Name);
            Assert.AreEqual("Черный", bag.Color);
            Assert.AreEqual("Chanel", bag.Brand.Name);
            Assert.AreEqual("France", bag.Brand.Country);
            Assert.AreEqual(30, bag.Size.Width);
        }
    }
}