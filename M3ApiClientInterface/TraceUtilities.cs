using System;
using System.Diagnostics;
using System.Reflection;


namespace M3ApiClientInterface
{
    public static class TraceUtilities
    {
        public static void WriteMethodError(MethodBase methodBase)
        {
            Trace.WriteLine(string.Format("An error occured within '{0}.{1}.{2}()'.", methodBase.ReflectedType.Namespace, methodBase.ReflectedType.Name, methodBase.Name));
        }
    }
}