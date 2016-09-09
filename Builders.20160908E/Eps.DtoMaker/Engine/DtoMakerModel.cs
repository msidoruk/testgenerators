using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Base;
using EPS.DtoMaker.Engine.Impl;

namespace EPS.DtoMaker.Engine
{
    public class ContextBoundModelItem
    {
        public ActionsApplicationHistoryItem ActionsApplicationHistoryItem { get; protected set; }
    }

    public class RawBaseClassSpecification
    {
        public string Name { get; set; }
        public string SourceNamespace { get; set; }
    }

    public class ModelEntity : ContextBoundModelItem
    {
        private DtoMakerContext _context;

        public class StereotypesNames
        {
            public const string IsEnum = ":enum";
            public const string NotDb = ":notDb";
        }

        public Dictionary<string, object> Stereotypes { get; set; }
        public string EntitySpace { get; set; }
        public string EntityName { get; set; }
        public PropertiesCollection Properties { get; private set; }
        public TableLinksCollection EntityLinks { get; set; }
        public OriginalPropertiesNamesCollection OriginalPropertiesNames { get; set; }
        public EntityInstancesCollection Instances { get; set; }
        public List<string> IdentityProperties { get; set; }
        public Dictionary<string, string> AlternativeNames { get; set; }
        public List<RawBaseClassSpecification> RawBaseClasses { get; private set; }

        public List<string> RawMembers { get; private set; }
        public bool IsRepositoryRequired { get; set; }
        public DtoRepository DtoRepository { get; private set; }
        public List<EntityLink> IncomingLinks { get; private set; }
        public ModelEntity(DtoMakerContext context, ActionsApplicationHistoryItem actionsApplicationHistoryItem)
        {
            _context = context;
            Stereotypes = new Dictionary<string, object>();
            Properties = new PropertiesCollection(this);
            EntityLinks = new TableLinksCollection(this);
            OriginalPropertiesNames = new OriginalPropertiesNamesCollection(this);
            Instances = new EntityInstancesCollection(this);
            IdentityProperties = new List<string>();
            AlternativeNames = new Dictionary<string, string>();
            RawBaseClasses = new List<RawBaseClassSpecification>();
            RawMembers = new List<string>();
            DtoRepository = new DtoRepository(_context, this);
            IncomingLinks = new List<EntityLink>();
            //
            ActionsApplicationHistoryItem = actionsApplicationHistoryItem;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat($"[{EntitySpace}.{EntityName}:props:({Properties}),links:({EntityLinks}),originalPropsNames:({OriginalPropertiesNames})]");
            return sb.ToString();
        }

        public bool HasPrimaryKey()
        {
            return IdentityProperties.Count == 1;
        }

        public bool IsItConnectingEntity(ModelEntity modelEntity, out EntityLink connectingLink, out ModelEntity connectedModelEntity)
        {
            connectingLink = null;
            connectedModelEntity = null;
            int propertyWithAssociatedLinksCount = Properties.Count(property => property.AssociatedLink != null);
            if (!HasPrimaryKey() && Properties.Count() == propertyWithAssociatedLinksCount && propertyWithAssociatedLinksCount == 2 /*Сейчас обрабатывается только такой случай'*/)
            {
                foreach (var entityLink in EntityLinks)
                {
                    if (entityLink.ReferencedModelEntity != modelEntity)
                    {
                        connectingLink = entityLink;
                        connectedModelEntity = entityLink.ReferencedModelEntity;
                        break;
                    }
                }
                return connectedModelEntity != null;
            }
            return false;
        }

}

public class EntityProperty
    {
        private ModelEntity _parentEntity;
        public string Name { get; private set; }
        public Type Type { get; private set; }
        public bool IsNullable { get; private set; }
        public Dictionary<string, string> AlternativeNames { get; set; }
        public EntityLink AssociatedLink { get; set; }

        public EntityProperty(ModelEntity parentEntity, string propertyName, Type propertyType, bool isNullable)
        {
            AlternativeNames = new Dictionary<string, string>();

            _parentEntity = parentEntity;
            Name = propertyName;
            Type = propertyType;
            IsNullable = isNullable;
        }
    }

    public class EntityLink
    {
        public ModelEntity ReferencingModelEntity { get; set; }
        public EntityProperty ReferencingPropertySourceProperty { get; set; }
        public string ReferencingPropertySourcePropertyName { get; set; }
        public string ReferencedEntitySpace { get; set; }
        public string ReferencedEntityName { get; set; }
        public string ReferencedEntityPropertyName { get; set; }
        public Dictionary<string, string> AlternativeNames { get; set; }
        public ModelEntity ReferencedModelEntity { get; set; }
        public EntityProperty ReferencedProperty { get; set; }

        public EntityLink()
        {
            AlternativeNames = new Dictionary<string, string>();
        }

        public override string ToString()
        {
            return $"{ReferencingPropertySourcePropertyName ?? "?"} ==> {ReferencedEntitySpace ?? "?"}.{ReferencedEntityName ?? "?"}.{ReferencedEntityPropertyName ?? "?"}";
        }
    }

    public class DtoMakerModel
    {
        public DtoMakerModel()
        {
            Entities = new ModelEntitiesCollection();
        }

        public ModelEntitiesCollection Entities { get; private set; }
    }
}
