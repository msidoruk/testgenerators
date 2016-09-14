using System.Linq;
using Eps.CsToDb.Builder.Base;
using Eps.DbSharedModel.DbModel.Parts;

namespace Eps.CsToDb.Builder.Impl
{
    public class DbSharedModelBuilder : IDbSharedModelBuilder
    {
        public DbSharedModel.DbModel.DbSharedModel Build(BuilderContext context)
        {
            DbSharedModel.DbModel.DbSharedModel outputModel = new DbSharedModel.DbModel.DbSharedModel();
            foreach (var dbTable in context.DbModel.DbTables)
            {
                DbTableDef dbTableDef = new DbTableDef()
                {
                    TableSchema = dbTable.DbSchema.Name,
                    TableName = dbTable.Name,
                    Columns = dbTable.DbColumns.Select(_ => new DbColumnDef {ColumnName = _.Name}).ToList(),
                    Features = dbTable.DbTableFeatures.Select(_ => _.Serialize()).ToList()
                };
                outputModel.DbTables.Add(dbTableDef);
            }
            return outputModel;
        }
    }
}
