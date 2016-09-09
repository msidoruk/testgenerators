using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Meta.TablesMarkers.Base
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DbPrimaryKeyAttribute : Attribute
    {
    }
}
