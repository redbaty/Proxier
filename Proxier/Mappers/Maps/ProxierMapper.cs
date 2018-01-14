using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Proxier.Extensions;

namespace Proxier.Mappers.Maps
{
    public class ProxierMapper
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

                var count = AppDomain.CurrentDomain.GetAssemblies().First(i => i.GetName().Name == "Proxier.Proxied")
                    .GetTypes().Count();
                return;
            }

            TypesOverrides.Clear();

            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetLoadableTypes()).Where(i =>
                i.IsClass && !i.ContainsGenericParameters && i.IsSubclassOf(typeof(T))).ToList();

            var mappers = types.Where(i => i.HasParameterlessContructor())
                .Select(Activator.CreateInstance).OfType<T>()
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
