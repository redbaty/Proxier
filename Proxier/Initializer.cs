using Proxier.Mappers;

namespace Proxier
{
    /// <summary>
    /// Happens when this module is loaded ( Thanks to fody! )
    /// </summary>
    public static class ModuleInitializer
    {
        /// <summary>
        /// Initialize the mapper
        /// </summary>
        public static void Initialize()
        {
            Mapper.InitializeMapperClasses();
        }
    }
}