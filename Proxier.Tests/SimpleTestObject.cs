namespace Proxier.Tests
{
    public class SimpleTestObject
    {
        public string Property1 { get; set; }

        public int Property2 { get; set; }

        public override string ToString() => $"{nameof(Property1)}: {Property1}, {nameof(Property2)}: {Property2}";
    }
}