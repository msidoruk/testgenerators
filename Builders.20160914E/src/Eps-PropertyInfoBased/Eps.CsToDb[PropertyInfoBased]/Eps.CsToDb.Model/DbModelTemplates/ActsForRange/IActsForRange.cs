using System;
using System.Data;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Model.DbModelTemplates.ActsForRange
{
    public interface IActsForRange
    {
        [DbColumn(SqlDbType.Date)]
        DateTime ActivationDate { get; }
    }
}
