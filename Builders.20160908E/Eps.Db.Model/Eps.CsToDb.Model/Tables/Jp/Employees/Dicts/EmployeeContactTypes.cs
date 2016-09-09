using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.StandardDictionary;

namespace Eps.CsToDb.Model.Tables.Jp.Employees.Dicts
{
    [DbModelPart()]
    [DbSchema("Dicts")]
    [Trait(typeof(StandardDictionary))]
    public class EmployeeContactTypes
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;
    }
}
