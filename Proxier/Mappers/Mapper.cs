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
        /// <summary>
        ///     Initializes a new instance of the <see cref="Mapper" /> class.
        /// </summary>
        /// <param name="parent">The parent.</param>
        /// <exception cref="ArgumentNullException">parent</exception>
        public Mapper(AttributesMapper parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        ///     Global mapping overrides
        /// </summary>
        public static Dictionary<Type, AttributesMapper> TypesOverrides { get; set; } =
            new Dictionary<Type, AttributesMapper>();

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
        public AttributesMapper Parent { get; set; }

        /// <summary>
        ///     Initializes the mapper classes.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void InitializeIMapperClasses(IKernel kernel = null)
        {
            TypesOverrides.Clear();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(i =>
                i.IsClass && !i.ContainsGenericParameters && i.IsSubclassOf(typeof(AttributesMapper))).ToList();

            var mappers = types.Select(i => kernel?.Get(i) ?? Activator.CreateInstance(i)).OfType<AttributesMapper>()
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