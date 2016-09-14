using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Builder.DbModel;
using Eps.CsToDb.Builder.DbModel.Utils;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Builder.Impl
{
    public class DbScriptsBuilder : IDbScriptsBuilder
    {
        private BuilderContext _context;
        private const int ReferenceCommentStartsOnLineColumn = 60;

        public void Build(BuilderContext context)
        {
            _context = context;
            //
            context.Logger.Debug("---------- BEGIN SCRIPT GENERATION ----------");
            foreach (var dbSchema in context.DbModel.DbSchemas)
            {
                context.Logger.Debug($"---------- Build script for SCHEMA: '{dbSchema.Name}' ----------");
                //
                string dbSchemaScript = BuildScript(dbSchema);
                dbSchema.CreationScript = dbSchemaScript;
            }
            //
            foreach (var dbSchema in context.DbModel.DbSchemas)
            {
                foreach (var dbTable in dbSchema.SchemaTables)
                {
                    context.Logger.Debug($"---------- Build script for TABLE: '{dbSchema.Name}.{dbTable.Name}' ----------");
                    //
                    string dbTableScript = BuildScript(dbTable);
                    dbTable.CreationScript = dbTableScript;
                }
            }
        }

        private string BuildScript(DbSchema dbSchema)
        {
            StringBuilder scriptStringBuilder = new StringBuilder();
            scriptStringBuilder.AppendLine($"CREATE SCHEMA [{dbSchema.Name}]");
            scriptStringBuilder.AppendLine($"\tAUTHORIZATION [dbo];");

            string scriptString = scriptStringBuilder.ToString();
            _context.Logger.Debug($"\n{scriptString}");
            return scriptString;
        }

        private string BuildScript(DbTable dbTable)
        {
            StringBuilder scriptStringBuilder = new StringBuilder();
            scriptStringBuilder.AppendLine($"CREATE TABLE [{dbTable.DbSchema.Name}].[{dbTable.Name}]");
            scriptStringBuilder.AppendLine($"(");
            Dictionary<string, DbColumn> processedColumns = new Dictionary<string, DbColumn>(StringComparer.InvariantCultureIgnoreCase);
            // Append Primary key column
            foreach (var dbColumn in dbTable.PrimaryKey.DbColumns)
            {
                scriptStringBuilder.AppendLine($"\t{BuildColumnSpecification(dbColumn, true)}");
                processedColumns[dbColumn.Name] = dbColumn;
            }
            // Append columns
            foreach (var dbColumn in dbTable.DbColumns)
            {
                if (!processedColumns.ContainsKey(dbColumn.Name))
                    scriptStringBuilder.AppendLine($"\t{BuildColumnSpecification(dbColumn, true)}");
            }
            scriptStringBuilder.AppendLine($"\t-- Constraints");
            // Append Primary key constraint
            StringBuilder primaryKeyColumnsListStringBuilder = new StringBuilder();
            int primaryKeyColumnIndex = 0;
            foreach (var dbColumn in dbTable.PrimaryKey.DbColumns)
            {
                if (DbModelHelpers.IsItPrimaryKeyProperty(_context, dbTable.SourceType, dbColumn.SourceField))
                {
                    if (primaryKeyColumnIndex > 0)
                        primaryKeyColumnsListStringBuilder.Append(", ");
                    primaryKeyColumnsListStringBuilder.Append(dbColumn.Name);
                    ++primaryKeyColumnIndex;
                }
            }
            string primaryKeyColumnsList = primaryKeyColumnsListStringBuilder.ToString();
            if (!string.IsNullOrEmpty(primaryKeyColumnsList))
            {
                string pkConstraintName = $"PK_{dbTable.Name}";
                scriptStringBuilder.AppendLine($"\tCONSTRAINT {pkConstraintName} PRIMARY KEY({primaryKeyColumnsList}),");
                dbTable.PrimaryKeyConstraintName = pkConstraintName;
            }
            // Append Foreign key constraints
            foreach (var foreignKey in dbTable.ForeignKeys)
            {
                string referencedTable = $"{foreignKey.ReferencedTable.DbSchema.Name }.[{foreignKey.ReferencedTable.Name}]";

                string fkConstraintNameBase = $"FK_{foreignKey.ReferencedTable.DbSchema.Name}_{foreignKey.ReferencedTable.Name}_{dbTable.DbSchema.Name}_{dbTable.Name}";

                string fkConstraintName = fkConstraintNameBase;
                StringBuilder referencedColumns = new StringBuilder();
                StringBuilder referencingColumns = new StringBuilder();
                for (int fkColumnIndex = 0;
                    fkColumnIndex < Math.Min(foreignKey.ReferencedColumns.Count, foreignKey.ReferencingColumns.Count);
                    ++fkColumnIndex)
                {
                    string referencedColumnName = foreignKey.ReferencedColumns[fkColumnIndex].Name;
                    string referencingColumnName = foreignKey.ReferencingColumns[fkColumnIndex].Name;
                    referencedColumns.AppendFormat("{0}{1}", fkColumnIndex == 0 ? string.Empty : ", ", referencedColumnName);
                    referencingColumns.AppendFormat("{0}{1}", fkColumnIndex == 0 ? string.Empty : ", ", referencingColumnName);
                    fkConstraintName = fkConstraintName + "_" + referencingColumnName;
                    foreignKey.ConstraintName = fkConstraintName;
                    break; // we support just one column PKs
                }
                string suppressing = foreignKey.IsSuppressed ? $"-- Suppressed with '{typeof(DbSuppressForeignKeyAttribute).Name}' -- " : string.Empty;
                scriptStringBuilder.AppendLine($"\t{suppressing}CONSTRAINT {fkConstraintName} FOREIGN KEY ({referencingColumns}) REFERENCES {referencedTable}({referencedColumns}),");
            }
            scriptStringBuilder.AppendLine($")");
            //
            string scriptString = scriptStringBuilder.ToString();
            _context.Logger.Debug($"\n{scriptString}");
            return scriptString;
        }

        private string BuildColumnSpecification(DbColumn dbColumn, bool appendComma)
        {
            bool canColumnBeNull = dbColumn.IsNullable && !dbColumn.IsIdentity && !DbModelHelpers.IsItPrimaryKeyProperty(_context, dbColumn.DbTable.SourceType, dbColumn.SourceField);

            string referenceComment = string.Empty;
            if (DbModelHelpers.IsItForeignKeyProperty(_context, dbColumn.DbTable.SourceType, dbColumn.SourceField))
            {
                DbTable referencedDbTable = DbModelHelpers.GetForeignKeyFieldReferencedTable(_context, dbColumn.DbTable.SourceType, dbColumn.SourceField);
                referenceComment = $"-- Reference to '{referencedDbTable.DbSchema.Name}.{referencedDbTable.Name}'";
            } else if (DbModelHelpers.IsItInternalDictionaryReferencingProperty(_context, dbColumn.DbTable.SourceType, dbColumn.SourceField))
            {
                referenceComment = $"-- Reference to internal dictionary '{dbColumn.SourceField.FieldType.FullName}'";
            }

            List<string> specificators = new List<string>
            {
                $"[{dbColumn.Name}]",
                dbColumn.ColumnType.ToString().ToUpper(),
                dbColumn.ColumnSize != 0 ? MakeColumnSizeString(dbColumn) : string.Empty,
                dbColumn.IsIdentity ? "IDENTITY(1,1)" : string.Empty,
                canColumnBeNull ? "NULL" : "NOT NULL",
            };

            StringBuilder columnSpecificationStringBuilder = new StringBuilder();
            int specificatorIndex = 0;
            foreach (var specificator in specificators)
            {
                if (!string.IsNullOrEmpty(specificator))
                {
                    string space = columnSpecificationStringBuilder.Length > 0 ? " " : string.Empty;
                    columnSpecificationStringBuilder.Append($"{space}{specificator}");
                    ++specificatorIndex;
                }
            }
            if (appendComma)
                columnSpecificationStringBuilder.Append(",");
            string columnSpecification = columnSpecificationStringBuilder.ToString();
            if (!string.IsNullOrEmpty(referenceComment))
            {
                int columnSpecificationLength = columnSpecification.Length;
                if (columnSpecificationLength < ReferenceCommentStartsOnLineColumn)
                    columnSpecification += new string(' ', ReferenceCommentStartsOnLineColumn - columnSpecificationLength);
                columnSpecification += referenceComment;
            }
            return columnSpecification;
        }

        private string MakeColumnSizeString(DbColumn dbColumn)
        {
            if (dbColumn.ColumnType == SqlDbType.VarChar || dbColumn.ColumnType == SqlDbType.Char ||
                dbColumn.ColumnType == SqlDbType.NVarChar || dbColumn.ColumnType == SqlDbType.NChar)
            {
                return dbColumn.ColumnSize > 0 ? $"({dbColumn.ColumnSize})" : (dbColumn.ColumnSize == -1 ? "(MAX)" : "0");
            }
            return string.Empty;
        }
    }
}
