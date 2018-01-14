using System;
using System.Linq.Expressions;
using Proxier.Mappers;

namespace Proxier.Interfaces
{
    /// <summary>
    /// Represents a mapper
    /// </summary>
    public interface IPropertyMap
    {
        /// <summary>
        ///     Gets or sets the parent.
        /// </summary>
        /// <value>
        ///     The parent.
        /// </value>
        AttributeMapper Parent { get; }

        /// <summary>
        /// Property attributes
        /// </summary>
        Expression<Func<Attribute>>[] Attributes { get; set; }
    }
}
