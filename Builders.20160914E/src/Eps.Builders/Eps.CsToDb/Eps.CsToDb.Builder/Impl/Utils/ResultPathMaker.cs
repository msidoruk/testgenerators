using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Builder.DbModel;

namespace Eps.CsToDb.Builder.Impl.Utils
{
    public class ResultPathMaker
    {
        public static string BuildSchemaScriptPath(BuilderContext context, DbSchema dbSchema)
        {
            return $"{context.ResultPath}\\DbScripts\\Schemas\\{dbSchema.Name}.sql";
        }

        public static string BuildTableScriptPath(BuilderContext context, DbTable dbTable)
        {
            return $"{context.ResultPath}\\DbScripts\\Tables\\{dbTable.DbSchema.Name}\\{dbTable.Name}.sql";
        }
    }
}
