using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Impl;

namespace Eps.CsToDb.Builder.Impl.Collections
{
    public class DbModelAssembliesCollection : ListBasedCollection<Assembly>
    {
        public DbModelAssembliesCollection() : base(new List<Assembly>())
        {
        }

        public void AddAssembly(Assembly assembly)
        {
            _underlyingList.Add(assembly);
        }
    }
}
