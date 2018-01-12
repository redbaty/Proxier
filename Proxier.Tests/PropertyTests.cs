using Proxier.Mappers;
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
    
    public class PropertyTests
    {
        [Fact]
        public void Test1()
        {
            var item = new TestClass();
            
            Assert.True(item.GetType().GetProperty(nameof(TestClass.DefaultProperty)) != null);            
            Assert.True(item.GetType().GetInjectedType().GetProperty("NewProperty") != null);
        }
    }
}