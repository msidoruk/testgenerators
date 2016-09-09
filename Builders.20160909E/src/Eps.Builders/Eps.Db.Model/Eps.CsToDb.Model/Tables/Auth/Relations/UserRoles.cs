using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.Tables.Auth.Entities;

namespace Eps.CsToDb.Model.Tables.Auth.Relations
{
    [DbModelPart]
    [DbSchema("Auth")]
    public class UserRoles
    {
        [DbPrimaryKey]
        Users REFKEY_U;

        [DbPrimaryKey]
        Roles REFKEY_R;
    }
}
