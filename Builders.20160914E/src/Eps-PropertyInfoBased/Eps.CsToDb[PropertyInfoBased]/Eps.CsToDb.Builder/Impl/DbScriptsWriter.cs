using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Builder.Common;
using Eps.CsToDb.Builder.Impl.Utils;

namespace Eps.CsToDb.Builder.Impl
{
    public class DbScriptsWriter : IDbScriptsWriter
    {
        private BuilderContext _context;

        public void Write(BuilderContext context)
        {
            _context = context;
            //
            foreach (var dbSchema in context.DbModel.DbSchemas)
            {
                string schemaScriptPath = ResultPathMaker.BuildSchemaScriptPath(context, dbSchema);
                //
                bool overriden;
                FileTools.WriteTextFileIfChanged(schemaScriptPath, dbSchema.CreationScript, out overriden);
                if (overriden)
                    context.Logger.Debug($"Schema script '{schemaScriptPath}' was overriden");
                else
                    context.Logger.Debug($"Schema script '{schemaScriptPath}' is not changed");
            }
            //
            foreach (var dbSchema in context.DbModel.DbSchemas)
            {
                foreach (var dbTable in dbSchema.SchemaTables)
                {
                    string tableScriptPath = ResultPathMaker.BuildTableScriptPath(context, dbTable);
                    //
                    bool overriden;
                    FileTools.WriteTextFileIfChanged(tableScriptPath, dbTable.CreationScript, out overriden);
                    if (overriden)
                        context.Logger.Debug($"Table script '{tableScriptPath}' was overriden");
                    else
                        context.Logger.Debug($"Table script '{tableScriptPath}' is not changed");
                }
            }
        }
    }
}
