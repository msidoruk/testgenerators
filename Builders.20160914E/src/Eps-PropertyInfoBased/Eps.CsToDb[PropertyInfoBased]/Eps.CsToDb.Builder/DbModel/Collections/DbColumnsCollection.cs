using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Impl;

namespace Eps.CsToDb.Builder.DbModel.Collections
{
    public class DbColumnsCollection : ListBasedCollection<DbColumn>
    {
        public DbColumnsCollection() : base(new List<DbColumn>())
        {
        }

        public void AddColumn(DbColumn dbColumn)
        {
            _underlyingList.Add(dbColumn);
        }
    }
}
