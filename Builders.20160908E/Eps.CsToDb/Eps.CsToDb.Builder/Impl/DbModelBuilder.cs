using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Autofac;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Builder.Common;
using Eps.CsToDb.Builder.DbModel;
using Eps.CsToDb.Builder.DbModel.Utils;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Builder.Impl
{
    public class DbModelBuilder : IDbModelBuilder
    {
        private BuilderContext _context;
        private DbNamingRules _dbNamingRules;

        public void Build(BuilderContext context)
        {
            _context = context;
            //
            _context.Logger.Debug($" -------------- DB MODEL BUILD STARTED ----------------------");
            //
            _dbNamingRules = new DbNamingRules(context);
            _context.DbModel = new DbModel.DbModel();
            //
            _context.Logger.Debug($" ------------------ CREATE TABLES AND SCHEMAS ------------------");
            CreateTablesAndSchemas();
            //
            _context.Logger.Debug($" ------------------ CREATE TABLES COLUMNS  ------------------");
            PropagateTablesWithColumns();
            //
            _context.Logger.Debug($" ------------------ CREATE TABLES PKs  ------------------");
            BuildPrimaryKeys();
            //
            _context.Logger.Debug($" ------------------ CREATE TABLES FKs  ------------------");
            BuildForeignKeys();
            //
            _context.Logger.Debug($" -------------- DB MODEL BUILD FINISHED ----------------------");
        }

        private void CreateTablesAndSchemas()
        {
            foreach (var dbModelType in _context.DbModelTypes)
            {
                DbTableAttribute dbTableAttribute = AttributesHelper.GetAttribute(dbModelType, new DbTableAttribute());
                if (dbTableAttribute == null)
                    return;
                DbSchemaAttribute dbSchemaAttribute = AttributesHelper.GetAttribute(dbModelType, new DbSchemaAttribute());
                if (dbSchemaAttribute == null)
                    throw new Exception($"DbModelType '{dbModelType.FullName}' doesn't define its DbSchema.");

                DbSchema dbSchema = _context.DbModel.FindDbSchemaOrCreateOne(_dbNamingRules.MakeSchemaName(dbSchemaAttribute, dbModelType));
                dbSchema.SourceType = dbModelType;
                _context.Logger.Debug($"DbSchema '{dbSchema.Name}' was found (or created) for '{dbModelType.FullName}'");

                string tableName = _dbNamingRules.MakeTableName(dbTableAttribute, dbModelType);
                if (!_context.PluralizationService.IsPlural(tableName)) 
                    _context.Logger.Warn($"!!! Table '{tableName}' of DbModelType '{dbModelType.FullName}' is not plural.");
                DbTable dbTable = _context.DbModel.AppendDbTable(tableName, dbModelType);
                _context.Logger.Debug($"DbTable '{dbTable.Name}' was created for '{dbModelType.FullName}'");
                dbTable.DbSchema = dbSchema;
                dbSchema.SchemaTables.AddTable(dbTable);
            }
        }

        private void PropagateTablesWithColumns()
        {
            foreach (DbTable dbTable in _context.DbModel.DbTables)
            {
                AppendColumnsByProperties(dbTable, dbTable.SourceType, false);
                AppendColumnsByTraits(dbTable, dbTable.SourceType);
            }
        }

        private void AppendColumnsByProperties(DbTable dbTable, Type dbModelType, bool isTrait)
        {
            foreach (FieldInfo fieldInfo in dbModelType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField))
            {
                DbColumnAttribute dbColumnAttribute = DbModelHelpers.GetDbColumnAttribute(_context, dbModelType, fieldInfo);
                string columnName = _dbNamingRules.MakeColumnName(dbColumnAttribute, dbModelType, fieldInfo);
                DbColumn dbColumn = dbTable.AppendColumn(columnName, dbColumnAttribute.ColumnType, dbColumnAttribute.ColumnSize, dbColumnAttribute.IsNullable);
                dbColumn.SourceField = fieldInfo;
                string columnSourceType = isTrait ? "TRAIT" : "ORIGINAL";
                _context.Logger.Debug($"DbColumn '{dbTable.Name}.{dbColumn.Name} (dbtype={dbColumn.ColumnType}, size={dbColumn.ColumnSize}, null={dbColumn.IsNullable})'");
                _context.Logger.Debug($"             that came from {columnSourceType}: {dbModelType.FullName}");
            }
        }

        private void AppendColumnsByTraits(DbTable dbTable, Type dbModelType)
        {
            IEnumerable<TraitAttribute> standardTraitAttributes = AttributesHelper.GetAttributes<TraitAttribute>(dbModelType);
            foreach (var standardTraitAttribute in standardTraitAttributes)
            {
                Type traitType = standardTraitAttribute.TraitType;
                //
                AppendColumnsByProperties(dbTable, traitType, true);
                //
                // Process traits recursively
                AppendColumnsByTraits(dbTable, traitType);
            }
        }

        private void BuildPrimaryKeys()
        {
            foreach (DbTable dbTable in _context.DbModel.DbTables)
            {
                foreach (var dbColumn in dbTable.DbColumns)
                {
                    if (DbModelHelpers.IsItPrimaryKeyProperty(_context, dbTable.SourceType, dbColumn.SourceField))
                    {
                        dbColumn.IsIdentity = DbModelHelpers.GetColumnIdentityFlag(_context, dbTable.SourceType, dbColumn.SourceField);
                        dbTable.PrimaryKey.DbColumns.Add(dbColumn);
                        _context.Logger.Debug($"DbTable '{dbTable.Name}' obtained PK '{dbColumn.Name}'");
                        // break; // We support just ONE-COLUMN PKs
                    }
                }
            }
        }

        private void BuildForeignKeys()
        {
            foreach (DbTable dbReferencingTable in _context.DbModel.DbTables)
            {
                foreach (var dbReferencingColumn in dbReferencingTable.DbColumns)
                {
                    if (DbModelHelpers.IsItForeignKeyProperty(_context, dbReferencingTable.SourceType, dbReferencingColumn.SourceField))
                    {
                        // Foreign key with KEYREF property name renaming.
                        string newReferencingColumnName;
                        if (DbNamingRules.ShouldReferencingColumnNameBeChanged(_context, dbReferencingTable,
                            dbReferencingColumn, out newReferencingColumnName))
                        {
                            dbReferencingColumn.Name = newReferencingColumnName;
                        }

                        DbTable dbReferencedTable = DbModelHelpers.GetForeignKeyFieldReferencedTable(_context,
                            dbReferencingTable.SourceType, dbReferencingColumn.SourceField);
                        if (dbReferencedTable != null)
                        {
                            if (dbReferencedTable.PrimaryKey.DbColumns.Count != 1)
                                throw new Exception($"Table '{dbReferencedTable.Name}' cannot be referenced from '{dbReferencingTable.Name}' cause it doesn't have primary key with one column.");
                            DbColumn dbReferencedColumn = dbReferencedTable.PrimaryKey.DbColumns[0];
                            //
                            if (dbReferencingColumn.ColumnType != dbReferencedColumn.ColumnType)
                                throw new Exception($"Referenced column '{dbReferencingTable.Name}.{dbReferencingColumn.Name}' and referencing column '{dbReferencedTable.Name}.{dbReferencingColumn.Name}' have different types.");
                            //
                            ForeignKey newForeignKey = new ForeignKey()
                            {
                                SourceTable = dbReferencingTable,
                                ReferencedTable = dbReferencedTable
                            };
                            newForeignKey.ReferencingColumns.Add(dbReferencingColumn);
                            newForeignKey.ReferencedColumns.Add(dbReferencedColumn);
                            newForeignKey.IsSuppressed = DbModelHelpers.IsForeignKeySuppressed(_context, dbReferencingTable.SourceType, dbReferencingColumn.SourceField);
                            dbReferencingTable.ForeignKeys.AddForeignKey(newForeignKey);
                            _context.Logger.Debug($"DbTable '{dbReferencingTable.Name}' obtained FK for '{dbReferencingColumn.Name}' that points to '{dbReferencedTable.Name}.{dbReferencedColumn.Name}'");
                        }
                    }
                }
            }
        }
    }
}
