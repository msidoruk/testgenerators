using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Builder.DbModel.Collections;

namespace Eps.CsToDb.Builder.DbModel
{
    public class DbModel
    {
        public DbSchemasCollection DbSchemas { get; private set; }
        public DbTablesCollection DbTables { get; private set; }
        
        private Dictionary<string, DbTable> _dbTablesByNames = new Dictionary<string, DbTable>(StringComparer.InvariantCultureIgnoreCase);
        private Dictionary<string /*DbModelType Name*/, DbTable> _dbModelTypeToDbTableMapping = new Dictionary<string, DbTable>(StringComparer.InvariantCultureIgnoreCase);

        public DbModel()
        {
            DbSchemas = new DbSchemasCollection();
            DbTables = new DbTablesCollection();
        }

        public DbTable AppendDbTable(string dbTableName, Type dbModelType)
        {
            DbTable newDbTable = new DbTable() {Name = dbTableName};
            DbTables.AddTable(newDbTable);
            newDbTable.SourceType = dbModelType;
            //
            _dbTablesByNames[newDbTable.Name] = newDbTable;
            _dbModelTypeToDbTableMapping[dbModelType.FullName] = newDbTable;
            //
            return newDbTable;
        }

        public DbSchema FindDbSchemaOrCreateOne(string dbSchemaName)
        {
            foreach (var dbSchema in DbSchemas)
            {
                if (dbSchema.Name.Equals(dbSchemaName, StringComparison.InvariantCultureIgnoreCase))
                    return dbSchema;
            }
            DbSchema newDbSchema = new DbSchema() {Name = dbSchemaName};
            DbSchemas.AddSchema(newDbSchema);
            return newDbSchema;
        }

        public DbTable GetModelTypeDbTable(Type dbModelType)
        {
            DbTable resultDbTable;
            _dbModelTypeToDbTableMapping.TryGetValue(dbModelType.FullName, out resultDbTable);
            return resultDbTable;
        }
    }
}
