using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Meta;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.CsToDb.Model.DbModelTemplates.StandardDictionary;

namespace Eps.CsToDb.Model.Tables.Dicts
{
    [DbModelPart()]
    [StandardTrait(typeof(IStandardDictionary))]
    public class BusinessAreas
    {
        [DbPrimaryKey(IsIdentity = true)]
        [DbColumn(SqlDbType.Int)]
        public int KEY;
    }
}
