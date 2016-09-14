using System;
using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;
using Eps.CsToDb.Model.DbModelTemplates.RequireConfirmation;
using Eps.CsToDb.Model.Tables.Dicts;
using Eps.CsToDb.Model.Tables.Eps;

namespace Eps.CsToDb.Model.Tables.Jp.Employees
{
    // Таблица 3. Элемент списка должностных лиц Контрагента
    // Таблица 8. Элемент списка Контактных лиц Контрагента;
    [DbModelPart]
    [DbSchema("Jp")]
    [Trait(typeof(HasActivityRange))]
    [Trait(typeof(WithDescription_200))]
    [Trait(typeof(ConfirmationRequired))]
    public class Employees
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        private JuridicalPersons REFKEY_JP;

        // Вид контакта
        private ContactEmployeeTypes REFKEY_ET;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeePositionSize)]
        string Position;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeeFirstnameSize)]
        string FirstName;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeeLastnameSize)]
        string LastName;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeeMiddlenameSize)]
        string MiddleName;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeeCitizenshipSize)]
        string Citizenship;

        DateTime BirthDate;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeeBirthplaceSize)]
        string BirthPlace;
    }
}
