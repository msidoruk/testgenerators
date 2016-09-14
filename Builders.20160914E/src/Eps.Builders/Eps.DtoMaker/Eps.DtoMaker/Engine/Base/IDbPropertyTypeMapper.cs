using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS.DtoMaker.Engine.Base
{
    public interface IDbPropertyTypeMapper
    {
        Type Map<TK, TV>(Dictionary<TK, TV> dbTypes, TK dbTypeId) where TV : DbTypeInfoParser;
    }
}
