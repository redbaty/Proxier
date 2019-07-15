using System;
using System.Linq;
using System.Reflection;

namespace Proxier.Repositories
{
    internal class TypeRepository
    {
        public PropertyInfo[] GetProperty(Type type, bool ignorePrivate)
        {
            var propertyInfos = ignorePrivate
                ? type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                : type
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            return propertyInfos.Where(
                i => i.CanRead && i.CanWrite).ToArray();
        }
    }
}