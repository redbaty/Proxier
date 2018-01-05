using Proxier.Mappers;

namespace Proxier
{
    public static class ModuleInitializer
    {
        public static void Initialize()
        {
            Mapper.InitializeMapperClasses();
        }
    }
}