using System.Collections.Generic;

namespace Proxier.Tests
{
    public class CollectionTestObject
    {
        public string Property1 { get; set; }
        
        public ICollection<string> Property2 { get; set; }

        public override string ToString() => $"{nameof(Property1)}: {Property1}, {nameof(Property2)}: {Property2}";
    }
}