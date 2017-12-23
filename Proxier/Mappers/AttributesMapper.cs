using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Ninject;

namespace Proxier.Mappers
{
    /// <summary>
    ///     Mapper abstraction
    /// </summary>
    public class AttributesMapper
    {
        /// <inheritdoc />
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:Proxier.Mappers.AttributesMapper" /> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public AttributesMapper(Type type) : this()
        {
            BaseType = type;
            Type = type;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AttributesMapper" /> class.
        /// </summary>
        public AttributesMapper()
        {
            Parent = this;
        }

        /// <summary>
        ///     Automatically hides elements that are not defined already.
        /// </summary>
        public bool AutoHide { get; set; } = false;

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
        public AttributesMapper Parent { get; set; }

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
        public List<Mapper> Mappings { get; } = new List<Mapper>();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public object Spawn()
        {
            return TransfomSpawn(Activator.CreateInstance(BaseType.GetInjectedType().AddParameterlessConstructor()));
        }

        internal virtual void HandleAction(object model, string action, object parameter)
        {
        }

        /// <summary>
        ///     Transfoms the spawn method.
        /// </summary>
        /// <param name="createInstance">The create instance.</param>
        /// <returns></returns>
        public virtual object TransfomSpawn(object createInstance)
        {
            Kernel?.Inject(createInstance);
            return createInstance;
        }

        /// <summary>
        ///     Called when [kernel loaded].
        /// </summary>
        public virtual void OnKernelLoaded()
        {
        }
    }

    /// <summary>
    ///     Mapper abstractions
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    public class AttributesMapper<TSource> : AttributesMapper
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AttributesMapper{TSource}" /> class.
        /// </summary>
        public AttributesMapper() : base(typeof(TSource))
        {
        }

        /// <summary>
        ///     Called when [action happened]
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="action">The action.</param>
        /// <param name="parameter">The parameter.</param>
        public virtual void Action(TSource model, string action, object parameter)
        {
        }

        /// <summary>
        ///     Handles the action.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="action">The action.</param>
        /// <param name="parameter">The parameter.</param>
        internal override void HandleAction(object model, string action, object parameter)
        {
            Action((TSource) model.CopyTo(Activator.CreateInstance(Type.AddParameterlessConstructor())), action,
                parameter);
        }

        /// <summary>
        ///     Add an attribute to a class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public void AddClassAttribute(params Expression<Func<Attribute>>[] expression)
        {
            var mapper = new Mapper(this)
            {
                PropertyInfo = null,
                Expression = expression
            };

            Mappings.Add(mapper);
        }

        /// <summary>
        ///     Adds a mapper by name.
        /// </summary>
        public void AddProperty(string prop, Type type)
        {
            Type = Type.InjectProperty(prop, type);
        }

        /// <summary>
        ///     Adds a mapper by name.
        /// </summary>
        public void AddPropertyAttribute(string prop,
            params Expression<Func<Attribute>>[] expression)
        {
            Mappings.Add(new Mapper(this)
            {
                Expression = expression,
                PropertyInfo = Type.GetHighestProperty(prop)
            });
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

            var mapper = new Mapper(this)
            {
                Expression = expression,
                PropertyInfo = propInfo
            };

            Mappings.Add(mapper);
        }
    }
}