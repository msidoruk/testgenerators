using System;
using System.Collections.Generic;
using System.Data;
using Autofac;
using EPS.DtoMaker.Engine.Actions.Base;
using EPS.DtoMaker.Engine.Base;

namespace EPS.DtoMaker.Engine.Actions.ImportEntityFromDb
{
    public class ImportEntityFromDb : BaseAction<ImportEntityFromDb>
    {
        public class Markers
        {
            public const string DbSchema = "db:schema";
        }

        private string _schema = "*";
        private string _table = string.Empty;
        private List<Func<string, Tuple<bool, string>>> _columnToPropertyRenamingList = new List<Func<string, Tuple<bool, string>>>();
        private List<RawBaseClassSpecification> _rawBaseClasses = new List<RawBaseClassSpecification>();
        private List<string> _rawMembers = new List<string>();

        public ImportEntityFromDb(DtoMakerContext context)
            : base(context)
        {
        }

        public ImportEntityFromDb Schema(string schema)
        {
            _schema = schema;
            return this;
        }

        public ImportEntityFromDb Table(string table)
        {
            _table = table;
            return this;
        }

        public ModelEntity ImportedModelEntity { get; private set; }

        public override ImportEntityFromDb Do(DtoMakerContext context)
        {
            Marks[Markers.DbSchema] = _schema;
            ActionsApplicationHistoryItem currentActionsApplicationHistoryItem = context.CreateActionsApplicationHistoryItem(Marks);
            //
            string schemaName = _schema;
            if (schemaName.Equals("*"))
            {
                var findSchemaMarkResult = context.FindUpstairsMark<string>(currentActionsApplicationHistoryItem, Markers.DbSchema, m => !string.Equals(m, "*"));
                if (!findSchemaMarkResult.Item1)
                    throw new Exception($"Cannot determine schema name for combination: {_schema} {_table}");
                schemaName = findSchemaMarkResult.Item2;
            }
            //
            IDbDictionaryAccessor dbDictionaryAccessor = (IDbDictionaryAccessor)context.DiContainer.Resolve(typeof(IDbDictionaryAccessor));
            IDbPropertyTypeMapper dbPropertyTypeMapper = (IDbPropertyTypeMapper)context.DiContainer.Resolve(typeof(IDbPropertyTypeMapper));
            //
            TableInfo tableInfo;
            var result = dbDictionaryAccessor.GetTableInfo(schemaName, _table, out tableInfo);
            if (!result.Item1)
                throw new Exception(result.Item2);
            //
            DataColumn nameColumn = tableInfo.TableColumns.Columns["name"];
            DataColumn columnTypeColumn = tableInfo.TableColumns.Columns[/*"system_type_id"*/ "user_type_id"];
            DataColumn isNullableColumn = tableInfo.TableColumns.Columns["is_nullable"];
            //
            // Build Model's entity
            ModelEntity modelEntity = context.Model.CreateEntity(currentActionsApplicationHistoryItem);
            modelEntity.EntitySpace = schemaName;
            modelEntity.EntityName = _table;
            modelEntity.IdentityProperties = new List<string>(tableInfo.PKColumns);
            foreach (DataRow currentColumnInfo in tableInfo.TableColumns.Rows)
            {
                string columnName = (string) currentColumnInfo[nameColumn.Ordinal];
                int userTypeId = (int) currentColumnInfo[columnTypeColumn.Ordinal];
                bool isNullable = (bool)currentColumnInfo[isNullableColumn.Ordinal];
                //
                string propertyName = InternalColumnNameToProperty(columnName);
                modelEntity.OriginalPropertiesNames.AppendNames(columnName, propertyName);
                modelEntity.Properties.AppendProperty(propertyName, dbPropertyTypeMapper.Map(tableInfo.SystemTypes, userTypeId), isNullable);
            }
            foreach (var tableLink in tableInfo.TableLinks)
            {
                modelEntity.EntityLinks.AppendTableLink(tableLink.SourceTableColumn, tableLink.ReferencedSchema, tableLink.ReferencedTableName, tableLink.ReferencedTableColumn);
            }
            modelEntity.RawBaseClasses.AddRange(_rawBaseClasses);
            modelEntity.RawMembers.AddRange(_rawMembers);
            modelEntity.IsRepositoryRequired = context.IsMarked(currentActionsApplicationHistoryItem, BuildDtos.BuildDtos.Markers.WantsRepo);
            ImportedModelEntity = modelEntity;
            ;
            // 
            // Analyze foreign-keys constraints
            //
            base.Do(context);
            //
            return this;
        }

        private string InternalColumnNameToProperty(string columnName)
        {
            foreach (var columnToPropertyRenamingF in _columnToPropertyRenamingList)
            {
                Tuple<bool, string> columnToPropertyRenamingResult;
                if ((columnToPropertyRenamingResult = columnToPropertyRenamingF(columnName)).Item1)
                    return columnToPropertyRenamingResult.Item2;
            }
            return columnName;
        }

        protected override void ResetFieldsToDefaults()
        {
            _schema = "*";
            _table = string.Empty;
            _columnToPropertyRenamingList = new List<Func<string, Tuple<bool, string>>>();
            _rawBaseClasses.Clear();
            _rawMembers.Clear();
            //
            base.ResetFieldsToDefaults();
        }

        public class DependenciesNames {
            public const string DbDictionaryAccessor = "DbDictionaryAccessor";
        }

        public ImportEntityFromDb ColumnToProperty(string columnName, string propertyName)
        {
            _columnToPropertyRenamingList.Add(c => c.Equals(columnName, StringComparison.InvariantCultureIgnoreCase) ? Tuple.Create(true,propertyName) : Tuple.Create(false,string.Empty));
            return this;
        }

        public ImportEntityFromDb ColumnToProperty(Func<string, Tuple<bool, string>> columnToPropertyF)
        {
            _columnToPropertyRenamingList.Add(columnToPropertyF);
            return this;
        }

        public ImportEntityFromDb HasRawBaseClass(string rawBaseClass, string sourceNamespace = "")
        {
            _rawBaseClasses.Add(new RawBaseClassSpecification { Name=rawBaseClass, SourceNamespace=sourceNamespace });
            return this;
        }

        public ImportEntityFromDb HasRawMember(string rawMember)
        {
            _rawMembers.Add(rawMember);
            return this;
        }
    }

    public static class Extentions
    {
        public static void UseDbSchema(this DtoMakerContext dtoMakerContext, string schemaName)
        {
            dtoMakerContext.AppendMark(ImportEntityFromDb.Markers.DbSchema, schemaName);
        }
    }
}
