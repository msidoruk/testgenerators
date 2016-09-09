using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.StandardDictionary;

namespace Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved.Dicts
{
    [DbModelPart()]
    [DbSchema("dicts")]
    [DbTable()]
    [Trait(typeof(StandardDictionary.StandardDictionary))]
    public class ApprovementStatus
    {
        [DbPrimaryKey()]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        public int ApprovementStatusId;
    }
}
