using System.Collections.Generic;

namespace Eps.CsToDb.Builder.DbModel
{
    public class PrimaryKey
    {
        public List<DbColumn> DbColumns { get; private set; }

        public PrimaryKey()
        {
            DbColumns = new List<DbColumn>();
        }
    }
}
