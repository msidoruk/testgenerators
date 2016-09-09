using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Builder.Common;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Builder.DbModel.Utils
{
    public class DbModelHelpers
    {
        public static bool IsItPrimaryKeyProperty(BuilderContext context, Type dbModelType, FieldInfo fieldInfo)
        {
            return AttributesHelper.IsMarkedBy<DbPrimaryKeyAttribute>(fieldInfo);
        }

        public static bool GetColumnIdentityFlag(BuilderContext context, Type sourceType, FieldInfo sourceField)
        {
            bool isIdentity = false;
            DbColumnAttribute dbColumnAttribute = AttributesHelper.GetAttribute<DbColumnAttribute>(sourceField);
            if (dbColumnAttribute != null)
                isIdentity = dbColumnAttribute.IsIdentity;
            return isIdentity;
        }

        public static bool IsItInternalDictionaryReferencingProperty(BuilderContext context, Type dbModelType, FieldInfo fieldInfo)
        {
            return AttributesHelper.IsMarkedBy<InternalDictionaryAttribute>(fieldInfo);
        }

        public static bool IsItForeignKeyProperty(BuilderContext context, Type dbModelType, FieldInfo fieldInfo)
        {
            if (AttributesHelper.IsMarkedBy<InternalDictionaryAttribute>(fieldInfo/*.FieldType*/))
                return false;
            return context.DbModel.GetModelTypeDbTable(fieldInfo.FieldType) != null;
        }

        public static DbTable GetForeignKeyFieldReferencedTable(BuilderContext context, Type dbModelType, FieldInfo fieldInfo)
        {
            if (AttributesHelper.IsMarkedBy<InternalDictionaryAttribute>(fieldInfo.FieldType))
                return null;
            return context.DbModel.GetModelTypeDbTable(fieldInfo.FieldType);
        }

        public static DbColumnAttribute GetDbColumnAttribute(BuilderContext context, Type dbModelType, FieldInfo fieldInfo)
        {
            DbColumnAttribute dbColumnAttribute = AttributesHelper.GetAttribute<DbColumnAttribute>(fieldInfo, null);
            if (dbColumnAttribute != null)
            {
                // Support of notion 'InternalDictionary' (dictionaries that implemented in application, for instance: Enums)
                InternalDictionaryAttribute internalDictionaryAttribute =
                    AttributesHelper.GetAttribute<InternalDictionaryAttribute>(fieldInfo.FieldType);
                if (internalDictionaryAttribute != null)
                    dbColumnAttribute.ColumnType = internalDictionaryAttribute.UnderlyingType;
            }
            else
            {
                // If dbModelType's member not marked by dbColumn we create dbColumn automatically
                dbColumnAttribute =
                    new DbColumnAttribute(GetDbColumnTypeByCsFieldType(context, dbModelType, fieldInfo))
                    {
                        ColumnSize = 0,
                        IsNullable = true,
                        Name = fieldInfo.Name
                    };
            }
            //
            return dbColumnAttribute;
        }

        private static SqlDbType GetDbColumnTypeByCsFieldType(BuilderContext context, Type dbModelType, FieldInfo fieldInfo)
        {
            // Support for Foreign keys
            if (GetForeignKeyFieldReferencedTable(context, dbModelType, fieldInfo) != null)
                return SqlDbType.Int;
            // Support of notion 'InternalDictionary' (dictionaries that implemented in application, for instance: Enums)
            InternalDictionaryAttribute internalDictionaryAttribute =
                AttributesHelper.GetAttribute<InternalDictionaryAttribute>(fieldInfo);
            if (internalDictionaryAttribute != null)
            {
                return internalDictionaryAttribute.UnderlyingType;
            }
            return context.CsToSqlTypesMapper.GetSqlType(fieldInfo.FieldType);
        }

        public static bool IsForeignKeySuppressed(BuilderContext context, Type sourceType, FieldInfo sourceField)
        {
            return AttributesHelper.IsMarkedBy<DbSuppressForeignKeyAttribute>(sourceField);
        }
    }
}
