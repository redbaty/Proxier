using System;
using System.Collections.Generic;
using System.Reflection;
using Proxier.Interfaces;

namespace Proxier
{
    /// <summary>
    ///     Represents copy options
    /// </summary>
    public class CopyToOptions
    {
        /// <summary>
        ///     If set, the copier will include non public properties.
        /// </summary>
        /// <returns></returns>
        public bool CopyPrivates { get; set; }

        /// <summary>
        ///     If set this will not copy null values.
        /// </summary>
        /// <returns></returns>
        public bool IgnoreNulls { get; set; }

        /// <summary>
        ///     These properties will be ignored when copying.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PropertyInfo> PropertiesToIgnore { get; set; }

        /// <summary>
        ///     If set only these properties will be used when copying objects.
        /// </summary>
        public IEnumerable<PropertyInfo> PropertiesToInclude { get; set; }

        /// <summary>
        ///     This will be used to get the value that will be set to a certain property.
        /// </summary>
        /// <returns></returns>
        public Func<ICopyContext, object> Resolver { get; set; } = context => context.Property.GetValue(context.Source);

        /// <summary>
        ///     If set and the properties between the two objects are different
        ///     the copier will try to convert it to the target type.
        /// </summary>
        /// <returns></returns>
        public bool TryToConvert { get; set; } = true;

        /// <summary>
        ///     If set the copier will compare the base type from nullable types
        /// </summary>
        public bool UseNullableBaseType { get; set; } = true;
    }
}