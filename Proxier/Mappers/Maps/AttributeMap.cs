using System;
using System.Linq.Expressions;
using System.Reflection;
using Proxier.Interfaces;

namespace Proxier.Mappers.Maps
{
    /// <inheritdoc />
    /// <summary>
    /// The mapper class
    /// </summary>
    public class AttributeMap : IPropertyMap
    {
        static AttributeMap()
        {
            ProxierMapper.InitializeMapperClasses();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeMap" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <exception cref="ArgumentNullException">parent</exception>
        public AttributeMap(AttributeMapper parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        /// This mapper property info
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// This mapper attribute expression
        /// </summary>
        public Expression<Func<Attribute>>[] Attributes { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        /// <value>
        /// The parent.
        /// </value>
        public AttributeMapper Parent { get; }
    }
}
