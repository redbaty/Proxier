using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Proxier.Extensions;
using Proxier.Repositories;

namespace Proxier.Builders
{
    /// <summary>
    ///     Represents a class builder
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ClassBuilder
    {
        /// <summary>
        ///     Gets the additional usings.
        /// </summary>
        /// <value>
        ///     The additional usings.
        /// </value>
        public List<string> AdditionalUsings { get; } = new List<string>();

        /// <summary>
        ///     Gets the class name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the namespace.
        /// </summary>
        /// <value>
        ///     The namespace.
        /// </value>
        public string Namespace { get; private set; }

        /// <summary>
        ///     Gets the property builders.
        /// </summary>
        /// <value>
        ///     The property builders.
        /// </value>
        public HashSet<PropertyBuilder> PropertyBuilders { get; } = new HashSet<PropertyBuilder>();

        private static Dictionary<string, Assembly> AssembliesCache { get; } = new Dictionary<string, Assembly>();

        private bool IsInterface { get; set; }

        private List<string> Parents { get; } = new List<string>();

        /// <summary>
        ///     Specifies that this class should be a interface instead.
        /// </summary>
        /// <returns></returns>
        public ClassBuilder AsInterface()
        {
            IsInterface = true;
            return this;
        }

        /// <summary>
        ///     Builds this instance into a real type.
        /// </summary>
        /// <returns></returns>
        public Type Build()
        {
            var code = GetAsCode();

            return AssembliesCache.GetOrAdd(code, () => new ClassBuilderRepository().GenerateAssembly(code)).GetTypes()
                .LastOrDefault();
        }

        /// <summary>
        ///     Use an existing type as model.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public ClassBuilder FromType(Type type)
        {
            foreach (var propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                WithProperty(propertyInfo.Name, propertyInfo.PropertyType, !propertyInfo.CanWrite,
                    propertyInfo.GetCustomAttributes<Attribute>().ToArray());

            return this;
        }

        /// <summary>
        ///     Gets the result as code instead of an type.
        /// </summary>
        /// <returns></returns>
        public string GetAsCode()
        {
            var propertiesBuilt = PropertyBuilders.Select(i =>
            {
                if (IsInterface)
                    i.AsInterface();

                return i.Build().ToString();
            }).ToArray();
            var typesInUse = PropertyBuilders.Select(i => i.PropertyType).Concat(PropertyBuilders
                .Where(i => i.Attributes != null)
                .SelectMany(o => o.Attributes.Select(j => j.Compile().Invoke().GetType())));
            var uniqueUsings = typesInUse.Select(o => o.Namespace).Distinct();

            return BuildClassOrInterface(uniqueUsings, propertiesBuilt, Name, IsInterface);
        }

        /// <summary>
        ///     Makes this class inherit from a certain class or interface.
        /// </summary>
        /// <param name="classOrInterfaceToInherit"></param>
        /// <returns></returns>
        public ClassBuilder InheritsFrom(string classOrInterfaceToInherit)
        {
            Parents.Add(classOrInterfaceToInherit);
            return this;
        }

        /// <summary>
        ///     Creates the type on a certain namespace.
        /// </summary>
        /// <param name="nameSpace">The namespace.</param>
        /// <returns></returns>
        public ClassBuilder OnNamespace(string nameSpace)
        {
            Namespace = nameSpace;
            return this;
        }

        /// <summary>
        ///     Forcefully add this using into the built class.
        /// </summary>
        /// <param name="additionalUsing">The additional using.</param>
        /// <returns></returns>
        public ClassBuilder Using(string additionalUsing)
        {
            AdditionalUsings.Add(additionalUsing);
            return this;
        }

        /// <summary>
        ///     Forcefully add these usings into the built class.
        /// </summary>
        /// <param name="additionalUsings">The additional usings.</param>
        /// <returns></returns>
        public ClassBuilder Using(IEnumerable<string> additionalUsings)
        {
            AdditionalUsings.AddRange(additionalUsings);
            return this;
        }

        /// <summary>
        ///     Sets a class name (Random generated if none is found at build time)
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ClassBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        ///     Adds a property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        /// <returns></returns>
        public ClassBuilder WithProperty(string name, Type type, bool readOnly)
        {
            PropertyBuilders.Add(new PropertyBuilder(name, type, readOnly: readOnly));
            return this;
        }

        /// <summary>
        ///     Adds a property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public ClassBuilder WithProperty(string name, Type type, bool readOnly, IEnumerable<Attribute> attributes)
        {
            PropertyBuilders.Add(new PropertyBuilder(name, type, attributes, readOnly));
            return this;
        }

        /// <summary>
        ///     Adds a property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="readOnly">if set to <c>true</c> [read only].</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public ClassBuilder WithProperty(string name,
            Type type,
            bool readOnly,
            params Expression<Func<Attribute>>[] attributes)
        {
            PropertyBuilders.Add(new PropertyBuilder(name, type, attributes, readOnly));
            return this;
        }

        /// <summary>
        ///     Adds a property.
        /// </summary>
        /// <param name="propertyBuilder">The property builder.</param>
        /// <returns></returns>
        public ClassBuilder WithProperty(PropertyBuilder propertyBuilder)
        {
            PropertyBuilders.Add(propertyBuilder);
            return this;
        }

        /// <summary>
        ///     Adds multiple properties.
        /// </summary>
        /// <param name="propertyBuilders">The property builders.</param>
        /// <returns></returns>
        public ClassBuilder WithProperty(IEnumerable<PropertyBuilder> propertyBuilders)
        {
            foreach (var propertyBuilder in propertyBuilders) PropertyBuilders.Add(propertyBuilder);

            return this;
        }

        private string BuildClassOrInterface(IEnumerable<string> uniqueUsings,
            string[] propertiesBuilt,
            string name,
            bool asInterface)
        {
            var classRepresentationBuilder = new ClassRepresentationBuilder().InheritsFrom(Parents)
                .WithNamespace(Namespace)
                .WithUsings(AdditionalUsings.ToArray())
                .WithUsings(uniqueUsings.ToArray())
                .WithName(name).WithProperties(propertiesBuilt);

            if (asInterface)
                classRepresentationBuilder.AsInterface();

            var classResult = classRepresentationBuilder.Build();
            return classResult;
        }
    }
}