using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS.DtoMaker.Engine.Impl
{
    public class ListBasedCollection<T> : IEnumerable<T> where T : class
    {
        protected readonly List<T> _underlyingList;

        protected ListBasedCollection(List<T> underlyingUnderlyingList)
        {
            _underlyingList = underlyingUnderlyingList;
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var entry in _underlyingList)
            {
                yield return entry;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }

}
