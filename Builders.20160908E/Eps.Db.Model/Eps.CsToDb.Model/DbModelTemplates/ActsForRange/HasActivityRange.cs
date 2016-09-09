using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Model.DbModelTemplates.ActsForRange
{
    public class HasActivityRange
    {
        [DbColumn(SqlDbType.Date)]
        DateTime ActivationDate;

        [DbColumn(SqlDbType.Date)]
        DateTime EndDate;
    }
}
