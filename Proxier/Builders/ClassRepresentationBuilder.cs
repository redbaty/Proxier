using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Proxier.Builders
{
    internal class ClassRepresentationBuilder
    {
        private List<string> ClassAttributes { get; } = new List<string>();

        private string ClassName { get; set; }
            = Nanoid.Nanoid.Generate("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ", 32);

        private bool IsInterface { get; set; }

        private string Namespace { get; set; }

        private List<string> Properties { get; } = new List<string>();

        private string Representation { get; set; } = "";

        private List<string> Usings { get; } = new List<string> {"System"};

        private List<string> Parents { get; } = new List<string>();

        public ClassRepresentationBuilder WithNamespace(string nameSpace)
        {
            Namespace = nameSpace;
            return this;
        }

        public ClassRepresentationBuilder InheritsFrom(string toInherit)
        {
            Parents.Add(toInherit);
            return this;
        }

        public ClassRepresentationBuilder InheritsFrom(IEnumerable<string> toInherit)
        {
            Parents.AddRange(toInherit);
            return this;
        }

        public ClassRepresentationBuilder AsInterface()
        {
            IsInterface = true;
            return this;
        }

        private void AddClassHeader()
        {
            foreach (var @using in Usings.Distinct()) WriteLine($"using {@using};");

            WriteLine("");

            if (!string.IsNullOrEmpty(Namespace)) AddNameSpace();

            AddClassAttributes();
            WriteLine($"public {(IsInterface ? "interface" : "class")} {ClassName}{ResolveInheritance()} {{");
        }

        private string ResolveInheritance()
        {
            if (Parents.Any())
            {
                return $" : {(Parents.Distinct().OrderBy(i => i).Aggregate((x, y) => $"{x}, {y}"))}";
            }

            return string.Empty;
        }

        private void AddNameSpace()
        {
            WriteLine($"namespace {Namespace}\n{{");
        }

        private void AddClassAttributes()
        {
            foreach (var classAttribute in ClassAttributes) WriteLine(classAttribute);
        }

        private void AddProperties()
        {
            Representation += Properties.Aggregate((x, y) => $"{x}{Environment.NewLine}{y}");
        }

        private void AddFooter()
        {
            WriteLine("}");

            if (!string.IsNullOrEmpty(Namespace)) WriteLine("}");
        }

        private void WriteLine(string toWrite)
        {
            Representation += toWrite + Environment.NewLine;
        }

        public ClassRepresentationBuilder WithClassAttributes(params string[] classAttributes)
        {
            ClassAttributes.AddRange(classAttributes);
            return this;
        }

        public ClassRepresentationBuilder WithName(string name)
        {
            if (!string.IsNullOrEmpty(name)) ClassName = name;

            return this;
        }

        public ClassRepresentationBuilder WithProperties(params string[] properties)
        {
            Properties.AddRange(properties);
            return this;
        }

        public ClassRepresentationBuilder WithUsings(params string[] usings)
        {
            Usings.AddRange(usings);
            return this;
        }

        public string Build()
        {
            AddClassHeader();
            AddProperties();
            AddFooter();
            return CSharpSyntaxTree.ParseText(Representation).GetRoot().NormalizeWhitespace().ToFullString();
        }
    }
}