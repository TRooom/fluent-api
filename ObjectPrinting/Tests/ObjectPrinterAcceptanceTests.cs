using System;
using System.Drawing;
using System.Globalization;
using FluentAssertions;
using NUnit.Framework;

namespace ObjectPrinting.Tests
{
    [TestFixture]
    public class ObjectPrinterAcceptanceTests
    {
        private Person person;
        private IPrintingConfig<Person> printer;
        private const string newLine = "\r\n";

        [SetUp]
        public void SetUp()
        {
            person = new Person {Name = "Alex", Age = 19};
            printer = ObjectPrinter.For<Person>();
        }

        [Test]
        public void Demo()
        {
            var printer = ObjectPrinter.For<Person>()
                //1. Исключить из сериализации свойства определенного типа
                .Excluding<Guid>()
                //2. Указать альтернативный способ сериализации для определенного типа
                .Printing<int>().Using(i => i.ToString("X"))
                //3. Для числовых типов указать культуру
                .Printing<double>().Using(CultureInfo.InvariantCulture)
                //4. Настроить сериализацию конкретного свойства
                .Printing(x => x.Age).Using(x => $"Age is {x}")
                //5. Настроить обрезание строковых свойств (метод должен быть виден только для строковых свойств)
                .Printing(p => p.Name).TrimingToLength(1)
                //6. Исключить из сериализации конкретного свойства
                .Excluding(p => p.Id);

            string s = printer.PrintToString(person);
            Console.WriteLine(s);
        }

        [Test]
        public void RerutnTypeName_WhenAllPropertiesExluded()
        {
            printer
                .Excluding(x => x.Name)
                .Excluding(x => x.Age)
                .Excluding(x => x.Height)
                .Excluding(x => x.Id);
            printer.PrintToString(person).Should().Be(nameof(Person) + Environment.NewLine);
        }

        [Test]
        public void RerutnTypeName_WhenAllTypesExluded()
        {
            printer
                .Excluding<Guid>()
                .Excluding<int>()
                .Excluding<double>()
                .Excluding<string>();
            printer.PrintToString(person).Should().Be(nameof(Person) + Environment.NewLine);
        }

        [Test]
        public void DefaultPinting()
        {
            var expected = string.Format("Person{0}\tId = Guid{0}\tName = Alex{0}\tHeight = 0\r\n\tAge = 19{0}",
                newLine);
            printer.PrintToString(person).Should().Be(expected);
        }

        [Test]
        public void ExcludeType()
        {
            var expected = string.Format("Person{0}\tId = Guid{0}\tHeight = 0\r\n\tAge = 19{0}", newLine);
            printer.Excluding<string>();
            printer.PrintToString(person).Should().Be(expected);
        }

        [Test]
        public void ExludeProperty()
        {
            var expected = string.Format("Person{0}\tName = Alex{0}\tHeight = 0\r\n\tAge = 19{0}",
                newLine);
            printer.Excluding(p => p.Id);
            printer.PrintToString(person).Should().Be(expected);
        }

        [Test]
        public void SpecifyTypePrinting()
        {
            var expected = string.Format("Person{0}\tId = Guid{0}\tName = Alex{0}\tHeight = 0\r\n\tAge = -19-{0}",
                newLine);
            printer.Printing<int>().Using(x => $"-{x}-");
            printer.PrintToString(person).Should().Be(expected);
        }

        [Test]
        public void SpecifyPropertyPrinting()
        {
            var expected = string.Format("Person{0}\tId = Guid{0}\tName = Alex{0}\tHeight is 0\r\n\tAge = 19{0}",
                newLine);
            printer.Printing(p => p.Height).Using(x => $"Height is {x}");
            printer.PrintToString(person).Should().Be(expected);
        }

        [Test]
        public void SpecifyMaxLengthProperty()
        {
            var expected = string.Format("Person{0}\tId = Guid{0}\tName = Al{0}\tHeight = 0\r\n\tAge = 19{0}",
                newLine);
            printer.Printing(x => x.Name).TrimingToLength(2);
            printer.PrintToString(person).Should().Be(expected);
        }

        [Test]
        public void SpecifyCulture()
        {
            var expected = string.Format("Person{0}\tId = Guid{0}\tName = Alex{0}\tHeight = 111,5\r\n\tAge = 19{0}",
                newLine);
            person.Height = 111.5;
            printer.Printing<double>().Using(new CultureInfo("uz"));
            printer.PrintToString(person).Should().Be(expected);
        }
    }
}