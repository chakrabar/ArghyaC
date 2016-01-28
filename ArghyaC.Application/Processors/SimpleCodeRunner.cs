using System;
using System.IO;
using System.Reflection;

namespace ArghyaC.Application.Processors
{
    public class SimpleCodeRunner
    {
        public T GetResult<T>(string assemblyPath, string fullyQualifiedClassName, string methodName, object[] methodParameters)
        {
            try
            {
                var assembly = Assembly.LoadFrom(assemblyPath);
                var type = assembly.GetType(fullyQualifiedClassName);
                var obj = Activator.CreateInstance(type);

                var methodInfo = type.GetMethod(methodName);
                T result = (T)methodInfo.Invoke(obj, methodParameters);
                return result;
            }
            catch(Exception ex)
            {
                //log ex
                return default(T);
            }
        }
    }
}
