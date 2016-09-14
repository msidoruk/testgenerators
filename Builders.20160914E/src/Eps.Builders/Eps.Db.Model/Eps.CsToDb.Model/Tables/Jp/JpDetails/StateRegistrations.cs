using System;
using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;
using Eps.CsToDb.Model.Tables.Eps;
using Eps.CsToDb.Model.Tables.Jp.Dicts;

namespace Eps.CsToDb.Model.Tables.Jp.JpDetails
{
    // Таблица 5. Элемент списка Сведения о государственной регистрации Контрагента
    [DbModelPart]
    [DbSchema("Jp")]
    [Trait(typeof(HasActivityRange))]
    [Trait(typeof(WithDescription_200))]
    public class StateRegistrations
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        JuridicalPersons REFKEY;

        DateTime RegistrationDate;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegistrationPlaceSize, IsNullable = true)]
        string RegistrationPlace;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegistrationDeptNameSize, IsNullable = true)]
        string RegisteredByDept;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegistrationOgrnSize, IsNullable = true)]
        string OGRN;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegistrationInnSize, IsNullable = true)]
        string INN;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegistrationKppSize, IsNullable = true)]
        string KPP;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegistrationOkatoSize, IsNullable = true)]
        string OKATO;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegistrationOktmoSize, IsNullable = true)]
       string OKTMO;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegistrationOkfsSize, IsNullable = true)]
        string OKFS;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegistrationOkpoSize, IsNullable = true)]
        string OKPO;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.JuridicalPersonStateRegistrationOkopfSize, IsNullable = true)]
        string OKOPF;
    }
}
