using System;
using System.IO;

namespace ArghyaC.Infrastructure.Utilities
{
    public class FileUtilities
    {
        public static T TempCreateDirAndExecute<T>(string tempDirectoryPath, Func<T> func)
        {
            if (!Directory.Exists(tempDirectoryPath))
                Directory.CreateDirectory(tempDirectoryPath);

            T result = func();

            if (Directory.Exists(tempDirectoryPath))
            {
                foreach (var file in Directory.EnumerateFiles(tempDirectoryPath))
                {
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    System.IO.File.Delete(file);
                }
                Directory.Delete(tempDirectoryPath, true);
            }
            return result;
        }
    }
}
