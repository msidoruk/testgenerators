using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Builder.Base
{
    public interface IDbModelAssembliesResolver
    {
        void Resolve(string dbModelAssembliesSearchPath, Func<Assembly, bool> registerModelAssembly);
    }
}
