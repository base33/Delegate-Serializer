using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DelegateSerializer
{
    /// <summary>
    /// Serializes delegates for persistance that can be restored and called back in restful applications
    /// </summary>
    public class Serializer
    {
        /// <summary>
        /// Converts a delegate method reference to a string to be called later
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public string Serialize(Delegate del)
        {
            if (del == null)
                throw new ApplicationException("Delegate is not set");

            var methodInfo = del.GetMethodInfo();

            if (!methodInfo.IsPublic || !methodInfo.IsStatic)
                throw new ApplicationException("Delegate not referencing a public static method!  The referenced method must be marked public and static");

            var functionName = del.Method.Name;
            var className = del.Method.DeclaringType.FullName + "," + del.Method.DeclaringType.Assembly.FullName;
            return string.Format("{0}:{1}", className, functionName);
        }

        /// <summary>
        /// Converts a compiled string back to the original delegate ready to be called
        /// </summary>
        /// <param name="compiledFunction"></param>
        /// <returns></returns>
        public Delegate Deserialize<T>(string compiledDelegate)
        {
            if (string.IsNullOrEmpty(compiledDelegate) || !compiledDelegate.Contains(":"))
                throw new ApplicationException("The compiled delegate is invalid");

            var functionParts = compiledDelegate.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            var className = functionParts[0];
            var functionName = functionParts[1];

            Type classType = Type.GetType(className);
            MethodInfo methodInfo = classType.GetMethod(functionName, BindingFlags.Public | BindingFlags.Static);

            if (methodInfo == null)
                throw new ApplicationException("Method is not a public static method!  It must be marked public and static");

            return Delegate.CreateDelegate(typeof(T), methodInfo);
        }
    }
}
