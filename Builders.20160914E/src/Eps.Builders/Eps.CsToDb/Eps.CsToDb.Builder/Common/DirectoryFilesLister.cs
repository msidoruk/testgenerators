using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Builder.Common
{
    public class DirectoryFilesLister
    {
        public static void List(string path, Func<string, bool> filesListener)
        {
            DirectoryLister(path, filesListener);
        }

        private static bool DirectoryLister(string directory, Func<string, bool> filesListener)
        {
            if (Directory.Exists(directory))
            {
                Console.WriteLine("Scan: {0}", directory);

                if (Directory.GetFiles(directory).Any(fileName => !filesListener(fileName)))
                    return false;

                return Directory.GetDirectories(directory).All(currentSubDirectory => DirectoryLister(currentSubDirectory, filesListener));
            }
            return false;
        }
    }
}
