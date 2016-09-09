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
    public class DbModelHelpers
    {
        public static bool IsItPrimaryKeyProperty(BuilderContext context, Type dbModelType, PropertyInfo propertyInfo)
        {
            return AttributesHelper.IsMarkedBy<DbPrimaryKeyAttribute>(propertyInfo);
        }

        public static bool GetPrimaryKeyIdentityFlag(BuilderContext context, Type dbModelType, PropertyInfo propertyInfo)
        {
            bool isIdentity = false;
            DbPrimaryKeyAttribute dbPrimaryKeyAttribute = AttributesHelper.GetAttribute<DbPrimaryKeyAttribute>(propertyInfo);
            if (dbPrimaryKeyAttribute != null)
                isIdentity = dbPrimaryKeyAttribute.IsIdentity;
            return isIdentity;
        }

        public static bool IsItForeignKeyProperty(BuilderContext context, Type dbModelType, PropertyInfo propertyInfo)
        {
            return context.DbModel.GetModelTypeDbTable(propertyInfo.PropertyType) != null;
        }

        public static DbTable GetForeignKeyPropertyReferencedTable(BuilderContext context, Type dbModelType, PropertyInfo propertyInfo)
        {
            return context.DbModel.GetModelTypeDbTable(propertyInfo.PropertyType);
        }
    }
}
