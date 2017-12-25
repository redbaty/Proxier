using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using AttributeBuilder.Standard;

namespace Proxier.Mappers
{
    /// <summary>
    ///     Mapper extensions
    /// </summary>
    public static class MapperExtensions
    {
        /// <summary>
        ///     Copies object to another object using reflection.
        /// </summary>
        /// <param name="baseClassInstance">The base class instance.</param>
        /// <param name="target">The target.</param>
        /// <returns></returns>
        public static object CopyTo(this object baseClassInstance, object target)
        {
            foreach (var propertyInfo in baseClassInstance.GetType().GetHighestProperties().Select(i => i.PropertyInfo))
                try
                {
                    var value = propertyInfo.GetValue(baseClassInstance, null);
                    var highEquiv = target.GetType().GetHighestProperty(propertyInfo.Name);

                    if (null != value)
                        highEquiv.SetValue(target, value, null);
                }
                catch
                {
                    // ignored
                }

            return target;
        }

        /// <summary>
        ///     Gets the a injected version of a object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The object.</param>
        /// <returns></returns>
        public static T GetInjectedObject<T>(this T obj) where T : class
        {
            if (!Mapper.TypesOverrides.ContainsKey(obj.GetType())) return obj;
            return (T) obj.CopyTo(Mapper.TypesOverrides[obj.GetType()].Spawn());
        }

        /// <summary>
        ///     Finds a parent mapper from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static T FindOverridableType<T>(this Type type) where T : AttributeMapper
        {
            if (Mapper.TypesOverrides.ContainsKey(type))
                return (T) Mapper.TypesOverrides[type];

            var injType = type.GetAllBaseTypes()
                .FirstOrDefault(allBaseType => Mapper.TypesOverrides.ContainsKey(allBaseType));

            return (T) (injType != null ? Mapper.TypesOverrides[injType] : null);
        }

        /// <summary>
        ///     Finds a parent mapper from a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static AttributeMapper FindOverridableType(this Type type)
        {
            return FindOverridableType<AttributeMapper>(type);
        }

        /// <summary>
        ///     Gets the injected version of a type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Type GetInjectedType(this Type type)
        {
            var mapper = type.FindOverridableType();
            if (mapper == null) return type;

            var props = mapper.Mappings.Where(i => i.PropertyInfo != null).GroupBy(i => i.PropertyInfo).Select(i => new
            {
                Expressions = i.SelectMany(o => o.Expression),
                i.Key
            });

            type = props.Aggregate(type,
                (current, expression) =>
                    current.InjectPropertyAttributes(expression.Key, expression.Expressions.ToArray()));

            if (mapper.Mappings.All(i => i.PropertyInfo != null))
                return type;

            return mapper.Mappings.Where(i => i.PropertyInfo == null).Aggregate(type,
                (current, expression) => current.InjectClassAttributes(expression.Expression));
        }

        /// <summary>
        ///     Gets if type has a parameterless constructor
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool HasParameterlessContructor(this Type type)
        {
            return type.GetConstructor(Type.EmptyTypes) != null;
        }

        /// <summary>
        ///     Adds a parameterless constructor.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static Type AddParameterlessConstructor(this Type type)
        {
            var constructor = type.GetConstructor(Type.EmptyTypes);

            if (constructor != null)
                return type;

            var moduleBuilder = ModuleBuilder();
            var typeBuilder = moduleBuilder.DefineType(type.Name + "ctor", TypeAttributes.Public, type);

            var constructorBuilder =
                typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            var cGen = constructorBuilder.GetILGenerator();
            cGen.Emit(OpCodes.Nop);
            cGen.Emit(OpCodes.Ret);

            return typeBuilder.CreateTypeInfo().AsType();
        }

        /// <summary>
        ///     Generate a module builder.
        /// </summary>
        /// <returns></returns>
        private static ModuleBuilder ModuleBuilder()
        {
            var assemblyBuilder =
                AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("ProxyBuilder"),
                    AssemblyBuilderAccess.Run);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule("ProxyBuilder");
            return moduleBuilder;
        }

        /// <summary>
        ///     Injects attributes to a class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="expressions">The expressions.</param>
        /// <returns></returns>
        /// <exception cref="Exception">Type is not a class, cannot inject.</exception>
        public static Type InjectClassAttributes(this Type type, params Expression<Func<Attribute>>[] expressions)
        {
            if (!type.IsClass)
                throw new Exception("Type is not a class, cannot inject.");

            var moduleBuilder = ModuleBuilder();
            var typeBuilder = moduleBuilder.DefineType(type.Name + "Proxy", TypeAttributes.Public, type);
            var constructor = type.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
            {
                var constructorBuilder =
                    typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Any, Type.EmptyTypes);
                var cGen = constructorBuilder.GetILGenerator();
                cGen.Emit(OpCodes.Nop);
                cGen.Emit(OpCodes.Ret);
            }

