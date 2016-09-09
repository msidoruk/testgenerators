﻿using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.Tables.Eps;
using Eps.CsToDb.Model.Tables.Jp.Dicts;

namespace Eps.CsToDb.Model.Tables.Jp.JpDetails
{
    [DbModelPart]
    [DbSchema("Jp")]
    public class StateRegOkveds
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        StateRegistrations REFKEY;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegOkvedSize)]
        string Okved;
    }
}
