using System.Data;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved.Dicts;

namespace Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved
{
    public interface IHasToBeApproved
    {
        [DbColumn(SqlDbType.Int)]
        ApprovementStatus Status { get; }
    }
}
