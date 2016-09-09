using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.StandardDictionary;

namespace Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved.Dicts
{
    [DbModelPart()]
    [DbTable()]
    [Trait(typeof(StandardDictionary.StandardDictionary))]
    public class ApprovementStatuses
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int ApprovementStatusId;
    }
}
