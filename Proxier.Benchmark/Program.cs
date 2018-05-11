using System;
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

        private static void Main()
        {
            ConvertToInterface();
            SingleIteration();
            MultipleIteration();
            DeepClone();
            Console.ReadLine();
        }

        private static void ConvertToInterface()
        {
            var builder = new ClassBuilder();
            var res = builder.FromType(typeof(Person)).AsInterface().WithName("IPerson").Build();
        }

        [Time]
        private static void DeepClone()
        {
            var baseIn = new Person
            {
                FirstName = "Godammit"
            };

            for (var i = 0; i < 1000; i++)
            {
                var baseOut = new Person();
                baseIn.CopyTo(baseOut);

                if (baseOut.FirstName != baseIn.FirstName) Log.Logger.Fatal("Something went really wrong");
            }
        }

        [Time]
        private static void MultipleIteration()
        {
            for (var i = 0; i < 1000; i++) Spawn();
        }

        [Time]
        private static void SingleIteration()
        {
            Spawn();
        }

        private static void Spawn()
        {
            var classBuilder = new ClassBuilder();
            var type = classBuilder.WithName("Person").WithProperty("FirstName", typeof(string), false)
                .Build();
        }
    }
}