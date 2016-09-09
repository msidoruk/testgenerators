using System;
using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.Tables.Dicts;

namespace Eps.CsToDb.Model.Tables.Jp.Employees
{
    [DbModelPart]
    [DbSchema("Jp")]
    public class EmployeeDocuments
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        Jp.Employees.Employees REFKEY;

        EmployeeDocumentTypes REFKEY_DocType;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeeDocumentTypeSize)]
        string DocumentType;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeeDocumentNumberSize)]
        string Number;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeeDocumentIssueLocationSize)]
        string IssueLocation;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.EmployeeDocumentIssuedByDeptSize)]
        string IssuedByDept;

        DateTime IssueDate;

        DateTime ValidUntilDate;
    }
}
