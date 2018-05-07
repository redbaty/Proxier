using System;
using System.Reflection;

namespace Proxier.Repositories
{
    internal class TypeRepository
    {
        public PropertyInfo[] GetProperty(Type type, bool ignorePrivate)
        {
            return ignorePrivate
                ? type.GetProperties()
                : type
                    .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}