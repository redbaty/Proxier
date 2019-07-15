using System;
using System.Linq;
using Bogus;
using MethodTimer;
using Proxier.Builders;
using Proxier.Extensions;
using Serilog;

namespace Proxier.Benchmark
{
    internal class Program
    {
        static Program()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        private static void ConvertToInterface<T>()
        {
            var builder = new ClassBuilder();
            var classBuilder = builder.FromType(typeof(T)).AsInterface().WithName($"I{typeof(T).Name}");
            var typeAsInterface = classBuilder.Build();
            var interfaceProperties = typeAsInterface.GetProperties().Select(i => i.Name).ToList();
            var originalProperties = typeof(T).GetProperties().Select(i => i.Name).ToList();

            if (originalProperties.Any(o => !interfaceProperties.Contains(o)))
                throw new InvalidOperationException("Missing properties.");
        }

        private static void Main()
        {
            try
            {
                ConvertToInterface<Person>();
                SingleIteration();
                MultipleIteration();
                MultipleDeepClones();
            }
            catch (Exception e)
            {
                Log.Fatal(e, "Something went wrong.");
            }
        }

        [Time]
        private static void MultipleDeepClones()
        {
            var faker = new Faker<Person>().RuleFor(oi => oi.FirstName, f => f.Person.FirstName);

            for (var i = 0; i < 10000; i++)
            {
                var baseIn = faker.Generate();
                var baseOut = new Person();
                baseIn.CopyTo(baseOut);

                if (!string.Equals(baseOut.FirstName, baseIn.FirstName, StringComparison.Ordinal))
                    throw new InvalidOperationException("Cloned different values.");
            }
        }

        [Time]
        private static void MultipleIteration()
        {
            for (var i = 0; i < 1000; i++) Spawn("Person", "FirstName", "LastName");
        }

        [Time]
        private static void SingleIteration()
        {
            Spawn("Person", "FirstName", "LastName");
        }

        private static void Spawn(string className, params string[] properties)
        {
            var classBuilder = new ClassBuilder().WithName(className);
            classBuilder = properties.Aggregate(classBuilder,
                (current, property) => current.WithProperty(property, typeof(string), false));

            var type = classBuilder
                .WithName(className)
                .Build();

            foreach (var property in properties)
            {
                var propertyInfo = type.GetProperty(property);
                if (propertyInfo == null)
                    throw new InvalidOperationException($"Property '{property}' was not found.");
            }
        }
    }
}