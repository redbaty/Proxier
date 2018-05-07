using System.Reflection;
using Proxier.Managers;

namespace Proxier.Repositories
{
    internal class ClassBuilderRepository
    {
        public Assembly GenerateAssembly(string code)
        {
            return CodeManager.GenerateAssembly(code).Result;
        }
    }
}