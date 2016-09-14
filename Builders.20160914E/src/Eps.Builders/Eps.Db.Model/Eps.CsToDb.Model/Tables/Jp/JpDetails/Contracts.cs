using System;
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
    // Таблица 2. Элемент списка договоров с Контрагентом
    [DbModelPart]
    [DbSchema("Jp")]
    [Trait(typeof(WithDescription_200))]
    [Trait(typeof(HasActivityDate))]
    [Trait(typeof(ConfirmationRequired))]
    public class Contracts
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        JuridicalPersons REFKEY;

        JuridicalPersonContactTypes REFKEY_ContactType;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.ContractNumberSize)]
        string ContractNumber;

        // В ТЗ указано что это символьное поле длиной 10 символов
        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.ContractDateSize)]
        string ContractDate;

        DocumentScans ContractREFKEY;

        Employees.Employees SignedREFKEY;

        DateTime TerminationDate;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.ContractTerminationReasonSize)]
        string TerminationReason;

        DocumentScans TerminationREFKEY;
    }
}
