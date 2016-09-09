using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Meta.TablesMarkers.Base
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DbPrimaryKeyAttribute : Attribute
    {
        public bool IsIdentity { get; set; }
    }
}
