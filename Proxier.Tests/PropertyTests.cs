using System;
using System.Reflection;
using Proxier.Extensions;
using Proxier.Mappers;
using Proxier.Mappers.Maps;
using Xunit;

namespace Proxier.Tests
{
    public class TestClass
    {
        public string DefaultProperty { get; set; }
    }

    public class TestClassExtension : AttributeMapper<TestClass>
    {
        public TestClassExtension()
        {
            AddProperty("NewProperty", typeof(string));
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class TestPropertyAttribute : Attribute
    {
        public string Test { get; set; } = "Hello World!";
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class TestClassAttribute : Attribute
    {
        public string Test { get; set; } = "World Hello!";
    }

    public class PropertyTests
    {
        [Fact]
        public void Test1()
        {
            var item = new TestClass();
            ProxierMapper.InitializeMapperClasses();

            var crazyObj = item.AddProperty("Hello", typeof(string))
                .AddPropertyAttribute(() => new TestPropertyAttribute())
                .AddClassAttribute(() => new TestClassAttribute()).Object;

            Assert.True(crazyObj.GetType().GetProperty("Hello") != null);
            Assert.True(crazyObj.GetType().GetProperty("Hello").GetCustomAttribute<TestPropertyAttribute>() != null);
            Assert.True(crazyObj.GetType().GetCustomAttribute<TestClassAttribute>() != null);
            Assert.True(item.GetType().GetProperty(nameof(TestClass.DefaultProperty)) != null);
            Assert.True(item.GetType().GetInjectedType().GetProperty("NewProperty") != null);
        }
    }
}
