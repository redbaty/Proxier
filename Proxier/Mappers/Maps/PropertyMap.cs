using System;
using System.Linq.Expressions;
using Proxier.Interfaces;

namespace Proxier.Mappers.Maps
{
    /// <summary>
    ///     Describes a property mappings
    /// </summary>
    public class PropertyMap : IPropertyMap
    {
        /// <summary>
        ///     Creates a new property
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="propertyType"></param>
        public PropertyMap(AttributeMapper parent, string name, Type propertyType)
        {
            Parent = parent;
            Name = name;
            PropertyType = propertyType;
        }

        /// <summary>
        ///     Property name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     The property type
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        ///     The parent mapper
        /// </summary>
        public AttributeMapper Parent { get; }

        /// <inheritdoc />
        /// <summary>
        ///     This mapper attribute expression
        /// </summary>
        public Expression<Func<Attribute>>[] Attributes { get; set; } = new Expression<Func<Attribute>>[0];
    }
}