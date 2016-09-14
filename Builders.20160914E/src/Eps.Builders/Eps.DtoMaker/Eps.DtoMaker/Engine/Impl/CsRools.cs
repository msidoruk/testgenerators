using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Base;
using EPS.DtoMaker.Engine.Helpers;
using Microsoft.CSharp;

namespace EPS.DtoMaker.Engine.Impl
{
    public class CsRools : ICsRools
    {
        private DtoMakerContext _context;
        public void BindToContext(DtoMakerContext context)
        {
            _context = context;
        }

        public string MakeNamespace(string rootNamespace, string entitySpace, string entityName)
        {
            return $"{rootNamespace}.{entitySpace}";
        }

        public string MakeQualifiedClassName(string entitySpace, string className)
        {
            return $"{entitySpace}.{className}";
        }

        public string MakeDtoClassName(ModelEntity entity)
        {
            return _context.LinguisticManager.ToSingular(entity.EntityName);
        }

        public string MakeRepositoryInterfaceName(ModelEntity entity)
        {
            return $"I{entity.EntityName.CapitalizeFirstLetter()}Repository";
        }

        public string MakeRepositoryImplementationClassName(ModelEntity entity)
        {
            return $"{entity.EntityName.CapitalizeFirstLetter()}Repository";
        }

        public string MakePropertyTypeName(Type type)
        {
            string typeName;
            using (var provider = new CSharpCodeProvider())
            {
                var typeRef = new CodeTypeReference(type);
                typeName = provider.GetTypeOutput(typeRef);
            }
            return typeName;
        }

        public string MakeReferencingPropertyName(ModelEntity referencingModelEntity, EntityProperty referencingPropertySourceProperty, string referencedEntity)
        {
            string referencingPropertyName = referencingPropertySourceProperty.Name;
            if (_context.Model.Entities.DoesEntityExist(referencedEntity))
            {
                string modelEntityClassName = referencingModelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
                // Для каждой ссылки модели (столбца, ссылающегося на таблицу) в Dto-классе создается два свойства:
                // - первое, имеющее тип int и соответствующее столбцу, содержащему foreign-ссылку, обычно имя этого столбца (и свойства) имеет суффикс Id;
                // - второе - свойство, имеющее тип, соответствующий целевой таблице (являющейся источником primarykey). Имя ссылочного свойства обычно
                // образуется из первого свойства путем удаления суффикса Id (если таковой имеется). Если же первое свойство не имеет суффикса Id,
                // то имя ссылочного свойства образуется путем добавления суффикса Ref (от 'Reference').
                if (referencingPropertyName.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase))
                {
                    referencingPropertyName = referencingPropertyName.Substring(0, referencingPropertyName.Length - 2);
                }
                else
                {
                    referencingPropertyName = referencingPropertyName + "Ref";
                }
                // Если имя ссылочного свойства совпадает с именем класса, включающего его, то возникает ошибка: 'xxx': member names cannot be the same as their enclosing type'.
                // Для таких случаев пока подставляем суффикс '_'.
                if (referencingPropertyName.Equals(modelEntityClassName))
                    referencingPropertyName = referencingPropertyName + "_";
            }
            return referencingPropertyName;
        }
    }
}
