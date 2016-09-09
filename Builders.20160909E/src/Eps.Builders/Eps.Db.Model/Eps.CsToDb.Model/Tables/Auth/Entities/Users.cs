using System;
using System.Data;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;

namespace Eps.CsToDb.Model.Tables.Auth.Entities
{
    [DbModelPart]
    [DbSchema("Auth")]
    public class Users
    {
        [DbPrimaryKey]
        [DbColumn(SqlDbType.Int, IsIdentity = true)]
        int KEY;

        bool IsActive;

        [DbColumn(SqlDbType.NVarChar, ColumnSize = 256)]
        string UserName;

        [DbColumn(SqlDbType.NVarChar, ColumnSize = 256)]
        string FullName;

        int DepartmentId;

        [DbColumn(SqlDbType.NVarChar, ColumnSize = 256)]
        string Email;

        [DbColumn(SqlDbType.NVarChar, ColumnSize = -1)]
        string PhoneNumber;

        [DbColumn(SqlDbType.NVarChar, ColumnSize = -1)]
        string PasswordHash;

        [DbColumn(SqlDbType.NVarChar, ColumnSize = -1)]
        string SecurityStamp;

        DateTime LockoutEndDateUtc;

        bool LockoutEnabled;

        int AccessFailedCount;

        DateTime CreationDat;

        DateTime LastChangeDate;

        bool PasswordChangeRequirement;

        DateTime PasswordLastChangeDate;

        int PersonNumber;

        [DbColumn(SqlDbType.NVarChar, ColumnSize = 256)]
        string LockoutReason;
    }
}
