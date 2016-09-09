using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.Tables.Eps;
using Eps.CsToDb.Model.Tables.Jp.Dicts;

namespace Eps.CsToDb.Model.Tables.Jp.JpDetails
{
    [DbModelPart]
    [DbSchema("Jp")]
    public class Checks
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        JuridicalPersons REFKEY;

        // Возможны различные типы проверок (в "Таблица 1.4. Параметры Контрагента. Информация" упоминаются ИАС и ДПК)
        JuridicalPersonCheckTypes JuridicalPersonCheckTypeId;

        [DbColumn(SqlDbType.VarChar, ColumnSize = 1000, IsNullable = true)]
        string CheckResult;
    }
}
