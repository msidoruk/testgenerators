using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.InternalDictionaries;
using Eps.CsToDb.Model.Tables.Auth;
using Eps.CsToDb.Model.Tables.Auth.Entities;

namespace Eps.CsToDb.Model.DbModelTemplates.RequireConfirmation
{
    public class ConfirmationRequired
    {
        [DbColumn(SqlDbType.Date)]
        DateTime CreationDate;

        // [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.DataWithConfirmationAuthorSize)]
        UserSessions REFKEY_US;

        [InternalDictionary]
        ConfirmationStates ConfirmationState;
    }
}
