﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.Remoting;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Timers;

namespace ArghyaC.CSharpRunner
{
    //http://stackoverflow.com/questions/1357231/restrict-plugin-access-to-file-system-and-network-via-appdomain
    //[Serializable] << this was the ass ruining my life!!!**
    class UntrustedCodeProcessor : MarshalByRefObject
    {
        internal static T GetResult<T>(string untrustedAssemblyDirectory, string assemblyFullPath, string className, string methodName, object[] methodParameters, int timeoutMilli = 1000)
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

                //newDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", TimeSpan.FromMilliseconds(200));

                //Timer timer = new Timer(timeoutMilli);
                ////System.Threading.Timer timer = new System.Threading.Timer(new System.Threading.TimerCallback())
                ////timer.Elapsed += ((obj, evnt) => { throw new TimeoutException("Code execution timed out"); });
                ////timer.Elapsed += OnTimedEvent;
                //timer.AutoReset = false;
                //timer.Enabled = true;

                var details = newDomainInstance.ExecuteUntrustedCode<T>(assemblyFullPath, className, methodName, methodParameters, timeoutMilli);
                return details;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (newDomain != null)
                    AppDomain.Unload(newDomain); //otherwise root directory cannot be deleted!
            }
        }

        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                throw new TimeoutException("Code execution timed out");
            }
            catch (Exception exception)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(
                    _ => { throw new Exception("Code execution timed out", exception); });
            }
        }

        //VS is failing here!
        //private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        //{
        //    try
        //    {
        //        throw new TimeoutException("Code execution timed out");
        //    }
        //    catch (Exception exception)
        //    {
        //        System.Threading.ThreadPool.QueueUserWorkItem(
        //            _ => { throw new Exception("Code execution timed out", exception); });
        //    }
        //}

        private T ExecuteUntrustedCode<T>(string assemblyName, string typeName, string entryPoint, Object[] parameters, int timeoutMilli)
        {
            //Load the MethodInfo for a method in the new Assembly. This might be a method you know, or 
            //you can use Assembly.EntryPoint to get to the main function in an executable.

            var assembly = Assembly.LoadFrom(assemblyName);
            var type = assembly.GetType(typeName);
            if (type == null)
                throw new ApplicationException(string.Format("The type `{0}` was not found in code!", typeName));
            var method = type.GetMethod(entryPoint);
            if (method == null)
                throw new ApplicationException(string.Format("The method `{0}` was not found in code!", entryPoint));

            try
            {
                //Timer timer = new Timer(timeoutMilli);
                //timer.Elapsed += ((obj, evnt) => { throw new TimeoutException("Code execution timed out"); });
                //timer.AutoReset = false;
                //timer.Enabled = true;

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
                if (ex is SecurityException) // || ex is TargetInvocationException << even for nulls!!
                {
                    expMsg = "Some suspicious code was detected! :\n" + ex.ToString();
                }
                else
                {
                    expMsg = "Something didn't work right! :\n" + ex.ToString();
                }
                CodeAccessPermission.RevertAssert();
                throw new ApplicationException(expMsg);
            }
        }
    }
}
