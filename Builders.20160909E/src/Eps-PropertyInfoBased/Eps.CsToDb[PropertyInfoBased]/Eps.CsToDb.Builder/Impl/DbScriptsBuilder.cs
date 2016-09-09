using System;
using System.Collections.Generic;
using System.Text;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Builder.DbModel;

namespace Eps.CsToDb.Builder.Impl
{
    public class DbScriptsBuilder : IDbScriptsBuilder
    {
        private BuilderContext _context;
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
                scriptStringBuilder.AppendLine($"\t{BuildColumnSpecification(dbColumn)},");
                processedColumns[dbColumn.Name] = dbColumn;
            }
            // Append columns
            foreach (var dbColumn in dbTable.DbColumns)
            {
                if (!processedColumns.ContainsKey(dbColumn.Name))
                    scriptStringBuilder.AppendLine($"\t{BuildColumnSpecification(dbColumn)},");
            }
            scriptStringBuilder.AppendLine($"\t-- Constraints");
            // Append Primary key constraint
            foreach (var dbColumn in dbTable.PrimaryKey.DbColumns)
            {
                scriptStringBuilder.AppendLine($"\tCONSTRAINT PK_{dbTable.Name} PRIMARY KEY({dbColumn.Name}),");
                break; // We support just one column PKs
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
                    fkConstraintName = fkConstraintName + "_" + referencedColumnName;
                    break; // we support just one column PKs
                }

                scriptStringBuilder.AppendLine($"\tCONSTRAINT {fkConstraintName} FOREIGN KEY ({referencingColumns}) REFERENCES {referencedTable}({referencedColumns}),");
            }
            scriptStringBuilder.AppendLine($")");
            //
            string scriptString = scriptStringBuilder.ToString();
            _context.Logger.Debug($"\n{scriptString}");
            return scriptString;
        }

        private string BuildColumnSpecification(DbColumn dbColumn)
        {
            List<string> specificators = new List<string>
            {
                $"[{dbColumn.Name}]",
                dbColumn.ColumnType.ToString().ToUpper(),
                dbColumn.ColumnSize > 0 ? $"({dbColumn.ColumnSize})" : string.Empty,
                dbColumn.IsIdentity ? "IDENTITY(1,1)" : string.Empty,
                dbColumn.IsNullable ? "NULL" : "NOT NULL"
            };

            StringBuilder columnSpecification = new StringBuilder();
            int specificatorIndex = 0;
            foreach (var specificator in specificators)
            {
                if (!string.IsNullOrEmpty(specificator))
                {
                    string space = columnSpecification.Length > 0 ? " " : string.Empty;
                    columnSpecification.Append($"{space}{specificator}");
                    ++specificatorIndex;
                }
            }
            return columnSpecification.ToString();
        }
    }
}
