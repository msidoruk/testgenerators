using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eps.CsToDb.Builder.Base
{
    public interface IDbScriptsWriter
    {
        void Write(BuilderContext context);
    }
}
