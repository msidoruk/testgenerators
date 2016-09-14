using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.Tables.Auth.Entities;

namespace Eps.CsToDb.Model.Tables.Auth
{
    [DbModelPart]
    [DbSchema("Auth")]

    public class UserSessionVars
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        UserSessions REFKEY;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.UserSessionVariableNameSize)]
        string Name;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.UserSessionVariableValueSize)]
        string Value;
    }
}
