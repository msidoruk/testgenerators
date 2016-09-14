using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Impl;

namespace Eps.CsToDb.Builder.DbModel.Collections
{
    public class ForeignKeysCollection : ListBasedCollection<ForeignKey>
    {
        public ForeignKeysCollection() : base(new List<ForeignKey>())
        {
        }

        public void AddForeignKey(ForeignKey foreignKey)
        {
            _underlyingList.Add(foreignKey);
        }
    }
}
