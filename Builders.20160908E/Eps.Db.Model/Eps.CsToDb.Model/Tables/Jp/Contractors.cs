using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;
using Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved;
using Eps.CsToDb.Model.Tables.Dicts;
using Eps.CsToDb.Model.Tables.Eps;

namespace Eps.CsToDb.Model.Tables.Jp
{
    [DbModelPart()]
    [DbSchema("Jp")]
    public abstract class Contractors
    {
        [DbPrimaryKey()]
        JuridicalPersons REFKEY;
    }
}
