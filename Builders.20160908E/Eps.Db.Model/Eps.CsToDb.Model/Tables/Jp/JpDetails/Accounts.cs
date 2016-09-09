using System.ComponentModel;
using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;
using Eps.CsToDb.Model.DbModelTemplates.RequireConfirmation;
using Eps.CsToDb.Model.Tables.Eps;
using Eps.CsToDb.Model.Tables.Jp.Dicts;

namespace Eps.CsToDb.Model.Tables.Jp.JpDetails
{
    // Таблица 1.7. Параметры Контрагента. Счета
    [DbModelPart()]
    [DbSchema("Jp")]
    [Trait(typeof(HasActivityRange))]
    [Trait(typeof(WithDescription_200))]
    [Trait(typeof(ConfirmationRequired))]
    public class Accounts
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        JuridicalPersons REFKEY;

        JuridicalPersonAccountTypes REFKEY_AccountType;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.BikSize)]
        string Bik;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.AccountNumberSize)]
        string AccountNumber;
    }
}
