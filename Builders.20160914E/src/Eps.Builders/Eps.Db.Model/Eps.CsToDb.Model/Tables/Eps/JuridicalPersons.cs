using System.Data;
using System.Data.SqlTypes;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;
using Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved;
using Eps.CsToDb.Model.InternalDictionary;
using Eps.CsToDb.Model.Tables.Dicts;
using Eps.CsToDb.Model.Tables.Jp.Dicts;
using Eps.CsToDb.Model.Tables.Jp.JpDetails;
using Eps.DbFeatures.Attributes;

namespace Eps.CsToDb.Model.Tables.Eps
{
    [TableFilter]
    public class JuridicalPersonsMainFormFilter
    {
        [TableFilterItemInfo(TableType = typeof (Accounts), ColumnName = "AccountNumber")]
        private string AccountNumber;

        [TableFilterItemInfo(TableType = typeof (Contracts), ColumnName = "ContractNumber")]
        private string ContractNumber;
    }

    [DbModelPart()]
    // [DbSchema("eps")]
    [DbTable(DbTableName = "JuridicalPersons")]
    [Trait(typeof(HasToBeApproved))]
    [Trait(typeof(HasActivityDate))]
    [SupportFilteredGet(typeof(JuridicalPersonsMainFormFilter))]
    public abstract class JuridicalPersons
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        [InternalDictionary]
        JuridicalPersonTypes JuridicalPersonType;

        BankDepartments REFKEY_BD;

        BusinessAreas REFKEY_BA;

        JuridicalPersonContractStateTypes REFKEY_CS;

        bool UseBankRko;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.AccountNumberSize)]
        string PsbAccount ;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.CommentsSize)]
        string Comments;

        int AbonentsNumber;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.CommentsSize)]
        string AlternativePaymentChannels;

        JuridicalPersonAbonentsNotificationWayTypes REFKEY_NW;
    }
}
