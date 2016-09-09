using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Model.Tables.Auth.Entities
{
    [DbModelPart]
    [DbSchema("Auth")]
    public class Groups
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        [DbColumn(SqlDbType.VarChar, ColumnSize = 256)]
        string Name;

        [DbColumn(SqlDbType.VarChar, ColumnSize = -1)]
        string Description;
    }
}
