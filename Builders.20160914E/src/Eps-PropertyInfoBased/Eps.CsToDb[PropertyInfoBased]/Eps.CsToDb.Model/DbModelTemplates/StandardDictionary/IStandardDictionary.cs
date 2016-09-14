using System;
using System.Data;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Model.DbModelTemplates.StandardDictionary
{
    public interface IStandardDictionary
    {
        [DbColumn(SqlDbType.VarChar, ColumnSize = 100)]
        string Code { get; }

        [DbColumn(SqlDbType.VarChar, ColumnSize = 200)]
        string Description { get; }

        [DbColumn(SqlDbType.Bit)]
        bool IsDefault { get; }

        [DbColumn(SqlDbType.Date)]
        DateTime ActivationDate { get; }

        [DbColumn(SqlDbType.Date)]
        DateTime EndDate { get; }
    }
}
