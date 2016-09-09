using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Impl;

namespace Eps.CsToDb.Builder.DbModel.Collections
{
    public class DbTablesCollection : ListBasedCollection<DbTable>
    {
        public DbTablesCollection() : base(new List<DbTable>())
        {
        }

        public void AddTable(DbTable dbTable)
        {
            _underlyingList.Add(dbTable);
        }
    }
}
