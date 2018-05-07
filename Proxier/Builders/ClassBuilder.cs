using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using FluentCache;
using FluentCache.Simple;
using Proxier.Repositories;

namespace Proxier.Builders
{
    /// <summary>
    ///     Represents a class builder
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class ClassBuilder
    {
        static ClassBuilder()
        {
            Cache = new FluentDictionaryCache().WithSource(new ClassBuilderRepository());
        }

        private static Cache<ClassBuilderRepository> Cache { get; }

        /// <summary>
        ///     Gets the class name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the property builders.
        /// </summary>
        /// <value>
        ///     The property builders.
        /// </value>
        public HashSet<PropertyBuilder> PropertyBuilders { get; } = new HashSet<PropertyBuilder>();

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
        public ClassBuilder WithProperty(string name, Type type, bool readOnly,
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

        /// <summary>
        ///     Builds this instance into a real type.
        /// </summary>
        /// <returns></returns>
        public Type Build()
        {
            var propertiesBuilt = PropertyBuilders.Select(i => i.Build().ToString()).ToArray();
            var typesInUse = PropertyBuilders.Select(i => i.PropertyType).Concat(PropertyBuilders
                .Where(i => i.Attributes != null)
                .SelectMany(o => o.Attributes.Select(j => j.Compile().Invoke().GetType())));
            var uniqueUsings = typesInUse.Select(o => o.Namespace).Distinct();

            var buildClass = BuildClass(uniqueUsings, propertiesBuilt, Name);

            var generateAssembly =
                Cache.Method(i => i.GenerateAssembly(buildClass)).GetValue();

            return generateAssembly.GetTypes()
                .LastOrDefault();
        }

        private static string BuildClass(IEnumerable<string> uniqueUsings, string[] propertiesBuilt, string name)
        {
            var classRepresentationBuilder = new ClassRepresentationBuilder();
            classRepresentationBuilder.WithUsings(uniqueUsings.ToArray());
            classRepresentationBuilder.WithName(name);
            classRepresentationBuilder.WithProperties(propertiesBuilt);
            var classResult = classRepresentationBuilder.Build();
            return classResult;
        }
    }
}