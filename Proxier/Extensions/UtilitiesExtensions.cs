﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LazyCache;

namespace Proxier.Extensions
{
    public static partial class MapperExtensions
    {
        private static readonly IAppCache Cache = new CachingService();

        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <param name="baseClassInstance">The base class instance.</param>
        /// <param name="target">The target.</param>
        /// <param name="ignoreNulls"></param>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public static object CopyTo(this object baseClassInstance, object target,
            bool ignoreNulls = true, Func<PropertyInfo, object, object, object> interceptor = null)
        {
            interceptor = interceptor ?? ((info, t1, o) => info.GetValue(o));

            var sourceProperties = Cache.GetOrAdd($"pCopy->{baseClassInstance.GetType().FullName}",
                () => baseClassInstance.GetType().GetHighestProperties().Select(i => i.PropertyInfo));

            foreach (var propertyInfo in sourceProperties)
                try
                {
                    var value = interceptor.Invoke(propertyInfo, target, baseClassInstance);

                    var targetProperties = Cache.GetOrAdd($"pCopy->{target.GetType().FullName}->Properties",
                        () => target.GetType().GetHighestProperties().ToDictionary(i => i.PropertyInfo.Name));

                    if (ignoreNulls && value == null)
                        continue;

                    targetProperties[propertyInfo.Name].PropertyInfo.SetValue(target, value);
                }
                catch
                {
                    // ignored
                }

            return target;
        }

        /// <summary>
        ///     Copies object to another object of a type using reflection.
        /// </summary>
        /// <param name="baseClassInstance">The base class instance.</param>
        /// <param name="targetType">The type to copy to.</param>
        /// <param name="ignoreNulls"></param>
        /// <param name="interceptor"></param>
        /// <returns></returns>
        public static object CopyTo(this object baseClassInstance, Type targetType, bool ignoreNulls = true,
            Func<PropertyInfo, object, object, object> interceptor = null)
        {
            return baseClassInstance.CopyTo(Activator.CreateInstance(targetType.AddParameterlessConstructor()),
                ignoreNulls, interceptor);
        }

        /// <summary>
        ///     Returns if a certain type contains a override
        /// </summary>
        /// <param name="item"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T HasTypeOverride<T>(this T item) where T : class
        {
            var mapper = item.GetType().GetMapper();
            return mapper == null ? null : item;
        }

        /// <summary>
        ///     Gets all the properties values, the key being its name.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static ILookup<string, object> GetPropertiesValue(this object obj)
        {
            return obj.GetType().GetProperties()
                .ToLookup(property => property.Name, property => property.GetValue(obj));
        }

        /// <summary>
        ///     Gets if type has a parameterless constructor
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasParameterlessContructor(this Type type)
        {
            try
            {
                return type.GetConstructor(Type.EmptyTypes) != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Gets all base types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllBaseTypes(this Type type)
        {
            if (type == null || type.BaseType == null) return new List<Type> {type};

            var returnList = type.GetInterfaces().ToList();

            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                returnList.Add(currentBaseType);
                currentBaseType = currentBaseType.BaseType;
            }

            return returnList;
        }
    }
}