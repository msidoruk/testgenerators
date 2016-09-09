using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Builder.Base
{
    public interface ICsToSqlTypesMapper
    {
        SqlDbType GetSqlType(Type fieldType);
    }
}
