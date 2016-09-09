using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS.DtoMaker.Engine.Helpers
{
    public static class TupleExtentions
    {
        public static T Untuple<T>(this Tuple<bool, T> tuple, Action errorAction)
        {
            if (!tuple.Item1)
            {
                errorAction();
                return default(T);
            }
            return tuple.Item2;
        }
    }
}
