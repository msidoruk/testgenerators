using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.InternalDictionary;

namespace Eps.CsToDb.Model.Tables.Jp.Employees
{
    [DbModelPart]
    [DbSchema("Jp")]
    public class EmployeeAddresses
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        Employees REFKEY;

        [InternalDictionary]
        EmployeeAddressTypes AddressType;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeeAddressSize)]
        string Address;
    }
}
