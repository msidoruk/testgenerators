using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.DbSharedModel.DbModel;

namespace Eps.CsToDb.Builder.Base
{
    public interface IDbSharedModelBuilder
    {
        DbSharedModel.DbModel.DbSharedModel Build(BuilderContext context);
    }
}
