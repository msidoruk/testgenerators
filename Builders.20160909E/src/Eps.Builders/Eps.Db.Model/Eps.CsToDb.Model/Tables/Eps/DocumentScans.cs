using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Model.Tables.Eps
{

    [DbModelPart()]
    public class DocumentScans
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        [DbColumn(SqlDbType.Image)]
        byte[] Image;
    }
}
