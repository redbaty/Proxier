using System;
using System.ComponentModel;
using Proxier.Builders;

namespace ConsoleApp1
{
    class Program
    {
        public string Hellow { get; }

        static void Main(string[] args)
        {
            var x = new ClassBuilder();
            var result = x.FromType(typeof(Program)).Build().Result;
        }
    }
}