using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Builder.Common;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Builder.DbModel.Utils
{
    public class DbNamingRules
    {
        private BuilderContext _context;

        public DbNamingRules(BuilderContext context)
        {
            _context = context;
        }

        public string MakeTableName(DbTableAttribute dbTableAttribute, Type dbModelType)
        {
            if (dbTableAttribute != null && !string.IsNullOrWhiteSpace(dbTableAttribute.DbTableName))
                return dbTableAttribute.DbTableName;
            return dbModelType.Name;
        }

        public string MakeColumnName(DbColumnAttribute dbColumnAttribute, Type dbModelType, PropertyInfo propertyInfo)
        {
            string columnName;
            if (dbColumnAttribute != null && !string.IsNullOrEmpty(dbColumnAttribute.Name))
                columnName = dbColumnAttribute.Name;
            else
                columnName = propertyInfo.Name;
            //
            if (DbModelHelpers.IsItPrimaryKeyProperty(_context, dbModelType, propertyInfo) && columnName.Equals("KEY", StringComparison.InvariantCultureIgnoreCase))
            {
                string dbModelTypeNameSingularForm = ToSingularForm(dbModelType.Name);
                columnName = $"{dbModelTypeNameSingularForm}Id";
            }
            //
            return columnName;
        }

        public string MakeSchemaName(DbSchemaAttribute dbSchemaAttribute, Type dbModelType)
        {
            if (dbSchemaAttribute != null && !string.IsNullOrEmpty(dbSchemaAttribute.Name))
                return dbSchemaAttribute.Name;
            return dbModelType.Namespace.Split('.').Last();
        }

        public string ToSingularForm(string name)
        {
            return _context.PluralizationService.Singularize(name);
        }

        public static bool ShouldReferencingColumnNameBeChanged(BuilderContext context, DbTable dbTable, DbColumn dbColumn, out string newColumnName)
        {
            newColumnName = string.Empty;
            string currentColumnName = dbColumn.Name;
            int refKeySuffixIndex = currentColumnName.LastIndexOf("REFKEY", StringComparison.InvariantCultureIgnoreCase);
            if (refKeySuffixIndex >= 0)
            {
                DbTable dbReferencedTable = DbModelHelpers.GetForeignKeyPropertyReferencedTable(context, dbTable.SourceType, dbColumn.SourceProperty);
                if (dbReferencedTable.PrimaryKey.DbColumns.Count == 1)
                {
                    string columnName = currentColumnName.Substring(0, refKeySuffixIndex) + dbReferencedTable.PrimaryKey.DbColumns[0].Name;
                    newColumnName = columnName;
                    return true;
                }
                throw new Exception($"Referenced table doesn't have primary key with one column, it cannot be referenced with a property with KEYREF-name.");
            }
            return false;
        }
    }
}
