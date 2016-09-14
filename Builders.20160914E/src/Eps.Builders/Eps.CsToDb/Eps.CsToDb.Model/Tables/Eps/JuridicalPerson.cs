using System.Data;
using System.Data.SqlTypes;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;
using Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved;

namespace Eps.CsToDb.Model.Tables.Eps
{
    [DbModelPart()]
    [DbSchema("eps")]
    [DbTable(DbTableName = "JuridicalPerson")]
    [Trait(typeof(HasToBeApproved))]
    [Trait(typeof(ActsForRange))]
    public abstract class JuridicalPerson
    {
        // [DbColumn(SqlDbType.Int)]
        [DbPrimaryKey()]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        public int Id;
    }
}
