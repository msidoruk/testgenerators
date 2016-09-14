using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Impl;

namespace Eps.CsToDb.Builder.DbModel.Collections
{
    public class DbSchemasCollection : ListBasedCollection<DbSchema>
    {
        public DbSchemasCollection() : base(new List<DbSchema>())
        {
        }

        public void AddSchema(DbSchema dbSchema)
        {
            _underlyingList.Add(dbSchema);
        }
    }
}
