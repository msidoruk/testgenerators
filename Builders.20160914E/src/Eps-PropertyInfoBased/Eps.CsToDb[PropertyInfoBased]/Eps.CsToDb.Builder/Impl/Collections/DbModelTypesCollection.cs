using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Impl;

namespace Eps.CsToDb.Builder.Impl.Collections
{
    public class DbModelTypesCollection : ListBasedCollection<Type>
    {
        public DbModelTypesCollection() : base(new List<Type>())
        {
        }

        public void AddType(Type type)
        {
            _underlyingList.Add(type);
        }
    }
}
