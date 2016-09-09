using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Base;

namespace EPS.DtoMaker.Engine.Impl
{
    public class ModelEntitiesCollection : ListBasedCollection<ModelEntity>
    {
        public ModelEntitiesCollection()
            : base(new List<ModelEntity>())
        {
        }

        public void Add(ModelEntity modelEntity)
        {
            _underlyingList.Add(modelEntity);
        }

        public bool DoesEntityExist(string entityName)
        {
            return _underlyingList.Any(modelEntity => modelEntity.EntityName.Equals(entityName));
        }

        public ModelEntity GetEntity(string entitySpace, string entityName)
        {
            return _underlyingList.FirstOrDefault(modelEntity => modelEntity.EntitySpace.Equals(entitySpace, StringComparison.InvariantCultureIgnoreCase) &&
                                                        modelEntity.EntityName.Equals(entityName, StringComparison.InvariantCultureIgnoreCase));
        }
    }

    public class PropertiesCollection : ListBasedCollection<EntityProperty>
    {
        private ModelEntity _parentEntity;

        public PropertiesCollection(ModelEntity parentEntity)
            : base(new List<EntityProperty>())
        {
            _parentEntity = parentEntity;
        }

        public EntityProperty AppendProperty(string propertyName, Type propertyType, bool isNullable)
        {
            var newEntityProperty = new EntityProperty(_parentEntity, propertyName, propertyType, isNullable);
            _underlyingList.Add(newEntityProperty);
            return newEntityProperty;
        }

        public EntityProperty GetProperty(string propertyName)
        {
            return _underlyingList.FirstOrDefault(p => p.Name == propertyName);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var entityProperty in _underlyingList)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                string entityPropertyTypeName = entityProperty.Type != null ? entityProperty.Type.Name : "?";
                sb.Append($"{entityProperty.Name} : {entityPropertyTypeName}");
            }
            return sb.ToString();
        }
    }

    public class TableLinksCollection : ListBasedCollection<EntityLink>
    {
        private ModelEntity _parentEntity;

        public TableLinksCollection(ModelEntity parentEntity)
            : base(new List<EntityLink>())
        {
            _parentEntity = parentEntity;
        }

        public EntityLink AppendTableLink(string sourceProperty, string referencedSpace, string referencedEntity, string referencedProperty)
        {
            var newEntityLink = new EntityLink()
            {
                ReferencingPropertySourcePropertyName = sourceProperty,
                ReferencedEntitySpace = referencedSpace,
                ReferencedEntityName = referencedEntity,
                ReferencedEntityPropertyName = referencedProperty
            };
            _underlyingList.Add(newEntityLink);
            return newEntityLink;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var tableLink in _underlyingList)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append(tableLink);
            }
            return sb.ToString();
        }
    }

    public class OriginalPropertiesNamesCollection : ListBasedCollection<Tuple<String, string>>
    {
        private ModelEntity _parentEntity;

        public OriginalPropertiesNamesCollection(ModelEntity parentEntity)
            : base(new List<Tuple<string,string>>())
        {
            _parentEntity = parentEntity;
        }

        public Tuple<string,string> AppendNames(string originalName, string resultName)
        {
            Tuple<string, string> names = Tuple.Create(originalName, resultName);
            _underlyingList.Add(names);
            return names;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var namesPair in _underlyingList)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append($"{namesPair.Item2} : {namesPair.Item1}");
            }
            return sb.ToString();
        }

        public string FindPropertyName(string propertyName)
        {
            foreach (var s in _underlyingList)
            {
                if (s.Item1 == propertyName)
                    return s.Item2;
            }
            return propertyName;
        }

        public string FindResultPropertyName(string originalPropertyName)
        {
            foreach (var s in _underlyingList)
            {
                if (s.Item2 == originalPropertyName)
                    return s.Item1;
            }
            return originalPropertyName;
        }
    }

    public class EntityInstancesCollection : ListBasedCollection<Dictionary<string, object>>
    {
        private ModelEntity _parentEntity;

        public EntityInstancesCollection(ModelEntity parentEntity)
            : base(new List<Dictionary<string, object>>())
        {
            _parentEntity = parentEntity;
        }

        public void AppendInstance(Dictionary<string, object> instanceProperties)
        {
            _underlyingList.Add(instanceProperties);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var instanceProperties in _underlyingList)
            {
                if (sb.Length > 0)
                    sb.Append(", ");
                sb.Append("DICTIONARY");
            }
            return sb.ToString();
        }
    }
}
