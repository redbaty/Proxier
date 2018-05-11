using System;
using System.Linq;
using System.Reflection;

namespace Proxier.Repositories
{
    internal class TypeRepository
    {
        public PropertyInfo[] GetProperty(Type type, bool ignorePrivate)
        {
            return ignorePrivate
                ? type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(
                    i => i.PropertyType.Namespace != null && i.PropertyType.Namespace.StartsWith("System") || i.PropertyType.IsEnum).ToArray()
                : type
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(
                        i => i.PropertyType.Namespace != null && i.PropertyType.Namespace.StartsWith("System") || i.PropertyType.IsEnum).ToArray();
        }
    }
}