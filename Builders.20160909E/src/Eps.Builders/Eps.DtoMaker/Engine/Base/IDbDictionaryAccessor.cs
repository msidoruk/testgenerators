using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Helpers;

namespace EPS.DtoMaker.Engine.Base
{
    public class TableLink
    {
        public string SourceTableColumn { get; set; }
        public string ReferencedTableName { get; set; }
        public string ReferencedTableColumn { get; set; }
        public string ReferencedSchema { get; set; }

        public override string ToString()
        {
            return $"{SourceTableColumn ?? "?"} ==> {ReferencedSchema ?? "?"}.{ReferencedTableName ?? "?"}.{ReferencedTableColumn ?? "?"}";
        }
    }

    public class TableInfo
    {
        public string SchemaName { get; set; }
        public int SchemaId { get; set; }
        public string TableName { get; set; }
        public int TableId { get; set; }
        public Dictionary<int, DbTypeInfoParser> SystemTypes { get; set; }
        public DataTable TableColumns { get; set; }
        public List<TableLink> TableLinks { get; set; }
        public List<string> PKColumns { get; set; }
    }

    public class DbTypeInfoParser
    {
        public DataRow TypeInfoRow { get; private set; }
        public DbTypeInfoParser(DataRow typeInfoRow)
        {
            TypeInfoRow = typeInfoRow;
        }
        public string TypeName => (string)TypeInfoRow["name"];
    }

    interface IDbDictionaryAccessor
    {
        Tuple<bool, string> GetTableInfo(string schemaName, string tableName, out TableInfo tableInfo);
    }
}
