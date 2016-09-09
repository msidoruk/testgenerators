using System;
using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.InternalDictionary;
using Eps.CsToDb.Model.Tables.Dicts;
using Eps.CsToDb.Model.Tables.Eps;

namespace Eps.CsToDb.Model.Tables.Jp.Employees
{
    [DbModelPart]
    [DbSchema("Jp")]
    public class EmployeeDocumentScans
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        EmployeeDocuments REFKEY_Doc;

        [InternalDictionary]
        EmployeeDocumentScanTypes EmployeeDocumentScanType;

        DocumentScans REFKEY_DocScan;
    }
}
