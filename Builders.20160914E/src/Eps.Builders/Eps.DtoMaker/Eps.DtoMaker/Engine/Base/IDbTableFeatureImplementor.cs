using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.DbFeatures.Attributes;

namespace EPS.DtoMaker.Engine.Base
{
    public interface IDbTableFeatureImplementor
    {
        bool CheckRequirements(out string errorMessage);
        void Implement(DtoMakerContext dtoMakerContext, DbTableFeatureAttribute dbTableFeatureAttribute);
    }
}
