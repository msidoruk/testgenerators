using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Builder.DbModel
{
    public class DbColumn
    {
        public string Name { get; set; }
        public SqlDbType ColumnType { get; set; }
        public int ColumnSize { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
        public PropertyInfo SourceProperty { get; set; }
    }
}
