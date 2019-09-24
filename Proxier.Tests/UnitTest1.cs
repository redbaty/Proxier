using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Proxier.Extensions;

namespace Proxier.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestSimpleDeepClone()
        {
            var originalObject = new SimpleTestObject {Property1 = "abcd", Property2 = 1};
            var clonedObject = originalObject.DeepClone();
            clonedObject.Property1 = "abcdefg";
            clonedObject.Property2 = 0;

            Assert.AreNotEqual(originalObject.Property1, clonedObject.Property1);
            Assert.AreNotEqual(originalObject.Property2, clonedObject.Property2);
            Assert.AreEqual("abcd", originalObject.Property1);
            Assert.AreEqual(1, originalObject.Property2);

            Assert.AreEqual("abcdefg", clonedObject.Property1);
            Assert.AreEqual(0, clonedObject.Property2);
        }

        [TestMethod]
        public void TestCollectionDeepClone()
        {
            var originalObject = new CollectionTestObject {Property1 = "abcd", Property2 = new List<string> {"a", "b"}};
            var clonedObject = originalObject.DeepClone();
            clonedObject.Property1 = "abcdefg";
            clonedObject.Property2.Add("c");
            clonedObject.Property2.Add("d");

            Assert.AreEqual("abcd", originalObject.Property1);
            Assert.AreEqual("abcdefg", clonedObject.Property1);
            Assert.AreNotEqual(originalObject.Property2.Count, clonedObject.Property2.Count);
        }

        [TestMethod]
        public void TestComplexDeepClone()
        {
            var originalObject = new ComplexTestObject
            {
                    Property1 = new SimpleTestObject {Property1 = "hey", Property2 = 0},
                    Property2 = new List<SimpleTestObject>
                    {
                            new SimpleTestObject {Property1 = "o", Property2 = 1},
                            new SimpleTestObject {Property1 = "h", Property2 = 3},
                            new SimpleTestObject {Property1 = "a", Property2 = 2}
                    }
            };
            var clonedObject = originalObject.DeepClone();
            clonedObject.Property2.AddRange(new[]
            {
                    new SimpleTestObject {Property1 = "i", Property2 = 4},
                    new SimpleTestObject {Property1 = "o", Property2 = 5}
            });
            clonedObject.Property1.Property1 = "nope";
            clonedObject.Property1.Property2 = 1;
            
            Assert.AreNotEqual(originalObject.Property2.Count, clonedObject.Property2.Count);
            Assert.AreNotEqual(originalObject.Property1.Property1, clonedObject.Property1.Property1);
            Assert.AreNotEqual(originalObject.Property1.Property2, clonedObject.Property1.Property2);
        }
    }
}