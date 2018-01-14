using System;
using System.Linq.Expressions;
using System.Reflection;
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
        /// Adds or merge to the global type overrides.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <returns></returns>
        public static FluentResult AddOrMerge(this FluentResult result)
        {
            if (ProxierMapper.TypesOverrides.ContainsKey(result.AttributeMapper.BaseType))
            {
                ProxierMapper.TypesOverrides[result.AttributeMapper.BaseType].Merge(result.AttributeMapper);
            }
            else
            {
                ProxierMapper.TypesOverrides.Add(result.AttributeMapper.BaseType, result.AttributeMapper);
            }
            return result;
        }

        /// <summary>
        /// Adds a property to this object
        /// </summary>
        /// <param name="item"></param>
        /// <param name="name"></param>
        /// <param name="propType"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static FluentResult AddProperty<T>(this T item, string name, Type propType)
        {
            var mapper = item.GetType().GetMapper() ?? new AttributeMapper(item.GetType());
            mapper.CustomProperties.Add(new PropertyMap(mapper, name, propType));
            var returnItem = mapper.Spawn();
            var fluentResult = new FluentResult(returnItem, returnItem.GetType().GetProperty(name),
                attributeMapper: mapper);
            return fluentResult;
        }


        /// <summary>
        /// Adds a class attribute.
        /// </summary>
        /// <param name="fluentResult">The fluent result.</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public static FluentResult AddClassAttribute(this FluentResult fluentResult,
            params Expression<Func<Attribute>>[] attributes)
        {
            var mapper = fluentResult.Object.GetType().GetMapper() ??
                         new AttributeMapper(fluentResult.Object.GetType());
            mapper.AttributeMappings.Add(new AttributeMap(mapper) { PropertyInfo = null, Attributes = attributes });
            return new FluentResult(mapper.Spawn(), attributeMapper: mapper);
        }

        /// <summary>
        /// Adds a property attribute.
        /// </summary>
        /// <param name="fluentResult">The fluent result.</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public static FluentResult AddPropertyAttribute(this FluentResult fluentResult,
            params Expression<Func<Attribute>>[] attributes)
        {
            return AddPropertyAttribute(fluentResult.Object, fluentResult.PropertyInfo, attributes);
        }

        /// <summary>
        /// Adds a property attribute.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="attributes">The attributes.</param>
        /// <returns></returns>
        public static FluentResult AddPropertyAttribute<T>(this T item, PropertyInfo propertyInfo,
            params Expression<Func<Attribute>>[] attributes)
        {
            var mapper = item.GetType().GetMapper() ?? new AttributeMapper(item.GetType());

            mapper.AttributeMappings.Add(new AttributeMap(mapper)
            {
                PropertyInfo = propertyInfo,
                Attributes = attributes
            });
            return new FluentResult(mapper.Spawn(), attributeMapper: mapper);
        }
    }

    /// <summary>
    /// Represents a result from Fluent extensions
    /// </summary>
    public class FluentResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FluentResult" /> class.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="propertyInfo">The property information.</param>
        /// <param name="saveAsGlobal">if set to <c>true</c> [save as global].</param>
        /// <param name="attributeMapper">The attribute mapper.</param>
        public FluentResult(object o = null, PropertyInfo propertyInfo = null, bool saveAsGlobal = false,
            AttributeMapper attributeMapper = null)
        {
            AttributeMapper = attributeMapper;
            SaveAsGlobal = saveAsGlobal;
            Object = o;
            PropertyInfo = propertyInfo;
        }

        /// <summary>
        /// Gets the object.
        /// </summary>
        /// <value>
        /// The object.
        /// </value>
        public object Object { get; }

        /// <summary>
        /// Gets the attribute mapper.
        /// </summary>
        /// <value>
        /// The attribute mapper.
        /// </value>
        public AttributeMapper AttributeMapper { get; }

        /// <summary>
        /// Gets the property information.
        /// </summary>
        /// <value>
        /// The property information.
        /// </value>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets a value indicating whether [save as global].
        /// </summary>
        /// <value>
        /// <c>true</c> if [save as global]; otherwise, <c>false</c>.
        /// </value>
        public bool SaveAsGlobal { get; }
    }
}
