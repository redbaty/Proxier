using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Proxier.Representations;

namespace Proxier.Builders
{
    /// <summary>
    ///     Represents a property builder
    /// </summary>
    public class PropertyBuilder
    {
        /// <inheritdoc />
        public PropertyBuilder()
        {
        }

        /// <inheritdoc />
        public PropertyBuilder(string name, Type propertyType,
            IEnumerable<Expression<Func<Attribute>>> attributes = null, bool readOnly = false)
        {
            Attributes = attributes;
            Name = name;
            PropertyType = propertyType;
            IsReadOnly = readOnly;
        }

        /// <inheritdoc />
        public PropertyBuilder(string name, Type propertyType, IEnumerable<Attribute> attributes, bool readOnly = false)
        {
            Name = name;
            PropertyType = propertyType;
            CompiledAttributes = attributes;
            IsReadOnly = readOnly;
        }

        /// <summary>
        ///     Attribute expressions.
        /// </summary>
        /// <value>
        ///     The expressions.
        /// </value>
        public IEnumerable<Expression<Func<Attribute>>> Attributes { get; private set; }

        /// <summary>
        ///     Gets the type of the property.
        /// </summary>
        /// <value>
        ///     The type of the property.
        /// </value>
        public Type PropertyType { get; private set; }

        /// <summary>
        ///     Attributes instances.
        /// </summary>
        /// <value>
        ///     The attributes instances.
        /// </value>
        private IEnumerable<Attribute> CompiledAttributes { get; }

        private bool IsReadOnly { get; set; }

        private bool IsInterface { get; set; }

        private string Name { get; set; }

        /// <summary>
        ///     Adds an attributes.
        /// </summary>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public PropertyBuilder WithAttributes(params Expression<Func<Attribute>>[] attributes)
        {
            Attributes = attributes;
            return this;
        }

        /// <summary>
        ///     Sets the property name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public PropertyBuilder WithName(string name)
        {
            Name = name;
            return this;
        }

        /// <summary>
        ///     Sets the propertie's type.
        /// </summary>
        /// <param name="propertyType">Type of the property.</param>
        /// <returns></returns>
        public PropertyBuilder WithType(Type propertyType)
        {
            PropertyType = propertyType;
            return this;
        }

        public PropertyBuilder AsInterface()
        {
            IsInterface = true;
            return this;
        }

        /// <summary>
        ///     Makes the property read only.
        /// </summary>
        /// <returns></returns>
        public PropertyBuilder ReadOnly()
        {
            IsReadOnly = true;
            return this;
        }

        /// <summary>
        ///     Builds this instance.
        /// </summary>
        /// <returns></returns>
        public PropertyRepresentation Build()
        {
            return new PropertyRepresentation(Name, PropertyType, IsReadOnly, Attributes, CompiledAttributes, IsInterface);
        }
    }
}