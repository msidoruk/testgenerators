using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.StandardDictionary;

namespace Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved.Dicts
{
    [DbModelPart()]
    [DbTable()]
    [StandardTrait(typeof(IStandardDictionary))]
    public class ApprovementStatus
    {
        [DbColumn(SqlDbType.Int)]
        [DbPrimaryKey(IsIdentity=true)]
        public int ApprovementStatusId { get; set; }
    }
}
