using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Base;

namespace EPS.DtoMaker.Engine.Helpers
{
    public class DbHelpers
    {
        public static int ReadDbQueryRows(IDbConnection dbConnection, string query, out DataTable dataTable)
        {
            List<DataRow> dataRows = new List<DataRow>();
            //
            DbCommand dbCommand = (DbCommand)dbConnection.CreateCommand();
            dbCommand.CommandText = query;
            dbCommand.CommandType = CommandType.Text;
            //
            DataTable tempTable = null;
            using (DbDataReader dbDataReader = dbCommand.ExecuteReader())
            {
                if (dbDataReader.HasRows)
                {
                    using (dbDataReader)
                    {
                        while (dbDataReader.Read())
                        {
                            ;
                            if (tempTable == null)
                            {
                                tempTable = new DataTable("tempTable");
                                for (int fieldIndex = 0; fieldIndex < dbDataReader.FieldCount; ++fieldIndex)
                                {
                                    tempTable.Columns.Add(new DataColumn(dbDataReader.GetName(fieldIndex),
                                        dbDataReader.GetFieldType(fieldIndex)));
                                }
                            }
                            //
                            object[] fieldsValues = new object[dbDataReader.FieldCount];
                            for (int fieldIndex = 0; fieldIndex < dbDataReader.FieldCount; ++fieldIndex)
                            {
                                fieldsValues[fieldIndex] = dbDataReader[fieldIndex];
                            }
                            //
                            DataRow dataRow = tempTable.NewRow();
                            dataRow.ItemArray = fieldsValues;
                            tempTable.Rows.Add(dataRow);
                        }
                    }
                }
            }
            //
            dataTable = tempTable;
            //
            return dataRows.Count;
        }

