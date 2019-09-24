using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Proxier.Contexts;
using Proxier.Repositories;

namespace Proxier.Extensions
{
    public static class CollectionExtensions
    {
        public static ICollection CastToCollectionType(this IEnumerable mopModels, params Type[] collectionType) =>
                (ICollection) Activator.CreateInstance(
                        typeof(List<>).MakeGenericType(collectionType),
                        typeof(Enumerable).GetMethod("Cast")
                                          ?.MakeGenericMethod(collectionType)
                                          .Invoke(null,
                                                  new object[]
                                                  {
                                                          mopModels
                                                  }));
    }

    /// <summary>
    ///     General Extensions
    /// </summary>
    public static class TypeExtensions
    {
        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source">The base class instance.</param>
        /// <returns></returns>
        public static IEnumerable<TTarget> ConvertTo<TTarget>(this IEnumerable source)
        {
            foreach (var src in source)
            {
                var instance = Activator.CreateInstance<TTarget>();
                CopyTo(src, instance, new CopyToOptions());
                yield return instance;
            }
        }

        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The base class instance.</param>
        /// <param name="newInstanceType"></param>
        /// <returns></returns>
        public static object CopyTo<TSource>(this TSource source, Type newInstanceType)
        {
            var instance = Activator.CreateInstance(newInstanceType);
            CopyTo(source, instance, new CopyToOptions());
            return instance;
        }

        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source">The base class instance.</param>
        /// <returns></returns>
        public static TTarget CopyTo<TTarget>(this object source)
            where TTarget : class
        {
            var instance = Activator.CreateInstance<TTarget>();
            CopyTo(source, instance, new CopyToOptions());
            return instance;
        }

        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The base class instance.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static void CopyTo<TSource>(this TSource source, object target)
        {
            CopyTo(source, target, new CopyToOptions());
        }

        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="source">The base class instance.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static TTarget CopyTo<TSource, TTarget>(this TSource source, TTarget target)
        {
            CopyTo(source, target, new CopyToOptions());
            return target;
        }

        /// <summary>
        ///     Copies object to another object ignoring some properties.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The base class instance.</param>
        /// <param name="target">The target.</param>
        /// <param name="propertiesToIgnore"></param>
        /// <returns></returns>
        public static void CopyTo<TSource>(this TSource source, object target,
            params string[] propertiesToIgnore)
        {
            CopyTo(source, target, new CopyToOptions
            {
                PropertiesToIgnore = typeof(TSource).GetProperties().Where(i => propertiesToIgnore.Contains(i.Name))
            });
        }

        /// <summary>
        ///     Copies object to another object ignoring some properties.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source">The base class instance.</param>
        /// <param name="target">The target.</param>
        /// <param name="propertiesToIgnore"></param>
        /// <returns></returns>
        public static void CopyTo<TSource>(this TSource source, object target,
            params Expression<Func<TSource, object>>[] propertiesToIgnore)
        {
            CopyTo(source, target, new CopyToOptions
            {
                PropertiesToIgnore = propertiesToIgnore.Select(ValidateProperty).Distinct()
            });
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
            
            var ignorePrivate = !options.CopyPrivates;

            var sourceProperties = options.PropertiesToInclude != null && options.PropertiesToInclude.Any()
                ? options.PropertiesToInclude
                : new TypeRepository().GetProperty(sourceType, ignorePrivate);

            var targetType = target.GetType();

            var targetProperties = options.PropertiesToInclude != null && options.PropertiesToInclude.Any()
                ? options.PropertiesToInclude.ToDictionary(i => i.Name)
                : new TypeRepository().GetProperty(targetType, ignorePrivate).ToDictionary(i => i.Name);

            foreach (var propertyInfo in sourceProperties.Except(options.PropertiesToIgnore ?? new List<PropertyInfo>())
            )
            {
                var value = options.Resolver.Invoke(new CopyContext(propertyInfo, source, target));

                if (options.IgnoreNulls && value == null)
                    continue;

                if (value is IEnumerable enumerable && propertyInfo.PropertyType.IsGenericType)
                {
                    var types = propertyInfo.PropertyType.GetGenericArguments();
                    value = enumerable.Cast<object>()
                                      .Select(i =>
                                      {
                                          var deepClone = DeepCloneObject(i);
                                          return deepClone;
                                      })
                                      .CastToCollectionType(types);
                }
                else
                {
                    value = DeepCloneObject(value);
                }

                if (targetProperties.ContainsKey(propertyInfo.Name))
                {
                    var targetProperty = targetProperties[propertyInfo.Name];

                    if (options.UseNullableBaseType)
                    {
                        var trueSourceType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ??
                                             propertyInfo.PropertyType;
                        var trueTargetType =
                            Nullable.GetUnderlyingType(targetProperty.PropertyType) ??
                            targetProperty.PropertyType;


                        if (!targetProperty.CanWrite)
                            continue;

                        if (trueSourceType == trueTargetType)
                            targetProperty.SetValue(target, value);
                        else if (options.TryToConvert && CanConvert(value, propertyInfo, targetProperty, out var x))
                            targetProperty.SetValue(target, x);
                    }
                    else
                    {
                        if (propertyInfo.PropertyType == targetProperty.PropertyType)
                            targetProperty.SetValue(target, value);
                        else if (options.TryToConvert && CanConvert(value, propertyInfo, targetProperty, out var x))
                            targetProperty.SetValue(target, x);
                    }
                }
            }
        }

        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">The base class instance.</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T source)
        {
            return (T) DeepCloneObject(source);
        }
        
        private static bool IsSimpleType(Type type)
        {
            return
                    type.IsPrimitive ||
                    new Type[] {
                            typeof(Enum),
                            typeof(String),
                            typeof(Decimal),
                            typeof(DateTime),
                            typeof(DateTimeOffset),
                            typeof(TimeSpan),
                            typeof(Guid)
                    }.Contains(type) ||
                    Convert.GetTypeCode(type) != TypeCode.Object ||
                    (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                    ;
        }
        
        private static object DeepCloneObject(this object source)
        {
            var sourceType = source?.GetType();
            if (source == null || IsSimpleType(sourceType)) return source;
            
            var instance = Activator.CreateInstance(sourceType);
            CopyTo(source, instance);
            return instance;
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

        private static bool CanConvert(object value, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo,
            out object obj)
        {
            obj = value;

            var trueSourceType = Nullable.GetUnderlyingType(sourcePropertyInfo.PropertyType) ??
                                 sourcePropertyInfo.PropertyType;

            var trueTargetSourceType = Nullable.GetUnderlyingType(targetPropertyInfo.PropertyType) ??
                                       targetPropertyInfo.PropertyType;

            if (trueSourceType == trueTargetSourceType) return true;

            try
            {
                var val = Convert.ChangeType(value, trueTargetSourceType);
                obj = val;
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static PropertyInfo ValidateProperty<TSource>(
            Expression<Func<TSource, object>> propertyLambda)
        {
            var type = typeof(TSource);

            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");

            if (propInfo.ReflectedType != null && type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(
                    $"Expression '{propertyLambda}' refers to a property that is not from type {type}.");

            return propInfo;
        }
    }
}