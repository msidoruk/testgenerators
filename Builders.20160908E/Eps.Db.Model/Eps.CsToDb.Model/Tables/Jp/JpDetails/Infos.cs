using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;
using Eps.CsToDb.Model.DbModelTemplates.RequireConfirmation;
using Eps.CsToDb.Model.Tables.Dicts;
using Eps.CsToDb.Model.Tables.Eps;
using Eps.CsToDb.Model.Tables.Jp.Dicts;

namespace Eps.CsToDb.Model.Tables.Jp.JpDetails
{
    // Таблица 4. Элемент списка Общих сведений о Контрагенте.
    [DbModelPart]
    [DbSchema("Jp")]
    [Trait(typeof(HasActivityRange))]
    [Trait(typeof(WithDescription_200))]
    [Trait(typeof(ConfirmationRequired))]
    public class Infos
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        JuridicalPersons REFKEY;

        Okopfs REFKEY_Okopfs;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonNameSize, IsNullable = true)]
        string Name;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonShortNameSize, IsNullable = true)]
        string ShortName;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonForeignNameSize, IsNullable = true)]
        string ForeignName;

        bool IsRegisteredInRussia;

        bool MustBeLicensed;

        bool HaveOwner;

        bool ActsForOwnerInterest;

        JuridicalPersonFinancialStates REFKEY_FS;

        BusinessAreas REFKEY_BA;
    }
}
