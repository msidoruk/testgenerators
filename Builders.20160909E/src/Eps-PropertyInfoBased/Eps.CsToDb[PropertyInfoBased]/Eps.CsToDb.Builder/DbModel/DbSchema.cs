using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Builder.DbModel.Collections;

namespace Eps.CsToDb.Builder.DbModel
{
    public class DbSchema
    {
        public string Name { get; set; }
        public Type SourceType { get; set; }
        public DbTablesCollection SchemaTables { get; private set; }
        public string CreationScript { get; set; }

        public DbSchema()
        {
            SchemaTables = new DbTablesCollection();
        }
    }
}
