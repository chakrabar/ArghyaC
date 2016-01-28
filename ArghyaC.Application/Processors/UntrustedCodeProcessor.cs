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
    //[Serializable] << this was the ass ruining my life!!!**
    public class UntrustedCodeProcessor : MarshalByRefObject
    {
        public static T GetResult<T>(string untrustedAssemblyDirectory, string assemblyFullPath, string className, string methodName, object[] methodParameters)
        {
            AppDomain newDomain = null;
            try
            {
                //Setting the AppDomainSetup. It is very important to set the ApplicationBase to a folder 
                //other than the one in which the sandboxer resides.
                AppDomainSetup adSetup = new AppDomainSetup();
                adSetup.ApplicationBase = Path.GetFullPath(untrustedAssemblyDirectory);

                //Setting the permissions for the AppDomain. We give the permission to execute and to 
                //read/discover the location where the untrusted code is loaded.
                PermissionSet permSet = new PermissionSet(PermissionState.None);
                permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
                permSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, untrustedAssemblyDirectory)); //** watch out

                //We want the sandboxer assembly's strong name, so that we can add it to the full trust list.
                StrongName fullTrustAssembly = typeof(UntrustedCodeProcessor).Assembly.Evidence.GetHostEvidence<StrongName>();
                //add strong name here http://stackoverflow.com/questions/8349573/getting-null-from-gethostevidence/10843860#10843860

                //Now we have everything we need to create the AppDomain, so let's create it.
                newDomain = AppDomain.CreateDomain("UntrustedCodeProcessor", null, adSetup, permSet, fullTrustAssembly);

                //Use CreateInstanceFrom to load an instance of the Sandboxer class into the
                //new AppDomain. 
                ObjectHandle handle = Activator.CreateInstanceFrom(
                    newDomain, typeof(UntrustedCodeProcessor).Assembly.ManifestModule.FullyQualifiedName,
                    typeof(UntrustedCodeProcessor).FullName
                    ); //** here it wants to be [Serializable]. made MarshalByRefObject as per http://stackoverflow.com/questions/29550474/sandboxing-untrusted-code-in-c-security-permissions-seem-not-working

                //Unwrap the new domain instance into a reference in this domain and use it to execute the 
                //untrusted code.
                UntrustedCodeProcessor newDomainInstance = (UntrustedCodeProcessor)handle.Unwrap();

                var details = newDomainInstance.ExecuteUntrustedCode<T>(assemblyFullPath, className, methodName, methodParameters);
                return details;
            }
            catch(Exception ex)
            {
                throw;
            }
            finally
            {
                if (newDomain != null)
                    AppDomain.Unload(newDomain); //otherwise root directory cannot be deleted!
            }            
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
            catch (Exception ex)
            {
                var expMsg = string.Empty;
                // When we print informations from a SecurityException extra information can be printed if we are 
                //calling it with a full-trust stack.
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                if (ex is SecurityException || ex is TargetInvocationException)
                {
                    expMsg = "SecurityException caught:\n" + ex.ToString();
                }
                else
                {
                    expMsg = "Exception :\n" + ex.ToString();
                }
                CodeAccessPermission.RevertAssert();
                throw new ApplicationException(expMsg);
            }
        }

        public static T GetResultQuickNeedsWork<T>(string untrustedAssemblyDirectory, string assemblyFullPath, string className, string methodName, object[] methodParameters)
        {
            try
            {
                AppDomainSetup adSetup = new AppDomainSetup();
                adSetup.ApplicationBase = Path.GetFullPath(untrustedAssemblyDirectory);

                PermissionSet permSet = new PermissionSet(PermissionState.None);
                permSet.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
                permSet.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, untrustedAssemblyDirectory)); //** watch out

                AppDomain newDomain = AppDomain.CreateDomain("UntrustedCodeProcessor", null, adSetup, permSet, null);

                var objectHandler = newDomain.CreateInstanceFrom(assemblyFullPath, className);

                var methodInfo = objectHandler.Unwrap().GetType().GetMethod(methodName);
                T retVal = (T)methodInfo.Invoke(null, methodParameters);
                return retVal;
            }
            catch (Exception ex)
            {
                var expMsg = string.Empty;
                // When we print informations from a SecurityException extra information can be printed if we are 
                //calling it with a full-trust stack.
                (new PermissionSet(PermissionState.Unrestricted)).Assert();
                if (ex is SecurityException || ex is TargetInvocationException)
                {
                    expMsg = "SecurityException caught:\n" + ex.ToString();
                }
                else
                {
                    expMsg = "Exception :\n" + ex.ToString();
                }
                CodeAccessPermission.RevertAssert();
                throw new ApplicationException(expMsg);
            }
        }
    }
}
