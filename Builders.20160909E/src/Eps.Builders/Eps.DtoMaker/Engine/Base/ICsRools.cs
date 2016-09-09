using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS.DtoMaker.Engine.Base
{
    public interface ICsRools
    {
        void BindToContext(DtoMakerContext context);
        string MakeDtoClassName(ModelEntity entity);
        string MakePropertyTypeName(Type type);
        string MakeReferencingPropertyName(ModelEntity referencingModelEntity, EntityProperty referencingPropertySourceProperty, string referencedEntity);
        string MakeNamespace(string rootNamespace, string entitySpace, string entityName);
        string MakeQualifiedClassName(string referencedEntitySpace, string referencedEntityName);
        string MakeRepositoryInterfaceName(ModelEntity entity);
        string MakeRepositoryImplementationClassName(ModelEntity entity);
    }
}
