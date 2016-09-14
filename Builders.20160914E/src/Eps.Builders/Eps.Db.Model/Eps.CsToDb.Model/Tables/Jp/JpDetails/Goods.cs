using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.Tables.Dicts;
using Eps.CsToDb.Model.Tables.Eps;

namespace Eps.CsToDb.Model.Tables.Jp.JpDetails
{
    [DbModelPart]
    [DbSchema("Jp")]
    public class GoodsCategories
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        JuridicalPersons REFKEY;

        GoodsCategories REFKEY_GC;
    }
}
