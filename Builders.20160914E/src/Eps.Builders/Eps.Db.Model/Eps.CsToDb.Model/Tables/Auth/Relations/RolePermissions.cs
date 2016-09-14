using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.Tables.Auth.Entities;

namespace Eps.CsToDb.Model.Tables.Auth.Relations
{
    [DbModelPart]
    [DbSchema("Auth")]
    public class RolePermissions
    {
        [DbPrimaryKey]
        Roles REFKEY_R;

        [DbPrimaryKey]
        Permissions REFKEY_P;
    }
}