            foreach (var expression in expressions)
                typeBuilder.SetCustomAttribute(expression);

            return typeBuilder.CreateTypeInfo().AsType();
        }

        /// <summary>
        ///     Gets all base types.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public static IEnumerable<Type> GetAllBaseTypes(this Type type)
        {
            if (type == null || type.BaseType == null)
                return new List<Type> {type};

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
        ///     Injects the property attributes.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="propInfo">The property information.</param>
        /// <param name="expressions">The expressions.</param>
        /// <returns></returns>
        public static Type InjectPropertyAttributes(this Type type, PropertyInfo propInfo,
            params Expression<Func<Attribute>>[] expressions)
        {
            type = type.AddParameterlessConstructor();
            var moduleBuilder = ModuleBuilder();
            var typeBuilder = moduleBuilder.DefineType(type.Name + "Proxy", TypeAttributes.Public, type);
            var custNamePropBldr = propInfo.Name.CreateProperty(typeBuilder, propInfo.PropertyType);
            foreach (var expression in expressions)
                custNamePropBldr.SetCustomAttribute(expression);
            return typeBuilder.CreateTypeInfo().AsType();
        }

        /// <summary>
        ///     Inject a new property into a type
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static Type InjectProperty(this Type type, string name, Type propertyType)
        {
            type = type.AddParameterlessConstructor();
            var moduleBuilder = ModuleBuilder();
            var typeBuilder = moduleBuilder.DefineType(type.Name + "prop", TypeAttributes.Public, type);
            name.CreateProperty(typeBuilder, propertyType);
            return typeBuilder.CreateTypeInfo().AsType();
        }

        /// <summary>
        ///     Create a property on a typeBuilder
        /// </summary>
        /// <param name="name"></param>
        /// <param name="typeBuilder"></param>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static PropertyBuilder CreateProperty(this string name, TypeBuilder typeBuilder, Type propertyType)
        {
            var custNamePropBldr = typeBuilder.DefineProperty(name,
                PropertyAttributes.HasDefault,
                propertyType,
                null);

            var customerNameBldr = typeBuilder.DefineField($"{name}_proxy_filter",
                propertyType,
                FieldAttributes.Private);

            const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName |
                                                MethodAttributes.HideBySig;

            var custNameGetPropMthdBldr =
                typeBuilder.DefineMethod($"get_{name}",
                    getSetAttr,
                    propertyType,
                    Type.EmptyTypes);

            var custNameGetIl = custNameGetPropMthdBldr.GetILGenerator();

            custNameGetIl.Emit(OpCodes.Ldarg_0);
            custNameGetIl.Emit(OpCodes.Ldfld, customerNameBldr);
            custNameGetIl.Emit(OpCodes.Ret);

            var custNameSetPropMthdBldr =
                typeBuilder.DefineMethod($"set_{name}",
                    getSetAttr,
                    null,
                    new[] {propertyType});

            var custNameSetIl = custNameSetPropMthdBldr.GetILGenerator();
            custNameSetIl.Emit(OpCodes.Ldarg_0);
            custNameSetIl.Emit(OpCodes.Ldarg_1);
            custNameSetIl.Emit(OpCodes.Stfld, customerNameBldr);
            custNameSetIl.Emit(OpCodes.Ret);
            custNamePropBldr.SetGetMethod(custNameGetPropMthdBldr);
            custNamePropBldr.SetSetMethod(custNameSetPropMthdBldr);
            return custNamePropBldr;
        }

        /// <summary>
        ///     Get a propertyInfo using lambdas.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="source"></param>
        /// <param name="propertyLambda"></param>
        /// <returns></returns>
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            this TSource source,
            Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var type = typeof(TSource);

            if (!(propertyLambda.Body is MemberExpression member))
                throw new ArgumentException(
                    $"Expression '{propertyLambda}' refers to a method, not a property.");

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
                throw new ArgumentException(
                    $"Expression '{propertyLambda}' refers to a field, not a property.");

            if (type != propInfo.ReflectedType &&
                !type.IsSubclassOf(propInfo.ReflectedType))
                throw new ArgumentException(
                    $"Expresion '{propertyLambda}' refers to a property that is not from type {type}.");

            return propInfo;
        }
    }
}