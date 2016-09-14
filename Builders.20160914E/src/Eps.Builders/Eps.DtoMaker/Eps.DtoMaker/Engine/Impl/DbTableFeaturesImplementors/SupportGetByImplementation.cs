using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eps.DbFeatures.Attributes;
using EPS.DtoMaker.Engine.Base;

namespace EPS.DtoMaker.Engine.Impl.DbTableFeaturesImplementors
{
    public class SupportGetByImplementation : IDbTableFeatureImplementor
    {
        private DtoMakerContext _context;

        public SupportGetByImplementation(DtoMakerContext context)
        {
            _context = context;
        }

        public bool CheckRequirements(out string errorMessage)
        {
            errorMessage = String.Empty;
            return true;
        }

        public void Implement(DtoMakerContext dtoMakerContext, DbTableFeatureAttribute dbTableFeatureAttribute)
        {
        }
    }
}
