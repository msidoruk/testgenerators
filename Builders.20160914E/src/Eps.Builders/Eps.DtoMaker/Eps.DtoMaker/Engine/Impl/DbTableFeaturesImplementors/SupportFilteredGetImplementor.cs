using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Eps.CsToDb.Builder.Common;
using Eps.CsToDb.Meta.TablesMarkers.Base;
using Eps.DbFeatures.Attributes;
using EPS.DtoMaker.Engine.Base;

namespace EPS.DtoMaker.Engine.Impl.DbTableFeaturesImplementors
{
    public class SupportFilteredGetImplementor : IDbTableFeatureImplementor
    {
        private DtoMakerContext _context;

        public SupportFilteredGetImplementor(DtoMakerContext context)
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
            SupportFilteredGetAttribute supportFilteredGet = dbTableFeatureAttribute as SupportFilteredGetAttribute;
            if (supportFilteredGet != null)
            {
                if (string.IsNullOrEmpty(supportFilteredGet.FilterTypeName))
                    throw new Exception($"Implementor '{GetType().FullName}' expects FilterTypeName property, but it's empty.");
                Type filterType = Type.GetType(supportFilteredGet.FilterTypeName);
                if (filterType == null)
                    throw new Exception($"Implementor '{GetType().FullName}' cannot find type '{supportFilteredGet.FilterTypeName}'.");

                // todo: IMPLEMENT IMPLEMENTOR LOGIC
                foreach (var fieldInfo in filterType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField))
                {
                    TableFilterItemInfoAttribute tableFilterItemInfo = AttributesHelper.GetAttribute<TableFilterItemInfoAttribute>(fieldInfo);
                }
            }
            else
            {
                string errorMessage = dbTableFeatureAttribute != null
                    ? $"but {dbTableFeatureAttribute.GetType().FullName} received"
                    : "but null received";
                throw new Exception($"Implementor '{GetType().FullName}' expects attribute of type '{typeof (SupportFilteredGetAttribute).FullName}': {errorMessage}.");
            }
        }
    }
}
