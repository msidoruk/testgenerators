using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Base;
using EPS.DtoMaker.Engine.Impl;
using EPS.DtoMaker.Engine.Helpers;

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
        public int Id { get; private set; }

        public ModelEntity(DtoMakerContext context, ActionsApplicationHistoryItem actionsApplicationHistoryItem, int id)
        {
            Id = id;
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

        public string IdentityPropertyName()
        {
            if (IdentityProperties.Count == 1)
                return IdentityProperties[0];
            if (IdentityProperties.Count == 0)
                throw new Exception($"Entity '{EntityName}' doesn't have identity properties. 'IdentityPropertyName' cannot be used for this case.");
            if (IdentityProperties.Count > 1)
                throw new Exception($"Entity '{EntityName}' have more than one identity property. 'IdentityPropertyName' cannot be used for this case.");
            return string.Empty;
        }

        public bool IsIdentityProperty(string name)
        {
            return IdentityProperties.Contains(name);
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
        public string GetShortVarName(int power = 0, string baseAlternativeName = DtoProjectComposer.ModelNames.CsDtoShortTypeName)
        {
            string result = AlternativeNames[baseAlternativeName].MakeAbbreviationByCapitals().ToLower() + new string('_', power);
            return result.UpdateNameIfItIsReservedWord();
        }
        public string GetVarName(string prefix = "", string suffix = "", int power = 0, string baseAlternativeName = DtoProjectComposer.ModelNames.CsDtoShortTypeName)
        {
            string result = (prefix ?? "") + AlternativeNames[baseAlternativeName].LowercaseFirstLetter() + (suffix ?? "") + new string('_', power);
            return result.UpdateNameIfItIsReservedWord();
        }
        public string GetPluralVarName(string prefix = "", string suffix = "", int power = 0, string baseAlternativeName = DtoProjectComposer.ModelNames.CsDtoShortTypeName)
        {
            string result = (prefix ?? "") + _context.LinguisticManager.ToPlural(AlternativeNames[baseAlternativeName]).LowercaseFirstLetter() + (suffix ?? "") + new string('_', power);
            return result.UpdateNameIfItIsReservedWord();
        }
        public List<ConnectedEntityInfo> GetConnectedEntities()
        {
            List<ConnectedEntityInfo> connectedEntityInfos = new List<ConnectedEntityInfo>();
            foreach (var entityAConnectionLink in IncomingLinks)
            {
                EntityLink entityBConnectionLink;
                ModelEntity connectedModelEntity;
                if (entityAConnectionLink.ReferencingModelEntity.IsItConnectingEntity(this, out entityBConnectionLink,
                    out connectedModelEntity))
                    connectedEntityInfos.Add(new ConnectedEntityInfo()
                    {
                        ConnectedEntity = connectedModelEntity,
                        ConnectingEntity = entityAConnectionLink.ReferencingModelEntity,
                        EntityAConnectinLink = entityAConnectionLink,
                        EntityBConnectinLink = entityBConnectionLink,
                    });
            }
            return connectedEntityInfos;
        }
    }

    public class ConnectedEntityInfo
    {
        public ModelEntity ConnectedEntity { get; set; }
        public ModelEntity ConnectingEntity { get; set; }
        public EntityLink EntityAConnectinLink { get; set; }
        public EntityLink EntityBConnectinLink { get; set; }
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
        private DtoMakerContext _context;
        private int _entitiesIdsCount = 1;

        public ConnectedEntityInfo[,] EntitiesConnectionsMatrix { get; private set; }

        public DtoMakerModel(DtoMakerContext context)
        {
            _context = context;
            Entities = new ModelEntitiesCollection();
        }

        public ModelEntitiesCollection Entities { get; private set; }

        public int EntitiesMinId
        {
            get { return 1; }
        }

        public int EntitiesMaxId
        {
            get { return _entitiesIdsCount - 1; }
        }

        public ModelEntity CreateEntity(ActionsApplicationHistoryItem actionsApplicationHistoryItem)
        {
            ModelEntity newModelEntity = new ModelEntity(_context, actionsApplicationHistoryItem, _entitiesIdsCount++);
            Entities.Add(newModelEntity);
            return newModelEntity;
        }

        public ModelEntity GetEntityById(int entityId)
        {
            return Entities.FirstOrDefault(e => e.Id == entityId);
        }

        public void SealModel()
        {
            BuildEntitiesIncomingLinks();

            BuildEntitiesConnectionsMatrix();
        }

        private void BuildEntitiesIncomingLinks()
        {
            foreach (var modelEntity in Entities)
            {
                foreach (var entityLink in modelEntity.EntityLinks)
                {
                    entityLink.ReferencingModelEntity = modelEntity;
                    entityLink.ReferencingPropertySourceProperty =
                        entityLink.ReferencingModelEntity.Properties.GetProperty(
                            entityLink.ReferencingPropertySourcePropertyName);
                    if (entityLink.ReferencingPropertySourceProperty == null)
                        throw new Exception(
                            $"Property '{entityLink.ReferencingPropertySourcePropertyName}' cannot be found in entity '{entityLink.ReferencingModelEntity.EntityName}'");

                    entityLink.ReferencingPropertySourceProperty.AssociatedLink = entityLink;

                    entityLink.ReferencedModelEntity =
                        _context.Model.Entities.GetEntity(entityLink.ReferencedEntitySpace,
                            entityLink.ReferencedEntityName);
                    if (entityLink.ReferencedModelEntity == null)
                    {
                        string referencingKeyInfo =
                            $"{modelEntity.EntitySpace}.{modelEntity.EntityName}.{entityLink.ReferencingPropertySourcePropertyName}";
                        string referencedEntityInfo =
                            $"{entityLink.ReferencedEntitySpace}.{entityLink.ReferencedEntityName}";
                        throw new Exception(
                            $"Foreign key of '{referencingKeyInfo}' refers to '{referencedEntityInfo}'... " +
                            $"But entity '{referencedEntityInfo}' is unavailable (was not read from Db to internal collection)." +
                            "Please check that it was mentioned in Db tables to read list.");
                    }

                    entityLink.ReferencedModelEntity.IncomingLinks.Add(entityLink);
                }
            }
        }

        private void BuildEntitiesConnectionsMatrix()
        {
            ConnectedEntityInfo[,] entitiesConnectionsMatrix = new ConnectedEntityInfo[Entities.Count() + 1, Entities.Count() + 1];
            foreach (var entity in Entities)
            {
                foreach (var connectedEntityInfo in entity.GetConnectedEntities())
                {
                    entitiesConnectionsMatrix[entity.Id, connectedEntityInfo.ConnectedEntity.Id] = connectedEntityInfo;
                }
            }
            EntitiesConnectionsMatrix = entitiesConnectionsMatrix;
        }

        public LinkedList<List<PathItem>> BuildEntitiesDirectChains(ModelEntity rootEntity)
        {
            ModelEntityChildrenFinder modelEntityChildrenFinder = new ModelEntityChildrenFinder(this);
            return modelEntityChildrenFinder.FindPathes(rootEntity);
        }

        public class ModelEntityChildrenFinder
        {
            private DtoMakerModel _model;
            private readonly LinkedList<PathItem> _workPath = new LinkedList<PathItem>();
            private readonly LinkedList<List<PathItem>> _foundPathes = new LinkedList<List<PathItem>>();

            public ModelEntityChildrenFinder(DtoMakerModel model)
            {
                _model = model;
            }

            public LinkedList<List<PathItem>> FindPathes(ModelEntity rootEntity)
            {
                HashSet<int> skipEntities = new HashSet<int>();

                ScanAndAddConnectedEntities(skipEntities, rootEntity, null);

                return _foundPathes;
            }

            private void ScanAndAddConnectedEntities(HashSet<int> skipEntities, ModelEntity entity, ConnectedEntityInfo connectedEntityInfo)
            {
                skipEntities.Add(entity.Id);

                bool pushInfo = connectedEntityInfo != null;
                if (pushInfo)
                    PushCurrentEntityToWorkPath(entity, connectedEntityInfo);

                for (int currentEntityId = _model.EntitiesMinId; currentEntityId <= _model.EntitiesMaxId; ++currentEntityId)
                {
                    if (skipEntities.Contains(currentEntityId)) // skip processed earlier (root, current, ...)
                        continue;
                    ConnectedEntityInfo currentConnectedEntityInfo = _model.EntitiesConnectionsMatrix[entity.Id, currentEntityId];
                    if (currentConnectedEntityInfo == null) // skip not connected
                        continue;
                    CreatePath(currentEntityId, currentConnectedEntityInfo);
                    ScanAndAddConnectedEntities(skipEntities, _model.GetEntityById(currentEntityId), currentConnectedEntityInfo);
                }

                if (pushInfo)
                    PopCurrentEntityFromWorkPath();

                skipEntities.Remove(entity.Id);
            }

            private void PushCurrentEntityToWorkPath(ModelEntity entity, ConnectedEntityInfo connectedEntityInfo)
            {
                _workPath.AddLast(new PathItem(entity, connectedEntityInfo));
            }

            private void PopCurrentEntityFromWorkPath()
            {
                _workPath.RemoveLast();
            }

            private void CreatePath(int tailEntityId, ConnectedEntityInfo currentConnectedEntityInfo)
            {
                List<PathItem> path = new List<PathItem>();
                foreach (var workPathItem in _workPath)
                {
                    path.Add(new PathItem(workPathItem.ConnectedModelEntity, workPathItem.ConnectedEntityInfo));
                }
                path.Add(new PathItem(_model.GetEntityById(tailEntityId), currentConnectedEntityInfo));
                _foundPathes.AddLast(path);
            }
        }
    }

    public class PathItem
    {
        public int Id => ConnectedModelEntity.Id;
        public string Name => ConnectedModelEntity.EntityName;

        public ModelEntity ConnectedModelEntity { get; private set; }
        public ConnectedEntityInfo ConnectedEntityInfo { get; private set; }

        public PathItem(ModelEntity connectedModelEntity, ConnectedEntityInfo currentConnectedEntityInfo)
        {
            ConnectedModelEntity = connectedModelEntity;
            ConnectedEntityInfo = currentConnectedEntityInfo;
        }
    }
}
