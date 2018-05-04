using System;
using System.Reflection;

namespace Proxier.Extensions
{
    /// <summary>
    ///     Property info extensions
    /// </summary>
    public static class PropertyInfoExtensions
    {
        /// <summary>
        ///     Get the last property from a type based on a name.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static PropertyInfo GetHighestProperty(this Type type, string name)
        {
            while (type != null)
            {
                var property = type.GetProperty(name, BindingFlags.DeclaredOnly |
                                                      BindingFlags.Public |
                                                      BindingFlags.Instance);
                if (property != null) return property;
                type = type.BaseType;
            }

            return null;
        }

        /// <summary>
        ///     Gets the highest property value.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static object GetHighestPropertyValue(this object obj, string property)
        {
            return obj.GetType().GetHighestProperty(property).GetValue(obj, null);
        }
    }
}