using System.Collections.Generic;

namespace Proxier.Tests
{
    public class ComplexTestObject
    {
        public SimpleTestObject Property1 { get; set; }
        
        public List<SimpleTestObject> Property2 { get; set; }

        public override string ToString() => $"{nameof(Property1)}: {Property1}, {nameof(Property2)}: {Property2}";
    }
}