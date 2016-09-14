using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Builder.DbModel
{
    public class ForeignKey
    {
        public DbTable SourceTable { get; set; }
        public List<DbColumn> ReferencingColumns { get; private set; }
        public DbTable ReferencedTable { get; set; }
        public List<DbColumn> ReferencedColumns { get; private set; }
        public bool IsSuppressed { get; set; }
        public string ConstraintName { get; set; }

        public ForeignKey()
        {
            ReferencingColumns = new List<DbColumn>();
            ReferencedColumns = new List<DbColumn>();
        }
    }
}
