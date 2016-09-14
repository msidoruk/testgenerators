using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.Tables.Auth.Entities;

namespace Eps.CsToDb.Model.Tables.Auth.Relations
{
    [DbModelPart]
    [DbSchema("Auth")]
    public class UserClaims
    {
        [DbPrimaryKey]
        Users REFKEY_U;

        [DbPrimaryKey]
        Claims REFKEY_C;
    }
}
