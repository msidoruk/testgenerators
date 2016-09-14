using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.Tables.Dicts;
using Eps.CsToDb.Model.Tables.Jp.Employees.Dicts;

namespace Eps.CsToDb.Model.Tables.Jp.Employees
{
    [DbModelPart]
    [DbSchema("Jp")]
    public class EmployeeContacts
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        Jp.Employees.Employees REFKEY;

        EmployeeContactTypes REFKEY_ContactType;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.ContactSize)]
        string ContactValue ;
    }
}
