using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Builder.Common;
using Eps.CsToDb.Meta;

namespace Eps.CsToDb.Builder.Impl
{
    public class DbModelAssembliesResolver : IDbModelAssembliesResolver
    {
        public void Resolve(string dbModelAssembliesSearchPath, Func<Assembly, bool> registerModelAssembly)
        {
            DirectoryFilesLister.List(dbModelAssembliesSearchPath,
                                        filePath =>
                                        {
                                            try
                                            {
                                                // log file cannot be accesses and it leads to problems in debugging, so we'll check file for access first.
                                                if (!FileTools.IsFileAccessible(filePath))
                                                    return true;
                                                //
                                                AssemblyName assemblyName = AssemblyName.GetAssemblyName(filePath);
                                                Assembly assembly = Assembly.Load(assemblyName);
                                                object[] attributes = assembly.GetCustomAttributes(typeof(DbModelPartAttribute), false);
                                                DbModelPartAttribute attribute = (attributes.Length > 0) ? attributes[0] as DbModelPartAttribute : null;
                                                if (attribute != null)
                                                    registerModelAssembly(assembly);
                                            }
                                            catch (BadImageFormatException)
                                            {
                                                // It's ok we just checked that we found an assembly.
                                            }
                                            catch (IOException)
                                            {
                                                // It's ok we just checked that we found an assembly.
                                            }
                                            return true;
                                        });
        }
    }
}
