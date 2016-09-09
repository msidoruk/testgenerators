using System;
using System.Data;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Model.DbModelTemplates.ActsForRange
{
    public class ActsForRange
    {
        [DbColumn(SqlDbType.Date)] DateTime ActivationDate;
    }
}
