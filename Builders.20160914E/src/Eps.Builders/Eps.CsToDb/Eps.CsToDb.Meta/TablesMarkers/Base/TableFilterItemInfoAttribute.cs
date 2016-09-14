using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Meta.TablesMarkers.Base
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TableFilterItemInfoAttribute : Attribute
    {
        public Type TableType { get; set; }
        public string ColumnName { get; set; }
    }
}
