using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Builder.DbModel.Collections;

namespace Eps.CsToDb.Builder.DbModel
{
    public class DbTable
    {
        public DbSchema DbSchema { get; set; }
        public string Name { get; set; }
        public DbColumnsCollection DbColumns { get; private set; }
        public PrimaryKey PrimaryKey { get; private set; }
        public ForeignKeysCollection ForeignKeys { get; private set; }
        public Type SourceType { get; set; }
        public string CreationScript { get; set; }

        public DbTable()
        {
            PrimaryKey = new PrimaryKey();
            DbColumns = new DbColumnsCollection();
            ForeignKeys = new ForeignKeysCollection();
        }

        public DbColumn AppendColumn(string columnName, SqlDbType columnType, int columnSize, bool isNullable)
        {
            if (DbColumns.FindColumn(columnName) != null)
                throw new Exception($"Table '{Name}' of DbModelType '{SourceType.FullName}' already has column named '{columnName}'");

            DbColumn newDbColumn = new DbColumn()
            {
                DbTable = this,
                Name = columnName,
                ColumnType = columnType,
                ColumnSize = columnSize,
                IsNullable = isNullable
            };
            DbColumns.AddColumn(newDbColumn);

            return newDbColumn;
        }
    }
}
