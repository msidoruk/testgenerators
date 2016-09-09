using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;
using Eps.CsToDb.Model.DbModelTemplates.RequireConfirmation;
using Eps.CsToDb.Model.InternalDictionaries;
using Eps.CsToDb.Model.Tables.Eps;

namespace Eps.CsToDb.Model.Tables.Jp.JpDetails
{
    // Таблица 6. Элемент списка Банковских реквизитов Контрагента
    [DbModelPart]
    [DbSchema("Jp")]
    [Trait(typeof(HasActivityRange))]
    [Trait(typeof(WithDescription_200))]
    [Trait(typeof(ConfirmationRequired))]
    public class BankRequisites
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int Key;

        JuridicalPersons REFKEY_JP;

        // Значение поля "Счет ПСД" предполагается вычислять, например, по БИК.

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.BikSize)]
        string BankBik;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.OrganizationNameSize)]
        string BankName;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.AccountNumberSize)]
        string CorrespondentAccount;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.AccountNumberSize)]
        string JuridicalPersonAccount;

        private DocumentScans CreateAccountREFKEY_DS;
    }
}
