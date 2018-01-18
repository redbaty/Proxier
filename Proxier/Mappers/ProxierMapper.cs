using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Proxier.Extensions;

namespace Proxier.Mappers.Maps
{
    /// <summary>
    /// Proxier manager
    /// </summary>
    public static class ProxierMapper
    {
        /// <summary>
        /// Global mapping overrides
        /// </summary>
        public static Dictionary<Type, AttributeMapper> TypesOverrides { get; } =
            new Dictionary<Type, AttributeMapper>();

        /// <summary>
        /// Initializes the mapper classes (AttributeMapper).
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void InitializeMapperClasses(IKernel kernel = null)
        {
            InitializeMapperClasses<AttributeMapper>(kernel);
        }

        /// <summary>
        /// Initializes the mapper classes from a certain type.
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        public static void InitializeMapperClasses<T>(IKernel kernel = null) where T : AttributeMapper
        {
            if (TypesOverrides.Count > 0)
            {
                foreach (var @override in TypesOverrides.Where(i => i.Value.Kernel != kernel))
                {
                    @override.Value.Kernel = kernel;
                    @override.Value.OnKernelLoaded();
                }

                return;
            }

            TypesOverrides.Clear();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetLoadableTypes()).Where(i =>
                i.IsClass && !i.ContainsGenericParameters && i.IsSubclassOf(typeof(T))).ToList();

            var mappers = types.Where(i => i.HasParameterlessContructor())
                .Select(Activator.CreateInstance).OfType<T>()
                .Where(i => i != null)
                .ToList();

            foreach (var mapper in mappers)
            {
                if (kernel != null)
                {
                    mapper.Kernel = kernel;
                    mapper.OnKernelLoaded();
                }

                if (!TypesOverrides.ContainsKey(mapper.BaseType))
                    TypesOverrides.Add(mapper.BaseType, mapper);
                else
                    TypesOverrides[mapper.BaseType].Merge(mapper);
            }
        }
    }
}
