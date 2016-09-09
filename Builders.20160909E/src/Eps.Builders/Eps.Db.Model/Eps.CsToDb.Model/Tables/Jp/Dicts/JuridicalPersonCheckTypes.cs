using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.StandardDictionary;

namespace Eps.CsToDb.Model.Tables.Jp.Dicts
{
    [DbModelPart()]
    [Trait(typeof(StandardDictionary))]
    public class JuridicalPersonCheckTypes
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;
    }
}
