using System;
using System.Data;
using Eps.CsToDb.Meta.TablesMarkers.Base;

// ReSharper disable ArrangeTypeMemberModifiers
// ReSharper disable InconsistentNaming
// ReSharper disable 
#pragma warning disable 169
#pragma warning disable 649
#pragma warning disable 67

namespace Eps.CsToDb.Model.DbModelTemplates.StandardDictionary
{

    public class StandardDictionary
    {
        [DbColumn(SqlDbType.VarChar, ColumnSize = 100)]
        string Code;

        [DbColumn(SqlDbType.VarChar, ColumnSize = 200)]
        string Description;

        [DbColumn(SqlDbType.Bit)]
        bool IsDefault;

        [DbColumn(SqlDbType.Date)]
        DateTime ActivationDate;

        [DbColumn(SqlDbType.Date)]
        DateTime EndDate;
    }
}
