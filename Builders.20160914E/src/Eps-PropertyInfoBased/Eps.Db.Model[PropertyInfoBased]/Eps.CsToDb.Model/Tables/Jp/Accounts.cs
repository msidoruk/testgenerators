using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.ActsForRange;
using Eps.CsToDb.Model.DbModelTemplates.HasToBeApproved;
using Eps.CsToDb.Model.Tables.Dicts;
using Eps.CsToDb.Model.Tables.Eps;

namespace Eps.CsToDb.Model.Tables.Jp
{
    [DbModelPart()]
    public class Accounts
    {
        [DbPrimaryKey(IsIdentity = true)]
        [DbColumn(SqlDbType.Int)]
        public int KEY;

        [DbColumn(SqlDbType.Int)]
        public JuridicalPerson REFKEY;

        [DbColumn(SqlDbType.Int)]
        public JuridicalPersonAccountType REFKEY_AccountType;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.DescriptionSize)]
        public string Description;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.BikSize)]
        public string Bik;

        [DbColumn(SqlDbType.VarChar, ColumnSize = DbTypes.TypesSizes.AccountNumberSize)]
        public string AccountNumber;
    }
}
