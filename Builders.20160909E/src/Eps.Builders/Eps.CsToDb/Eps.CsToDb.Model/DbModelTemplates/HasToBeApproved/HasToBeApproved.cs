using System.Data;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved.Dicts;

namespace Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved
{
    public class HasToBeApproved
    {
        [DbColumn(SqlDbType.Int)]
        public ApprovementStatus Status;
    }
}
