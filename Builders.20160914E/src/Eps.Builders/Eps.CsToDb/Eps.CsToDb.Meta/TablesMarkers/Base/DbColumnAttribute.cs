using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Meta.TablesMarkers.Base
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DbColumnAttribute : Attribute
    {
        public DbColumnAttribute(SqlDbType columnType)
        {
            ColumnType = columnType;
            IsNullable = true;
        }

        public string Name { get; set; }
        public SqlDbType ColumnType { get; set; }
        public int ColumnSize { get; set; }
        public bool IsNullable { get; set; }
        public bool IsIdentity { get; set; }
    }
}
