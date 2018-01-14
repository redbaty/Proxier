using System;
using Proxier.Mappers;
using Proxier.Mappers.Maps;

namespace Proxier.Extensions
{
    /// <summary>
    /// Fluent mapper extensions
    /// </summary>
    public static class FluentExtensions
    {
        /// <summary>
        /// Adds a property to this object
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="propType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static object AddProperty<T>(this T item, string name, Type propType)
        {
            var mapper = item.GetType().GetMapper() ?? new AttributeMapper(item.GetType());
            mapper.CustomProperties.Add(new PropertyMap(mapper, name, propType));
            return mapper.Spawn();
        }
    }
}