        public static bool GetDbTableRowColumnOrDefault<T>(DataTable dataTable, int rowIndex, string columnName, Func<object,T> f, out T value)
        {
            value = default(T);
            if (dataTable != null)
            {
                if (rowIndex < dataTable.Rows.Count)
                {
                    int columnIndex = dataTable.Columns.IndexOf(columnName);
                    if (columnIndex >= 0)
                    {
                        object columnValueAsObject = dataTable.Rows[rowIndex][columnIndex];
                        if (columnValueAsObject != null)
                        {
                            value = (f != null) ? f(columnValueAsObject) : (T) columnValueAsObject;
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        public static Tuple<bool, string> GetTableInfo(IDbConnection dbConnection, string schemaName, string tableName, out TableInfo tableInfo)
        {
            tableInfo = null;
            //
            try
            {
                dbConnection.Open();
                // Get types info.
                Dictionary<int, DbTypeInfoParser> dbSystemTypes = new Dictionary<int, DbTypeInfoParser>();
                DataTable typesInfoDbTable;
                DbHelpers.ReadDbQueryRows(dbConnection, "select * from sys.types", out typesInfoDbTable);
                foreach (DataRow currentTypeInfo in typesInfoDbTable.Rows)
                {
                    int userTypeId = (int) currentTypeInfo["user_type_id"/*"system_type_id"*/];
                    dbSystemTypes[userTypeId] = new DbTypeInfoParser(currentTypeInfo);
                }
                // Get schema info.
                int schemaId;
                if (!GetSchemaInfo(dbConnection, schemaName, out schemaId))
                {
                    return Tuple.Create(false, $"Cannot find schema '{schemaName}'");
                }
                // Get table info.
                int tableObjectId;
                DataTable sourceTableColumns;
                List<string> pkColumns;
                if (!GetTableInfo(dbConnection, schemaId, tableName, out tableObjectId, out sourceTableColumns, out pkColumns))
                {
                    return Tuple.Create(false, $"Cannot find table '{tableName}'");
                }
                //
                Dictionary<int, string> sourceTableColumnsByIds;
                BuildTableColumnsMap(sourceTableColumns, out sourceTableColumnsByIds);
                // Get table foreign keys info.
                List<TableLink> tableLinks = new List<TableLink>();
                DataTable tableForeignKeys;
                DbHelpers.ReadDbQueryRows(dbConnection, $"select * from sys.foreign_keys where parent_object_id = {tableObjectId}", out tableForeignKeys);
                if (tableForeignKeys != null)
                {
                    foreach (DataRow currentForeignKeyInfo in tableForeignKeys.Rows)
                    {
                        int foreignKeyId = (int) currentForeignKeyInfo["object_id"];
                        int foreignKeyReferencedTableId = (int) currentForeignKeyInfo["referenced_object_id"];
                        // Get foreign key's columns.
                        DataTable foreignKeyColumns;
                        DbHelpers.ReadDbQueryRows(dbConnection,
                            $"select * from sys.foreign_key_columns where constraint_object_id = '{foreignKeyId}'",
                            out foreignKeyColumns);
                        // Get referenced table info.
                        DataTable referencedTableColumns;
                        int referencedTableSchemaId;
                        string referencedSchemaName;
                        string referencedTableName;
                        List<string> tablePKColumns;
                        if (!GetTableInfo(dbConnection, foreignKeyReferencedTableId, out referencedTableSchemaId, out referencedTableName, out referencedTableColumns, out tablePKColumns))
                        {
                            return Tuple.Create(false,
                                $"Cannot find table's referenced table: tableName='{tableName}' referenced table's object_id='{foreignKeyReferencedTableId}'");
                        }
                        GetSchemaInfo(dbConnection, referencedTableSchemaId, out referencedSchemaName);
                        Dictionary<int, string> referencedTableColumnsByIds;
                        BuildTableColumnsMap(referencedTableColumns, out referencedTableColumnsByIds);
                        // Process foreign key columns.
                        for (int foreignKeyColumnIndex = 0;
                            foreignKeyColumnIndex < foreignKeyColumns.Rows.Count;
                            ++foreignKeyColumnIndex)
                        {
                            int parentColumnId;
                            if (!DbHelpers.GetDbTableRowColumnOrDefault(foreignKeyColumns, foreignKeyColumnIndex, "parent_column_id", null, out parentColumnId))
                            {
                            }
                            string sourceTableColumn;
                            sourceTableColumnsByIds.TryGetValue(parentColumnId, out sourceTableColumn);
                            //
                            int referencedColumnId;
                            if (
                                !DbHelpers.GetDbTableRowColumnOrDefault(foreignKeyColumns, foreignKeyColumnIndex,
                                    "referenced_column_id", null, out referencedColumnId))
                            {
                            }
                            string referencedTableColumn;
                            referencedTableColumnsByIds.TryGetValue(referencedColumnId, out referencedTableColumn);
                            //
                            tableLinks.Add(new TableLink()
                            {
                                SourceTableColumn = sourceTableColumn,
                                ReferencedSchema = referencedSchemaName,
                                ReferencedTableName = referencedTableName,
                                ReferencedTableColumn = referencedTableColumn
                            });
                        }
                    }
                }
                // Create result object.
                tableInfo = new TableInfo
                {
                    SchemaName = schemaName,
                    SchemaId = schemaId,
                    TableName = tableName,
                    TableId = tableObjectId,
                    SystemTypes = dbSystemTypes,
                    TableColumns = sourceTableColumns,
                    TableLinks = tableLinks,
                    PKColumns = pkColumns,
                };
                //
                return Tuple.Create(true, string.Empty);
            }
            finally
            {
                dbConnection.Close();
            }
        }

        private static bool BuildTableColumnsMap(DataTable tableColumns, out Dictionary<int, string> tableColumnsByIds)
        {
            tableColumnsByIds = new Dictionary<int, string>();
            for (int columnIndex = 0; columnIndex < tableColumns.Rows.Count; ++columnIndex)
            {
                int columnId;
                if (!DbHelpers.GetDbTableRowColumnOrDefault(tableColumns, columnIndex, "column_id", null, out columnId))
                    return false;
                string name;
                if (!DbHelpers.GetDbTableRowColumnOrDefault(tableColumns, columnIndex, "name", null, out name))
                    return false;
                tableColumnsByIds[columnId] = name;
            }
            return true;
        }

        private static bool GetSchemaInfo(IDbConnection dbConnection, string schemaName, out int schemaId)
        {
            string schemaInfoQuery = $"select schema_id from sys.schemas where name='{schemaName}'";
            DataTable schemaInfoDbTable;
            DbHelpers.ReadDbQueryRows(dbConnection, schemaInfoQuery, out schemaInfoDbTable);
            return DbHelpers.GetDbTableRowColumnOrDefault(schemaInfoDbTable, 0, "schema_id", null, out schemaId);
        }

        private static bool GetSchemaInfo(IDbConnection dbConnection, int schemaId, out string schemaName)
        {
            string schemaInfoQuery = $"select name from sys.schemas where schema_id='{schemaId}'";
            DataTable schemaInfoDbTable;
            DbHelpers.ReadDbQueryRows(dbConnection, schemaInfoQuery, out schemaInfoDbTable);
            return DbHelpers.GetDbTableRowColumnOrDefault(schemaInfoDbTable, 0, "name", null, out schemaName);
        }
        private static bool GetTableInfo(IDbConnection dbConnection, int schemaId, string tableName, out int tableObjectId, out DataTable tableColumns, out List<string> pkColumns)
        {
            pkColumns = new List<string>();
            tableColumns = null;
            string tableInfoQuery = $"select * from sys.all_objects where (type='U' and name='{tableName}') and (schema_id={schemaId})";
            DataTable tableInfoDbTable;
            DbHelpers.ReadDbQueryRows(dbConnection, tableInfoQuery, out tableInfoDbTable);
            if (!DbHelpers.GetDbTableRowColumnOrDefault(tableInfoDbTable, 0, "object_id", null, out tableObjectId))
                return false;
            // Get table PK
            if (!GetTablePkColumns(dbConnection, tableName, tableObjectId, pkColumns))
                return false;
            // Get table columns info.
            return GetTableColumns(dbConnection, tableObjectId, out tableColumns);
        }

        private static bool GetTablePkColumns(IDbConnection dbConnection, string tableName, int tableObjectId, List<string> pkColumns)
        {
            if (string.IsNullOrEmpty(tableName))
                tableName = $"table_id({tableObjectId})";
            string tablePKQuery =
                "SELECT c.name AS column_name, i.name AS index_name, i.is_primary_key, c.is_identity " +
                "FROM sys.indexes i " +
                "inner join sys.index_columns ic  ON i.object_id = ic.object_id AND i.index_id = ic.index_id " +
                "inner join sys.columns c ON ic.object_id = c.object_id AND c.column_id = ic.column_id " +
                $" WHERE i.is_primary_key = 1 and c.object_id={tableObjectId}";
            string tablePKColumnName;
            DataTable tablePKColumns;
            DbHelpers.ReadDbQueryRows(dbConnection, tablePKQuery, out tablePKColumns);
            if (tablePKColumns != null && tablePKColumns.Rows.Count == 1)
            {
                if (!DbHelpers.GetDbTableRowColumnOrDefault(tablePKColumns, 0, "column_name", null, out tablePKColumnName))
                    return false;
                pkColumns.Add(tablePKColumnName);
                return true;
            }
            else
            {
                // $mas(20160905): throw new Exception($"Unable to retrieve PK columns for table: {tableName}");
                return true;
            }

        }

        private static bool GetTableInfo(IDbConnection dbConnection, int tableObjectId, out int schemaId, out string tableName, out DataTable tableColumns, out List<string> pkColumns)
        {
            pkColumns = new List<string>();
            schemaId = -1;
            tableName = string.Empty;
            tableColumns = null;
            string tableInfoQuery = $"select name, schema_id from sys.all_objects where type='U' and object_id={tableObjectId}";
            DataTable tableInfoDbTable;
            DbHelpers.ReadDbQueryRows(dbConnection, tableInfoQuery, out tableInfoDbTable);
            if (!DbHelpers.GetDbTableRowColumnOrDefault(tableInfoDbTable, 0, "schema_id", null, out schemaId))
                return false;
            if (!DbHelpers.GetDbTableRowColumnOrDefault(tableInfoDbTable, 0, "name", null, out tableName))
                return false;
            // Get table PK
            if (!GetTablePkColumns(dbConnection, tableName, tableObjectId, pkColumns))
                return false;
            // Get table columns info.
            return GetTableColumns(dbConnection, tableObjectId, out tableColumns);
        }

        private static bool GetTableColumns(IDbConnection dbConnection, int tableObjectId, out DataTable tableColumns)
        {
            tableColumns = null;
            string tableColumnsInfoQuery = $"select * from sys.all_columns where object_id = '{tableObjectId}'";
            DbHelpers.ReadDbQueryRows(dbConnection, tableColumnsInfoQuery, out tableColumns);
            return tableColumns != null;
        }
    }
}
