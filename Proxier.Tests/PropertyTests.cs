using System;
using System.Linq;
using Newtonsoft.Json;
using Proxier.Mappers;
using Xunit;
using Xunit.Abstractions;

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
    
    public class PropertyTests
    {
        private readonly ITestOutputHelper _output;

        public PropertyTests(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact]
        public void Test1()
        {
            var item = new TestClass();
            
            Assert.True(item.GetType().GetProperty(nameof(TestClass.DefaultProperty)) != null);

            _output.WriteLine($"Injected Properties: {JsonConvert.SerializeObject(item.GetType().GetInjectedType().GetProperties().Select(i => i.Name), Formatting.Indented)}");
            
            Assert.True(item.GetType().GetInjectedType().GetProperty("NewProperty") != null);
        }
    }
}