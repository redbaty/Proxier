using System.Reflection;

namespace Proxier.Benchmark
{
    // ReSharper disable once UnusedMember.Global
    public static class MethodTimeLogger
    {
        public static void Log(MethodBase methodBase, long milliseconds)
        {
            Serilog.Log.Logger.Information($"{methodBase.Name} took {milliseconds}ms");
        }
    }
}