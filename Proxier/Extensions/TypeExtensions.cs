﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LazyCache;

namespace Proxier.Extensions
{
    /// <summary>
    ///     General Extensions
    /// </summary>
    public static class TypeExtensions
    {
        private static readonly IAppCache Cache = new CachingService();

        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <param name="source">The base class instance.</param>
        /// <returns></returns>
        public static void DeepClone(this object source)
        {
            CopyTo(source, Activator.CreateInstance(source.GetType()));
        }

        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <param name="source">The base class instance.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static void CopyTo(this object source, object target)
        {
            CopyTo(source, target, new CopyToOptions());
        }

        /// <summary>
        ///     Copies object to another object ignoring some properties.
        /// </summary>
        /// <param name="source">The base class instance.</param>
        /// <param name="target">The target.</param>
        /// <param name="propertiesToIgnore"></param>
        /// <returns></returns>
        public static void CopyTo<TSource, TProperty>(this TSource source, object target, params Expression<Func<TSource, TProperty>>[] propertiesToIgnore)
        {
            CopyTo(source, target, new CopyToOptions
            {
                PropertiesToIgnore = propertiesToIgnore.Select(i => ValidateProperty(i)).Distinct()
            });
        }

        private static PropertyInfo ValidateProperty<TSource, TProperty>(Expression<Func<TSource, TProperty>> propertyLambda)
        {
            Type type = typeof(TSource);

            MemberExpression member = propertyLambda.Body as MemberExpression;
            if (member == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a method, not a property.",
                    propertyLambda.ToString()));

            PropertyInfo propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a field, not a property.",
                    propertyLambda.ToString()));

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(string.Format(
                    "Expression '{0}' refers to a property that is not from type {1}.",
                    propertyLambda.ToString(),
                    type));

            return propInfo;
        }

        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <param name="source">The base class instance.</param>
        /// <param name="target">The target.</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static void CopyTo(this object source, object target, CopyToOptions options)
        {
            var sourceType = source.GetType();

            var sourceProperties = Cache.GetOrAdd(
                $"pCopy->{sourceType.FullName}->{options.IgnoreNulls}",
                () => GetProperty(!options.CopyPrivates, sourceType));

            var targetType = target.GetType();

            var targetProperties = Cache.GetOrAdd(
                $"pCopy->{targetType.FullName}->Properties->{options.IgnoreNulls}",
                () => GetProperty(!options.CopyPrivates, targetType)).ToDictionary(i => i.Name);

            foreach (var propertyInfo in sourceProperties.Except(options.PropertiesToIgnore))
            {
                var value = options.Resolver.Invoke(new CopyContext(propertyInfo, source, target));

                if (options.IgnoreNulls && value == null)
                    continue;

                targetProperties[propertyInfo.Name].SetValue(target, value);
            }
        }

        private static PropertyInfo[] GetProperty(bool ignorePrivate, Type type)
        {
            return ignorePrivate
                ? type.GetProperties()
                : type
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
            if (type == null || type.BaseType == null) return new List<Type> { type };

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