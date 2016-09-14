using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Model.DbModelTemplates
{
    public class WithDescription_200
    {
        [DbColumn(SqlDbType.VarChar, ColumnSize = 200)]
        string Description;
    }
}
