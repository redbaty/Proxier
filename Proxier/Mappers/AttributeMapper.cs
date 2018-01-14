using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Ninject;
using Proxier.Extensions;
using Proxier.Mappers.Maps;

namespace Proxier.Mappers
{
    /// <summary>
    ///     Mapper abstraction
    /// </summary>
    public class AttributeMapper
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Proxier.Mappers.AttributeMapper" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public AttributeMapper(Type type) : this()
        {
            BaseType = type;
            Type = type;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AttributeMapper" /> class.
        /// </summary>
        public AttributeMapper()
        {
            Parent = this;
        }

        /// <summary>
        ///     Gets or sets the kernel.
        /// </summary>
        /// <value>
        ///     The kernel.
        /// </value>
        public IKernel Kernel { get; set; }

        /// <summary>
        ///     Gets or sets the parent.
        /// </summary>
        /// <value>
        ///     The parent.
        /// </value>
        public AttributeMapper Parent { get; set; }

        /// <summary>
        ///     Gets or sets the injected type.
        /// </summary>
        /// <value>
        ///     The type.
        /// </value>
        public Type Type { get; set; }

        /// <summary>
        ///     Gets the base type.
        /// </summary>
        /// <value>
        ///     The type of the base.
        /// </value>
        public Type BaseType { get; }


        /// <summary>
        ///     Gets the mappings.
        /// </summary>
        /// <value>
        ///     The mappings.
        /// </value>
        public List<AttributeMap> AttributeMappings { get; } = new List<AttributeMap>();
        
        /// <summary>
        /// Represents custom properties
        /// </summary>
        public List<PropertyMap> CustomProperties { get; } = new List<PropertyMap>();
        
        /// <summary>
        /// </summary>
        /// <returns></returns>
        public object Spawn()
        {
            TransfomSpawn(null);
            return Activator.CreateInstance(BaseType.GetInjectedType(true).AddParameterlessConstructor());
        }

        /// <summary>
        ///     Transfoms the spawn method.
        /// </summary>
        /// <param name="createInstance">The create instance.</param>
        /// <returns></returns>
        public virtual object TransfomSpawn(object createInstance)
        {
            if (createInstance != null)
                Kernel?.Inject(createInstance);
            return createInstance;
        }

        /// <summary>
        ///     Called when [kernel loaded].
        /// </summary>
        public virtual void OnKernelLoaded()
        {
        }

        /// <summary>
        ///     Add an attribute to a class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void AddClassAttribute(params Expression<Func<Attribute>>[] expression)
        {
            var mapper = new AttributeMap(this)
            {
                PropertyInfo = null,
                Attributes = expression
            };

            AttributeMappings.Add(mapper);
        }

        /// <summary>
        ///     Adds a mapper by name.
        /// </summary>
        public void AddProperty(string prop, Type type)
        {
            CustomProperties.Add(new PropertyMap(this, prop, type));
        }

        /// <summary>
        ///     Adds a mapper by name.
        /// </summary>
        public void AddPropertyAttribute(string prop,
            params Expression<Func<Attribute>>[] expression)
        {
            AttributeMappings.Add(new AttributeMap(this)
            {
                Attributes = expression,
                PropertyInfo = Type.GetHighestProperty(prop)
            });
        }
    }

    /// <inheritdoc />
    /// <summary>
    ///     Mapper abstractions
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class AttributeMapper<TSource> : AttributeMapper
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AttributeMapper{TSource}" /> class.
        /// </summary>
        public AttributeMapper() : base(typeof(TSource))
        {
        }

        /// <summary>
        ///     Adds a mapper.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <param name="propertyLambda"></param>
        public void AddPropertyAttribute<TProperty>(Expression<Func<TSource, TProperty>> propertyLambda,
            params Expression<Func<Attribute>>[] expression)
        {
            var type = Type;

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

            var mapper = new AttributeMap(this)
            {
                Attributes = expression,
                PropertyInfo = propInfo
            };

            AttributeMappings.Add(mapper);
        }
    }
}