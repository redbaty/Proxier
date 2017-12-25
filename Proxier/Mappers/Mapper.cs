using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ninject;

namespace Proxier.Mappers
{
    /// <summary>
    ///     The mapper class
    /// </summary>
    public class Mapper
    {
        static Mapper()
        {
            InitializeMapperClasses();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Mapper" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <exception cref="ArgumentNullException">parent</exception>
        public Mapper(AttributeMapper parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        ///     Global mapping overrides
        /// </summary>
        public static Dictionary<Type, AttributeMapper> TypesOverrides { get; } =
            new Dictionary<Type, AttributeMapper>();

        /// <summary>
        ///     This mapper attribute expression
        /// </summary>
        public Expression<Func<Attribute>>[] Expression { get; set; }

        /// <summary>
        ///     This mapper property info
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        ///     Gets or sets the parent.
        /// </summary>
        /// <value>
        ///     The parent.
        /// </value>
        public AttributeMapper Parent { get; }

        /// <summary>
        ///     Initializes the mapper classes (AttributeMapper).
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void InitializeMapperClasses(IKernel kernel = null)
        {
            InitializeMapperClasses<AttributeMapper>(kernel);
        }

        /// <summary>
        ///     Initializes the mapper classes from a certain type.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void InitializeMapperClasses<T>(IKernel kernel = null) where T : AttributeMapper
        {
            TypesOverrides.Clear();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(i =>
                i.IsClass && !i.ContainsGenericParameters && i.IsSubclassOf(typeof(T))).ToList();

            var mappers = types.Where(i => i.HasParameterlessContructor())
                .Select(i => kernel?.Get(i) ?? Activator.CreateInstance(i)).OfType<T>()
                .ToList();

            foreach (var type in mappers)
            {
                if (kernel != null)
                {
                    type.Kernel = kernel;
                    type.OnKernelLoaded();
                }

                TypesOverrides.Add(type.BaseType, type);
            }
        }
    }
}