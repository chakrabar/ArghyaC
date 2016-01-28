using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;

namespace ArghyaC.Application.Processors
{
    //http://stackoverflow.com/questions/1357231/restrict-plugin-access-to-file-system-and-network-via-appdomain
    [Serializable]
    public class UntrustedCodeProcessor
    {
        public static T GetResult<T>(string untrustedAssemblyDirectory, string assemblyFullPath, string className, string methodName, object[] methodParameters)
        {
            //Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder 
            //other than the one in which the sandboxer resides.
            AppDomainSetup adSetup = new AppDomainSetup();
            adSetup.ApplicationBase = Path.GetFullPath(untrustedAssemblyDirectory);

            //Setting the permissions for the AppDomain. We give the permission to execute and to 
            //read/discover the location where the untrusted code is loaded.
            PermissionSet permSet = new PermissionSet(PermissionState.None);
            permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
            StrongName fullTrustAssembly = typeof(UntrustedCodeProcessor).Assembly.Evidence.GetHostEvidence<StrongName>();

            //Now we have everything we need to create the AppDomain, so let's create it.
            AppDomain newDomain = AppDomain.CreateDomain("UntrustedCodeProcessor", null, adSetup, permSet, fullTrustAssembly);

            //Use CreateInstanceFrom to load an instance of the Sandboxer class into the
            //new AppDomain. 
            ObjectHandle handle = Activator.CreateInstanceFrom(
                newDomain, typeof(UntrustedCodeProcessor).Assembly.ManifestModule.FullyQualifiedName,
                typeof(UntrustedCodeProcessor).FullName
                );
            //Unwrap the new domain instance into a reference in this domain and use it to execute the 
            //untrusted code.
            UntrustedCodeProcessor newDomainInstance = (UntrustedCodeProcessor)handle.Unwrap();
            return newDomainInstance.ExecuteUntrustedCode<T>(assemblyFullPath, className, methodName, methodParameters);
        }

        private T ExecuteUntrustedCode<T>(string assemblyName, string typeName, string entryPoint, Object[] parameters)
        {
            //Load the MethodInfo for a method in the new Assembly. This might be a method you know, or 
            //you can use Assembly.EntryPoint to get to the main function in an executable.
            var assembly = Assembly.LoadFrom(assemblyName);
            var method = assembly.GetType(typeName).GetMethod(entryPoint);
            try
            {
                //Now invoke the method.
                T retVal = (T)method.Invoke(null, parameters);
                return retVal;
            }
            catch (AccessViolationException ex)
            {
                var expMsg = string.Empty;
                // When we print informations from a SecurityException extra information can be printed if we are 
                //calling it with a full-trust stack.
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                expMsg = "SecurityException caught:\n{0}" + ex.ToString();
                CodeAccessPermission.RevertAssert();
                throw new AccessViolationException(expMsg);
            }
            catch (Exception ex)
            {
                var expMsg = string.Empty;
                // When we print informations from a SecurityException extra information can be printed if we are 
                //calling it with a full-trust stack.
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                expMsg = "Exception :\n{0}" + ex.ToString();
                CodeAccessPermission.RevertAssert();
                throw new ApplicationException(expMsg);
            }
        }
    }
}
