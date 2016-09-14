using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Eps.DbFeatures.Attributes;
using EPS.DtoMaker.Engine.Base;
using EPS.DtoMaker.Engine.Helpers;
using LinqToDB.Data;

namespace EPS.DtoMaker.Engine.Impl
{
    public class RepositoryMethod
    {
        public string Name { get; set; }

        public string ReturnValueTypeQualifiedName { get; set; }

        public RepositoryMethodParametersCollection Parameters { get; private set; }
        public StringBuilder MethodBody { get; private set; }
        public string GenericSpecification { get; set; }

        public RepositoryMethod()
        {
            Parameters = new RepositoryMethodParametersCollection();
            MethodBody = new StringBuilder();
            GenericSpecification = string.Empty;
        }
    }

    public class MethodParameter
    {
        public enum ParameterDirection
        {
            PdIn = 1,
            PdOut = 2,
            PdRef = 3
        }

        public MethodParameter()
        {
            DefaultValueAsString = string.Empty;
        }

        public string Name { get; set; }
        public string QualifiedTypeName { get; set; }
        public ParameterDirection Direction { get; set; }
        public bool DoSimplifyTypeName { get; set; }
        public string DefaultValueAsString { get; set; }
    }

    public class RepositoryMethodsCollection : ListBasedCollection<RepositoryMethod>
    {
        public RepositoryMethodsCollection()
            : base(new List<RepositoryMethod>())
        {
        }

        public void Add(RepositoryMethod repositoryMethod)
        {
            _underlyingList.Add(repositoryMethod);
        }
    }

    public class RepositoryMethodParametersCollection : ListBasedCollection<MethodParameter>
    {
        public RepositoryMethodParametersCollection()
            : base(new List<MethodParameter>())
        {
        }
        public void Add(MethodParameter methodParameter)
        {
            _underlyingList.Add(methodParameter);
        }
    }

    public class DtoRepository
    {
        private DtoMakerContext _context;
        private readonly ModelEntity _modelEntity;
        private RepositoryMethodsBuilder _repositoryMethodsBuilder;

        public RepositoryMethodsCollection RepositoryMethods { get; private set; }
        public NamespaceTracker SignaturesNamespaceTracker { get; private set; }
        public NamespaceTracker BodyNamespaceTracker { get; set; }

        public DtoRepository(DtoMakerContext context, ModelEntity modelEntity)
        {
            _context = context;
            _modelEntity = modelEntity;

            RepositoryMethods = new RepositoryMethodsCollection();
            SignaturesNamespaceTracker = new NamespaceTracker();
            BodyNamespaceTracker = new NamespaceTracker();

            _repositoryMethodsBuilder = new RepositoryMethodsBuilder(context, modelEntity, this);
        }

        public RepositoryMethod AppendMethod(string methodName)
        {
            RepositoryMethod repositoryMethod = new RepositoryMethod() { Name = methodName };
            RepositoryMethods.Add(repositoryMethod);
            return repositoryMethod;
        }

        public void BuildRepository()
        {
            _repositoryMethodsBuilder.AddListMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddWithMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddDeleteItemMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddUpdateItemMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddCreateItemWithIdentityMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddCreateItemWithIdentityAndGetMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddParameterizedCreateItemWithIdentityMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddParameterizedCreateItemWithIdentityAndGetMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddCreateItemMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            //
            _repositoryMethodsBuilder.AddGetItemsMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            if (_modelEntity.IdentityProperties.Count == 1)
                _repositoryMethodsBuilder.AddGetItemMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            //
            _repositoryMethodsBuilder.AddRunAnyWithConnectionMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddRunWithConnectionMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            //
            List<ConnectedEntityInfo> connectedEntityInfos = _modelEntity.GetConnectedEntities();
            foreach (var connectedEntityInfo in connectedEntityInfos)
            {
                _repositoryMethodsBuilder.AddContainerAddMethod(SignaturesNamespaceTracker, BodyNamespaceTracker, connectedEntityInfo);
                _repositoryMethodsBuilder.AddContainerDeleteMethod(SignaturesNamespaceTracker, BodyNamespaceTracker, connectedEntityInfo);
                _repositoryMethodsBuilder.AddContainerGetChildrenMethod(SignaturesNamespaceTracker, BodyNamespaceTracker, connectedEntityInfo);
            }
            //
            var directChains = _context.Model.BuildEntitiesDirectChains(_modelEntity);
            foreach (var directChain in directChains)
            {
                _repositoryMethodsBuilder.AddGetDirectChainMethod(SignaturesNamespaceTracker, BodyNamespaceTracker, directChain);
            }
            //
            ImplementFeaturesByDbSharedModel();
        }

        private void ImplementFeaturesByDbSharedModel()
        {
            foreach (var dbTableDef in _context.DbSharedModel.DbTables)
            {
                if (dbTableDef.TypedFeatures.Any())
                {
                    foreach (var dbTableFeatureAttribute in dbTableDef.TypedFeatures)
                    {
                        ImplementDbTableFeature(dbTableFeatureAttribute);
                    }
                }
            }
        }

        private void ImplementDbTableFeature(DbTableFeatureAttribute dbTableFeatureAttribute)
        {
            IDbTableFeatureImplementor dbTableFeatureImplementor = GetDbTableFeatureImplementor(dbTableFeatureAttribute);
            if (dbTableFeatureImplementor == null)
                throw new Exception($"DbFeatureImplementor for '{dbTableFeatureAttribute.GetType().Name}' not found");

            dbTableFeatureImplementor.Implement(_context, dbTableFeatureAttribute);
        }

        private IDbTableFeatureImplementor GetDbTableFeatureImplementor(DbTableFeatureAttribute dbTableFeatureAttribute)
        {
            string dbTableFeatureImplementorTypeName = dbTableFeatureAttribute.GetType().FullName;
            IDbTableFeatureImplementor dbTableFeatureImplementor;
            _context.DbTableFeaturesImplementors.TryGetValue(dbTableFeatureImplementorTypeName, out dbTableFeatureImplementor);
            return dbTableFeatureImplementor;
        }

        public class ConnectedEntitiesChains
        {
            public void Build()
            {
            }

            public void GetConnectedEntities()
            {
            }
        }

        public void AppendToChain(ModelEntity modelEntity, List<ModelEntity> chain)
        {
            foreach (var entityAConnectionLink in modelEntity.IncomingLinks)
            {
                EntityLink entityBConnectionLink;
                ModelEntity connectedModelEntity;
                if (entityAConnectionLink.ReferencingModelEntity.IsItConnectingEntity(_modelEntity, out entityBConnectionLink, out connectedModelEntity))
                {
                    if (connectedModelEntity != modelEntity)
                    {
                        chain.Add(connectedModelEntity);
                        AppendToChain(connectedModelEntity, chain);
                    }
                }
            }
        }
    }

    internal class RepositoryMethodsBuilder
    {
        private ModelEntity _modelEntity;
        private DtoMakerContext _context;
        private DtoRepository _dtoRepository;

        public RepositoryMethodsBuilder(DtoMakerContext context, ModelEntity modelEntity, DtoRepository dtoRepository)
        {
            _context = context;
            _modelEntity = modelEntity;
            _dtoRepository = dtoRepository;
        }

        internal void AddRunWithConnectionMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositoryRunWithConnectionMethod = _modelEntity.DtoRepository.AppendMethod("RunWithConnection");
            repositoryRunWithConnectionMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodEnumerableDtoReturnTypeName(_modelEntity);
            //
            string outerDataConnectionParameterQualifiedTypeName = "LinqToDB.Data.DataConnection";
            //
            MethodParameter functionParameter = new MethodParameter
            {
                Direction = MethodParameter.ParameterDirection.PdIn,
                Name = "functionToRun",
                QualifiedTypeName = $"System.Func<{outerDataConnectionParameterQualifiedTypeName},{repositoryRunWithConnectionMethod.ReturnValueTypeQualifiedName}>",
                DoSimplifyTypeName = true
            };
            repositoryRunWithConnectionMethod.Parameters.Add(functionParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter
            {
                Direction = MethodParameter.ParameterDirection.PdIn,
                Name = "outerDataConnection",
                QualifiedTypeName = outerDataConnectionParameterQualifiedTypeName,
                DefaultValueAsString = "null"
            };
            repositoryRunWithConnectionMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder runWithConnectionMethodLogic = new StringBuilder();
            runWithConnectionMethodLogic.Append("return functionToRun(dataConnection);");
            //
            bodyNamespaceTracker.AddNamespace("System.Threading"); // for work with Thread (access to DataConnection via TLS)
            repositoryRunWithConnectionMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetRunWithConnectionMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", runWithConnectionMethodLogic.ToString()));

            repositoryRunWithConnectionMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryRunWithConnectionMethod, bodyNamespaceTracker));
        }

        internal void AddRunAnyWithConnectionMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositoryRunWithConnectionMethod = _modelEntity.DtoRepository.AppendMethod("RunAnyWithConnection");
            repositoryRunWithConnectionMethod.GenericSpecification = "<R>";
            repositoryRunWithConnectionMethod.ReturnValueTypeQualifiedName = "R";
            //
            string outerDataConnectionParameterQualifiedTypeName = "LinqToDB.Data.DataConnection";
            //
            MethodParameter functionParameter = new MethodParameter
            {
                Direction = MethodParameter.ParameterDirection.PdIn,
                Name = "functionToRun",
                QualifiedTypeName = $"System.Func<{outerDataConnectionParameterQualifiedTypeName},R>",
                DoSimplifyTypeName = true
            };
            repositoryRunWithConnectionMethod.Parameters.Add(functionParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter
            {
                Direction = MethodParameter.ParameterDirection.PdIn,
                Name = "outerDataConnection",
                QualifiedTypeName = outerDataConnectionParameterQualifiedTypeName,
                DefaultValueAsString = "null"
            };
            repositoryRunWithConnectionMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder runWithConnectionMethodLogic = new StringBuilder();
            runWithConnectionMethodLogic.Append("return functionToRun(dataConnection);");
            //
            bodyNamespaceTracker.AddNamespace("System.Threading"); // for work with Thread (access to DataConnection via TLS)
            repositoryRunWithConnectionMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetRunAnyWithConnectionMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", runWithConnectionMethodLogic.ToString()));

            repositoryRunWithConnectionMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryRunWithConnectionMethod, bodyNamespaceTracker));
        }

        internal void AddGetItemsMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositorygetItemsMethod = _modelEntity.DtoRepository.AppendMethod($"Get{_context.LinguisticManager.ToPlural(_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName])}");
            repositorygetItemsMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodEnumerableDtoReturnTypeName(_modelEntity);
            //
            MethodParameter loadLinkedObjectsParameter = new MethodParameter();
            loadLinkedObjectsParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            loadLinkedObjectsParameter.Name = "loadLinkedObjects";
            loadLinkedObjectsParameter.QualifiedTypeName = "bool";
            loadLinkedObjectsParameter.DefaultValueAsString = "false";
            repositorygetItemsMethod.Parameters.Add(loadLinkedObjectsParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemsMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder getItemsMethodLogic = new StringBuilder();
            getItemsMethodLogic.Append($@"{{
                if (!loadLinkedObjects) {{
                    return dataConnection.GetTable<{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}>();
                }} else {{
                    return dataConnection.GetTable<{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}>()");
            foreach (var property in _modelEntity.Properties)
            {
                if (property.AssociatedLink != null)
                {
                    getItemsMethodLogic.Append($@"
                    .LoadWith(_ => _.{property.AssociatedLink.AlternativeNames[DtoProjectComposer.ModelNames.CsLinkPropertyName]})");
                }
            }
            getItemsMethodLogic.Append($@";
                }}
            }}");
            //
            repositorygetItemsMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetGetItemsMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", getItemsMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddGetItemMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Get{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodScalarDtoReturnTypeName(_modelEntity);
            //
            string idParameterName =$"{_modelEntity.GetVarName("", "Id")}";
            string primaryKeyPropertyName = $"{_modelEntity.IdentityPropertyName()}";
            string lambdaParameterName = $"{_modelEntity.GetShortVarName()}";
            //
            MethodParameter idParameter = new MethodParameter();
            idParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            idParameter.Name = idParameterName;
            idParameter.QualifiedTypeName = "int";
            repositorygetItemMethod.Parameters.Add(idParameter);
            //
            MethodParameter loadLinkedObjectsParameter = new MethodParameter();
            loadLinkedObjectsParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            loadLinkedObjectsParameter.Name = "loadLinkedObjects";
            loadLinkedObjectsParameter.QualifiedTypeName = "bool";
            loadLinkedObjectsParameter.DefaultValueAsString = "false";
            repositorygetItemMethod.Parameters.Add(loadLinkedObjectsParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);

            listMethodLogic.Append($@"{{
                if (!loadLinkedObjects) {{
                    return dataConnection.GetTable<{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}>().FirstOrDefault({lambdaParameterName}=>{lambdaParameterName}.{primaryKeyPropertyName}=={idParameterName});
                }} else {{
                    return dataConnection.GetTable<{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}>()");
            foreach (var property in _modelEntity.Properties)
            {
                if (property.AssociatedLink != null)
                {
                    listMethodLogic.Append($@"
                        .LoadWith(_ => _.{property.AssociatedLink.AlternativeNames[DtoProjectComposer.ModelNames.CsLinkPropertyName]})");
                }
            }
            listMethodLogic.Append($@"
                        .FirstOrDefault({lambdaParameterName}=>{lambdaParameterName}.{primaryKeyPropertyName}=={idParameterName});
                }}
            }}");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetGetItemMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddCreateItemMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Create{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodScalarIdentityReturnTypeName(_modelEntity);
            //
            string parameterName = $"{_modelEntity.GetVarName()}";
            //
            MethodParameter dtoParameter = new MethodParameter();
            dtoParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoParameter.Name = parameterName;
            dtoParameter.QualifiedTypeName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositorygetItemMethod.Parameters.Add(dtoParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("LinqToDB" /* for DataConnection.Insert */);
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            listMethodLogic.Append($@"dataConnection.Insert({parameterName})");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetCreateItemMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddCreateItemWithIdentityMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Create{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}WithIdentity");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodScalarIdentityReturnTypeName(_modelEntity);
            //
            string parameterName = $"{_modelEntity.GetVarName()}";
            //
            MethodParameter dtoParameter = new MethodParameter();
            dtoParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoParameter.Name = parameterName;
            dtoParameter.QualifiedTypeName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositorygetItemMethod.Parameters.Add(dtoParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("LinqToDB" /* for DataConnection.Insert */);
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            listMethodLogic.Append($@"Convert.ToInt32(dataConnection.InsertWithIdentity({parameterName}))");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetCreateItemWithIdentityMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddCreateItemWithIdentityAndGetMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            string modelEntityClassName = _modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Create{modelEntityClassName}WithIdentityAndGet");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodScalarDtoReturnTypeName(_modelEntity);
            //
            string parameterName = $"{_modelEntity.GetVarName()}";
            //
            MethodParameter dtoParameter = new MethodParameter();
            dtoParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoParameter.Name = parameterName;
            dtoParameter.QualifiedTypeName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositorygetItemMethod.Parameters.Add(dtoParameter);
            //
            MethodParameter loadLinkedObjectsParameter = new MethodParameter();
            loadLinkedObjectsParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            loadLinkedObjectsParameter.Name = "loadLinkedObjects";
            loadLinkedObjectsParameter.QualifiedTypeName = "bool";
            loadLinkedObjectsParameter.DefaultValueAsString = "false";
            repositorygetItemMethod.Parameters.Add(loadLinkedObjectsParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("LinqToDB" /* for DataConnection.Insert */);
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            listMethodLogic.Append($@"Get{modelEntityClassName}(Convert.ToInt32(dataConnection.InsertWithIdentity({parameterName})), loadLinkedObjects, dataConnection)");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetCreateItemWithIdentityMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddParameterizedCreateItemWithIdentityMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            string modelEntityClassName = _modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Create{modelEntityClassName}WithIdentity");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodScalarIdentityReturnTypeName(_modelEntity);
            //
            string lambdaVariableName = $"{_modelEntity.GetVarName()}";
            //
            Dictionary<string, string> propertiesParametersNames = new Dictionary<string, string>();
            foreach (var property in _modelEntity.Properties)
            {
                if (_modelEntity.IsIdentityProperty(property.Name))
                    continue;
                //
                string propertyName = property.AlternativeNames[DtoProjectComposer.ModelNames.CsProperyName];
                string propertyQualifiedTypeName = property.AlternativeNames[DtoProjectComposer.ModelNames.CsPropertyTypeName];
                string propertyTypeName = signatureNamespaceTracker.SimplifyName(propertyQualifiedTypeName, true);
                string propertyParameterName = propertyName.LowercaseFirstLetter();
                //
                propertiesParametersNames[propertyName] = propertyParameterName;
                //
                MethodParameter dtoParameter = new MethodParameter();
                dtoParameter.Direction = MethodParameter.ParameterDirection.PdIn;
                dtoParameter.Name = propertyParameterName;
                dtoParameter.QualifiedTypeName = $"{propertyQualifiedTypeName}";
                dtoParameter.DefaultValueAsString = $"default({propertyTypeName})";
                //
                repositorygetItemMethod.Parameters.Add(dtoParameter);
            }
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            string variableInitializationList = String.Empty;
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("LinqToDB" /* for DataConnection.Insert */);
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            listMethodLogic.AppendLine("{");
            listMethodLogic.Append($"{modelEntityClassName} {lambdaVariableName} = new {modelEntityClassName}({variableInitializationList}) {{");
            int appendedPropertiesIndex = 0;
            foreach (var property in _modelEntity.Properties)
            {
                if (_modelEntity.IsIdentityProperty(property.Name))
                    continue;
                string propertyName = property.AlternativeNames[DtoProjectComposer.ModelNames.CsProperyName];
                string propertyParameterName = propertiesParametersNames[propertyName];
                if (appendedPropertiesIndex > 0)
                    listMethodLogic.AppendLine($",");
                listMethodLogic.Append($"{propertyName} = {propertyParameterName}");
                ++appendedPropertiesIndex;
            }
            listMethodLogic.AppendLine($"}};");
            listMethodLogic.AppendLine($@"return Convert.ToInt32(dataConnection.InsertWithIdentity({lambdaVariableName}));");
            listMethodLogic.AppendLine("}");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetCreateItemWithIdentityMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddListMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            string modelEntityClassName = _modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];

            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"List{_context.LinguisticManager.ToPlural(modelEntityClassName)}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodEnumerableDtoReturnTypeName(_modelEntity);
            //
            List<EntityProperty> valueTypeProperties = new List<EntityProperty>();
            Dictionary<string, string> propertiesParametersNames = new Dictionary<string, string>();
            Dictionary<string, string> propertiesVariablesNamesForAssigningInLambda = new Dictionary<string, string>();
            foreach (var property in _modelEntity.Properties)
            {
                if (_modelEntity.IsIdentityProperty(property.Name))
                    continue;
                //
                string propertyName = property.AlternativeNames[DtoProjectComposer.ModelNames.CsProperyName];
                string propertyQualifiedTypeName =
                    property.AlternativeNames[DtoProjectComposer.ModelNames.CsPropertyTypeName];
                string propertyTypeName = signatureNamespaceTracker.SimplifyName(propertyQualifiedTypeName, true);
                bool itIsDateTimeProperty = propertyTypeName.IndexOf("DateTime", StringComparison.InvariantCultureIgnoreCase) >= 0 || property.Type.IsValueType;

                string propertyParameterName = propertyName.LowercaseFirstLetter();
                string propertyParameterNameForAssigningInLambda = itIsDateTimeProperty
                    ? propertyParameterName + "_"
                    : propertyParameterName;
                //
                if (itIsDateTimeProperty || property.Type.IsValueType)
                    valueTypeProperties.Add(property);
                //
                propertiesParametersNames[propertyName] = propertyParameterName;
                propertiesVariablesNamesForAssigningInLambda[propertyName] = propertyParameterNameForAssigningInLambda;
                //
                MethodParameter dtoParameter = new MethodParameter();
                dtoParameter.Direction = MethodParameter.ParameterDirection.PdIn;
                dtoParameter.Name = propertyParameterName;
                dtoParameter.QualifiedTypeName = $"{propertyQualifiedTypeName}";
                dtoParameter.DefaultValueAsString = $"default({propertyTypeName})";
                //
                repositorygetItemMethod.Parameters.Add(dtoParameter);
            }
            //
            MethodParameter loadLinkedObjectsParameter = new MethodParameter();
            loadLinkedObjectsParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            loadLinkedObjectsParameter.Name = "loadLinkedObjects";
            loadLinkedObjectsParameter.QualifiedTypeName = "bool";
            loadLinkedObjectsParameter.DefaultValueAsString = "false";
            repositorygetItemMethod.Parameters.Add(loadLinkedObjectsParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            string variableInitializationList = String.Empty;
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("LinqToDB" /* for DataConnection.Insert */);
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            bodyNamespaceTracker.AddNamespace("System.Text" /* for StringBuilder */);
            listMethodLogic.AppendLine("{");
            if (valueTypeProperties.Count > 0)
            {
                foreach (var valueTypeProperty in valueTypeProperties)
                {
                    string valueTypePropertyType =
                        valueTypeProperty.AlternativeNames[DtoProjectComposer.ModelNames.CsPropertyTypeName];
                    listMethodLogic.AppendLine($@"
                    {valueTypePropertyType}? {propertiesVariablesNamesForAssigningInLambda[valueTypeProperty.Name]} = new {valueTypePropertyType}?();");
                    listMethodLogic.AppendLine($@"
                    if ({propertiesParametersNames[valueTypeProperty.Name]} != default({valueTypePropertyType}))
                       {propertiesVariablesNamesForAssigningInLambda[valueTypeProperty.Name]} = {propertiesParametersNames[valueTypeProperty.Name]};");
                }
                listMethodLogic.AppendLine("");
            }
            listMethodLogic.AppendLine(@"
                List<DataParameter> parameters = new List<DataParameter>();
            ");

            DataParameter dp = new DataParameter() {};

            listMethodLogic.AppendLine(@"
                StringBuilder whereStatementStringBuilder = new StringBuilder();
                int wherePropertyIndex = 0;
                ");

            string quote = "\"";
            string quotedAnd = "\" and \"";

            foreach (var property in _modelEntity.Properties)
            {
                if (_modelEntity.IsIdentityProperty(property.Name))
                    continue;
                if (property.Type.IsValueType)
                {
                    string quotedPropertyName = $"\"@{propertiesParametersNames[property.Name]}\"";
                    listMethodLogic.AppendLine($@"
                    if ({propertiesVariablesNamesForAssigningInLambda[property.Name]}.HasValue) {{
                        if (wherePropertyIndex > 0)
                            whereStatementStringBuilder.Append({quotedAnd});
                        parameters.Add(new DataParameter() {{ Name = {quotedPropertyName}, Value = {propertiesVariablesNamesForAssigningInLambda[property.Name]} }});
                        whereStatementStringBuilder.AppendLine({quote}{property.Name} = @{propertiesParametersNames[property.Name]}{quote});
                        ++wherePropertyIndex;
                    }}");
                }
                else
                {
                    string quotedPropertyName = $"\"@{propertiesParametersNames[property.Name]}\"";
                    listMethodLogic.AppendLine($@"
                    if ({propertiesParametersNames[property.Name]} != null) {{
                        if (wherePropertyIndex > 0)
                            whereStatementStringBuilder.Append({quotedAnd});
                        parameters.Add(new DataParameter() {{ Name = {quotedPropertyName}, Value = {propertiesParametersNames[property.Name]} }});
                        whereStatementStringBuilder.AppendLine({quote}{property.Name} = @{propertiesParametersNames[property.Name]}{quote});
                        ++wherePropertyIndex;
                    }}");
                }
            }

            listMethodLogic.AppendLine($@"
                string queryString = {quote}select * from {_modelEntity.EntitySpace}.{_modelEntity.EntityName} {quote} + ((wherePropertyIndex > 0) ? {quote}where {quote} + whereStatementStringBuilder.ToString() : string.Empty);
                ");

            listMethodLogic.AppendLine($"return dataConnection.Query<{modelEntityClassName}>(queryString, parameters.ToArray()).ToList();");
            listMethodLogic.AppendLine("}");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetListMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddParameterizedCreateItemWithIdentityAndGetMethod(NamespaceTracker signatureNamespaceTracker,
            NamespaceTracker bodyNamespaceTracker)
        {
            string modelEntityClassName =
                _modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
            RepositoryMethod repositorygetItemMethod =
                _modelEntity.DtoRepository.AppendMethod($"Create{modelEntityClassName}WithIdentityAndGet");
            repositorygetItemMethod.ReturnValueTypeQualifiedName =
                GetRepositoryMethodScalarDtoReturnTypeName(_modelEntity);
            //
            string lambdaVariableName = $"{_modelEntity.GetVarName()}";
            //
            List<EntityProperty> dateTimeProperties = new List<EntityProperty>();
            Dictionary<string, string> propertiesParametersNames = new Dictionary<string, string>();
            Dictionary<string, string> propertiesVariablesNamesForAssigningInLambda = new Dictionary<string, string>();
            foreach (var property in _modelEntity.Properties)
            {
                if (_modelEntity.IsIdentityProperty(property.Name))
                    continue;
                //
                string propertyName = property.AlternativeNames[DtoProjectComposer.ModelNames.CsProperyName];
                string propertyQualifiedTypeName =
                    property.AlternativeNames[DtoProjectComposer.ModelNames.CsPropertyTypeName];
                string propertyTypeName = signatureNamespaceTracker.SimplifyName(propertyQualifiedTypeName, true);
                bool itIsDateTimeProperty = propertyTypeName.IndexOf("DateTime", StringComparison.InvariantCultureIgnoreCase) >= 0;
                
                string propertyParameterName = propertyName.LowercaseFirstLetter();
                string propertyParameterNameForAssigningInLambda = itIsDateTimeProperty
                    ? propertyParameterName + "_"
                    : propertyParameterName;
                //
                if (itIsDateTimeProperty)
                    dateTimeProperties.Add(property);
                //
                propertiesParametersNames[propertyName] = propertyParameterName;
                propertiesVariablesNamesForAssigningInLambda[propertyName] = propertyParameterNameForAssigningInLambda;
                //
                MethodParameter dtoParameter = new MethodParameter();
                dtoParameter.Direction = MethodParameter.ParameterDirection.PdIn;
                dtoParameter.Name = propertyParameterName;
                dtoParameter.QualifiedTypeName = $"{propertyQualifiedTypeName}";
                dtoParameter.DefaultValueAsString = $"default({propertyTypeName})";
                //
                repositorygetItemMethod.Parameters.Add(dtoParameter);
            }
            //
            MethodParameter loadLinkedObjectsParameter = new MethodParameter();
            loadLinkedObjectsParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            loadLinkedObjectsParameter.Name = "loadLinkedObjects";
            loadLinkedObjectsParameter.QualifiedTypeName = "bool";
            loadLinkedObjectsParameter.DefaultValueAsString = "false";
            repositorygetItemMethod.Parameters.Add(loadLinkedObjectsParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            string variableInitializationList = String.Empty;
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("LinqToDB" /* for DataConnection.Insert */);
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            listMethodLogic.AppendLine("{");
            if (dateTimeProperties.Count > 0) { 
                foreach (var dateTimeProperty in dateTimeProperties)
                {
                    listMethodLogic.AppendLine(
                        $"DateTime? {propertiesVariablesNamesForAssigningInLambda[dateTimeProperty.Name]} = new DateTime?();");
                    listMethodLogic.AppendLine(
                        $@"if ({propertiesParametersNames[dateTimeProperty.Name]} != default(DateTime))
                            {propertiesVariablesNamesForAssigningInLambda[dateTimeProperty.Name]} = {propertiesParametersNames[dateTimeProperty.Name]};");
                }
                listMethodLogic.AppendLine("");
            }
            listMethodLogic.Append($"{modelEntityClassName} {lambdaVariableName} = new {modelEntityClassName}({variableInitializationList}) {{");
            int appendedPropertiesIndex = 0;
            foreach (var property in _modelEntity.Properties)
            {
                if (_modelEntity.IsIdentityProperty(property.Name))
                    continue;
                string propertyName = property.AlternativeNames[DtoProjectComposer.ModelNames.CsProperyName];
                string propertyParameterName = propertiesVariablesNamesForAssigningInLambda[propertyName];
                if (appendedPropertiesIndex > 0)
                    listMethodLogic.AppendLine($",");
                listMethodLogic.Append($"{propertyName} = {propertyParameterName}");
                ++appendedPropertiesIndex;
            }
            listMethodLogic.AppendLine($"}};");
            listMethodLogic.AppendLine($@"return Get{modelEntityClassName}(Convert.ToInt32(dataConnection.InsertWithIdentity({lambdaVariableName})), loadLinkedObjects, dataConnection);");
            listMethodLogic.AppendLine("}");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetCreateItemWithIdentityMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddDeleteItemMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Delete{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodVoidReturnTypeName(_modelEntity);
            //
            string primaryKeyPropertyName = $"{_modelEntity.IdentityPropertyName()}";
            string lambdaParameterName = $"{_modelEntity.GetShortVarName()}";
            string parameterName = $"{_modelEntity.GetVarName()}";
            //
            MethodParameter dtoParameter = new MethodParameter();
            dtoParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoParameter.Name = parameterName;
            dtoParameter.QualifiedTypeName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositorygetItemMethod.Parameters.Add(dtoParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            listMethodLogic.AppendLine($@"dataConnection.GetTable<{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}>()");
            listMethodLogic.AppendLine($@".Delete({lambdaParameterName}=>{lambdaParameterName}.{primaryKeyPropertyName} == {parameterName}.{primaryKeyPropertyName})");
            // FirstOrDefault({ lambdaParameterName}=>{ lambdaParameterName}.{ primaryKeyPropertyName}=={ idParameterName})");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetDeleteItemMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddUpdateItemMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Update{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodVoidReturnTypeName(_modelEntity);
            //
            string primaryKeyPropertyName = $"{_modelEntity.IdentityPropertyName()}";
            string lambdaParameterName = $"{_modelEntity.GetShortVarName()}";
            string parameterName = $"{_modelEntity.GetVarName()}";
            //
            MethodParameter dtoParameter = new MethodParameter();
            dtoParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoParameter.Name = parameterName;
            dtoParameter.QualifiedTypeName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositorygetItemMethod.Parameters.Add(dtoParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder updateItemMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            updateItemMethodLogic.AppendLine($@"dataConnection.GetTable<{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}>()");
            updateItemMethodLogic.AppendLine($@".Where({lambdaParameterName}=>{lambdaParameterName}.{primaryKeyPropertyName} == {parameterName}.{primaryKeyPropertyName})");
            int valuablePropertiesCount = 0;
            foreach (var property in _modelEntity.Properties)
            {
                if (!property.Name.Equals(primaryKeyPropertyName))
                {
                    updateItemMethodLogic.AppendLine($@".Set({lambdaParameterName}=>{lambdaParameterName}.{property.Name}, {parameterName}.{property.Name})");
                    ++valuablePropertiesCount;
                }
            }
            updateItemMethodLogic.AppendLine($@".Update()");
            if (valuablePropertiesCount == 0)
            {
                updateItemMethodLogic.Clear();
                updateItemMethodLogic.Append("\"\"");
            }
            // FirstOrDefault({ lambdaParameterName}=>{ lambdaParameterName}.{ primaryKeyPropertyName}=={ idParameterName})");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetUpdateItemMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", updateItemMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        public void AddContainerAddMethod(NamespaceTracker signaturesNamespaceTracker, NamespaceTracker bodyNamespaceTracker, ConnectedEntityInfo connectedEntityInfo)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Add{connectedEntityInfo.ConnectedEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodScalarBoolReturnTypeName(_modelEntity);
            //
            string entityAIdentityPropertyName = $"{_modelEntity.OriginalPropertiesNames.FindResultPropertyName(_modelEntity.IdentityPropertyName())}";
            string entityBIdentityPropertyName = $"{connectedEntityInfo.ConnectedEntity.OriginalPropertiesNames.FindResultPropertyName(connectedEntityInfo.ConnectedEntity.IdentityPropertyName())}";
            string entityAParameterName = $"{_modelEntity.GetVarName()}";
            string entityBParameterName = $"{connectedEntityInfo.ConnectedEntity.GetVarName()}";
            string lambdaParameterName = $"{connectedEntityInfo.ConnectingEntity.GetShortVarName()}";
            string connectingModelEntityClassName = connectedEntityInfo.ConnectingEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
            //
            MethodParameter dtoEntityAParameter = new MethodParameter();
            dtoEntityAParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoEntityAParameter.Name = entityAParameterName;
            dtoEntityAParameter.QualifiedTypeName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositorygetItemMethod.Parameters.Add(dtoEntityAParameter);
            //
            MethodParameter dtoEntityBParameter = new MethodParameter();
            dtoEntityBParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoEntityBParameter.Name = entityBParameterName;
            dtoEntityBParameter.QualifiedTypeName = $"{connectedEntityInfo.ConnectedEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositorygetItemMethod.Parameters.Add(dtoEntityBParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            listMethodLogic.AppendLine("{");
            listMethodLogic.AppendLine($@"{connectingModelEntityClassName} {lambdaParameterName} = new {connectingModelEntityClassName}();");
            listMethodLogic.AppendLine($@"{lambdaParameterName}.{entityAIdentityPropertyName} = {entityAParameterName}.{entityAIdentityPropertyName};");
            listMethodLogic.AppendLine($@"{lambdaParameterName}.{entityBIdentityPropertyName} = {entityBParameterName}.{entityBIdentityPropertyName};");
            listMethodLogic.AppendLine($@"dataConnection.Insert({lambdaParameterName});");
            listMethodLogic.AppendLine($@"return true;");
            listMethodLogic.Append("}");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetContainerAddMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        public void AddContainerDeleteMethod(NamespaceTracker signaturesNamespaceTracker, NamespaceTracker bodyNamespaceTracker, ConnectedEntityInfo connectedEntityInfo)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Delete{connectedEntityInfo.ConnectedEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodScalarBoolReturnTypeName(_modelEntity);
            //
            string entityAIdentityPropertyName = $"{_modelEntity.OriginalPropertiesNames.FindResultPropertyName(_modelEntity.IdentityPropertyName())}";
            string entityBIdentityPropertyName = $"{connectedEntityInfo.ConnectedEntity.OriginalPropertiesNames.FindResultPropertyName(connectedEntityInfo.ConnectedEntity.IdentityPropertyName())}";
            string entityAParameterName = $"{_modelEntity.GetVarName()}";
            string entityBParameterName = $"{connectedEntityInfo.ConnectedEntity.GetVarName()}";
            string lambdaParameterName = $"{connectedEntityInfo.ConnectingEntity.GetShortVarName()}";
            string connectingModelEntityClassName = connectedEntityInfo.ConnectingEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
            //
            MethodParameter dtoEntityAParameter = new MethodParameter();
            dtoEntityAParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoEntityAParameter.Name = entityAParameterName;
            dtoEntityAParameter.QualifiedTypeName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositorygetItemMethod.Parameters.Add(dtoEntityAParameter);
            //
            MethodParameter dtoEntityBParameter = new MethodParameter();
            dtoEntityBParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoEntityBParameter.Name = entityBParameterName;
            dtoEntityBParameter.QualifiedTypeName = $"{connectedEntityInfo.ConnectedEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositorygetItemMethod.Parameters.Add(dtoEntityBParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            listMethodLogic.AppendLine("{");
            listMethodLogic.AppendLine($@"{connectingModelEntityClassName} {lambdaParameterName} = new {connectingModelEntityClassName}();");
            listMethodLogic.AppendLine($@"{lambdaParameterName}.{entityAIdentityPropertyName} = {entityAParameterName}.{entityAIdentityPropertyName};");
            listMethodLogic.AppendLine($@"{lambdaParameterName}.{entityBIdentityPropertyName} = {entityBParameterName}.{entityBIdentityPropertyName};");
            listMethodLogic.AppendLine($@"dataConnection.Delete({lambdaParameterName});");
            listMethodLogic.AppendLine($@"return true;");
            listMethodLogic.Append("}");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetContainerDeleteMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));

        }

        public void AddContainerGetChildrenMethod(NamespaceTracker signaturesNamespaceTracker, NamespaceTracker bodyNamespaceTracker, ConnectedEntityInfo connectedEntityInfo)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Get{_context.LinguisticManager.ToPlural(connectedEntityInfo.ConnectedEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName])}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodEnumerableDtoReturnTypeName(connectedEntityInfo.ConnectedEntity);
            //
            string entityAIdentityPropertyName = $"{_modelEntity.OriginalPropertiesNames.FindResultPropertyName(_modelEntity.IdentityPropertyName())}";
            string entityBIdentityPropertyName = $"{connectedEntityInfo.ConnectedEntity.OriginalPropertiesNames.FindResultPropertyName(connectedEntityInfo.ConnectedEntity.IdentityPropertyName())}";
            string entityAParameterName = $"{_modelEntity.GetVarName()}";
            string entityBParameterName = $"{connectedEntityInfo.ConnectedEntity.GetVarName()}";
            string connectingModelEntityClassName = connectedEntityInfo.ConnectingEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
            string connectedModelEntityClassName = connectedEntityInfo.ConnectedEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
            //
            MethodParameter dtoEntityAParameter = new MethodParameter();
            dtoEntityAParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoEntityAParameter.Name = entityAParameterName;
            dtoEntityAParameter.QualifiedTypeName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositorygetItemMethod.Parameters.Add(dtoEntityAParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder listMethodLogic = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);
            // Некоторые имена переменных не приемлемы в языке выражений linq (например, group), поэтому модифицируем имена для их использования в linq.
            string entityAParameterNameForLinq = entityAParameterName + "_";
            string entityBParameterNameForLinq = entityBParameterName + "_";
            string linqOuterVariableName = connectedEntityInfo.ConnectingEntity.GetShortVarName(0);
            string linqInnerVariableName = connectedEntityInfo.ConnectedEntity.GetShortVarName(0);
            listMethodLogic.Append("{");
            listMethodLogic.AppendLine($@"
                    var {entityAParameterNameForLinq} = {entityAParameterName};
                    var query =
                        from {linqOuterVariableName} in dataConnection.GetTable<{connectingModelEntityClassName}>().Where(_ => _.{entityAIdentityPropertyName} == {entityAParameterNameForLinq}.{entityAIdentityPropertyName})
                        join {linqInnerVariableName} in dataConnection.GetTable<{connectedModelEntityClassName}>() on {linqOuterVariableName}.{entityBIdentityPropertyName} equals {linqInnerVariableName}.{entityBIdentityPropertyName}
                        select {linqInnerVariableName};");
            listMethodLogic.AppendLine($@"return query.ToList();");
            listMethodLogic.Append("}");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetContainerGetChildrenMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));

        }
        public void AddGetDirectChainMethod(NamespaceTracker signaturesNamespaceTracker, NamespaceTracker bodyNamespaceTracker, List<PathItem> directChain)
        {
            ModelEntity connectedEntity = directChain[directChain.Count - 1].ConnectedModelEntity;
            StringBuilder methodNameStringBuilder = new StringBuilder();
            methodNameStringBuilder.Append("ChainedGet");
            foreach (var pathItem in directChain)
            {
                methodNameStringBuilder.Append(pathItem.Name);
            }
            string methodName = methodNameStringBuilder.ToString();
            RepositoryMethod repositoryDirectChainGetMethod = _modelEntity.DtoRepository.AppendMethod($"{methodName}");
            repositoryDirectChainGetMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodEnumerableDtoReturnTypeName(connectedEntity);

            string entityAParameterName = $"{_modelEntity.GetVarName()}";

            MethodParameter dtoEntityAParameter = new MethodParameter();
            dtoEntityAParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            dtoEntityAParameter.Name = entityAParameterName;
            dtoEntityAParameter.QualifiedTypeName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
            repositoryDirectChainGetMethod.Parameters.Add(dtoEntityAParameter);

            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositoryDirectChainGetMethod.Parameters.Add(outerDataConnectionParameter);

            StringBuilder repositoryDirectChainGetMethodBody = new StringBuilder();
            bodyNamespaceTracker.AddNamespace("System.Linq" /* for FirstOrDefault */);

            repositoryDirectChainGetMethodBody.Append("{");
            string entityAParameterNameForLinq = entityAParameterName + "_";
            repositoryDirectChainGetMethodBody.AppendLine($@"
                   var {entityAParameterNameForLinq} = {entityAParameterName};");

            string linqExpressionResultTypeName = string.Empty;
            if (directChain.Count > 0)
            {
                LinkedList<ConnectionSchemeItem> connectionsScheme = new LinkedList<ConnectionSchemeItem>();
                PathItem prevPathItem = null;
                for (int pathItemIndex = 0; pathItemIndex < directChain.Count; ++pathItemIndex)
                {
                    PathItem pathItem = directChain[pathItemIndex];

                    connectionsScheme.AddLast(new ConnectionSchemeItem()
                    {
                        Entity = pathItem.ConnectedEntityInfo.ConnectingEntity,
                        IdentityAName = pathItem.ConnectedEntityInfo.EntityAConnectinLink.ReferencingPropertySourcePropertyName,
                        IdentityBName = pathItem.ConnectedEntityInfo.EntityAConnectinLink.ReferencingPropertySourcePropertyName,
                        PathItem = pathItem,
                        PrevPathItem = prevPathItem,
                        IsLast = false
                    });

                    if (pathItemIndex == directChain.Count - 1)
                    {
                        connectionsScheme.AddLast(new ConnectionSchemeItem()
                        {
                            Entity = pathItem.ConnectedEntityInfo.ConnectedEntity,
                            IdentityAName = pathItem.ConnectedEntityInfo.EntityBConnectinLink.ReferencedEntityPropertyName,
                            IdentityBName = pathItem.ConnectedEntityInfo.EntityBConnectinLink.ReferencedEntityPropertyName,
                            PathItem = pathItem,
                            PrevPathItem = prevPathItem,
                            IsLast = true
                        });

                        linqExpressionResultTypeName = pathItem.ConnectedEntityInfo.ConnectedEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
                    }

                    prevPathItem = pathItem;
                }

                string outerItem = _modelEntity.GetVarName();
                string outerCollection = _modelEntity.GetPluralVarName();
                string lastItemName = string.Empty;
                int index = 0;
                while (connectionsScheme.Count > 0)
                {
                    ConnectionSchemeItem connectionSchemeItem = connectionsScheme.First();
                    connectionsScheme.RemoveFirst();

                    if (index == 0)
                    {
                        repositoryDirectChainGetMethodBody.AppendLine($@"
                        var {connectionSchemeItem.Entity.GetPluralVarName()} = dataConnection.GetTable<{connectionSchemeItem.Entity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}>().Where(_ => _.{_modelEntity.IdentityPropertyName()} == {entityAParameterNameForLinq}.{_modelEntity.IdentityPropertyName()});
                        ");
                        repositoryDirectChainGetMethodBody.Append($@"
                        var query =");

                        outerItem = connectionSchemeItem.Entity.GetVarName();
                        outerCollection = connectionSchemeItem.Entity.GetPluralVarName();

                        ++index;
                        continue;
                    }

                    string innerTable = connectionSchemeItem.Entity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
                    string innerItem = connectionSchemeItem.Entity.GetVarName() + "_";
                    string innerCollection = connectionSchemeItem.Entity.GetPluralVarName();

                    repositoryDirectChainGetMethodBody.Append($@"
                    from {outerItem} in {outerCollection}
                    join {innerItem} in dataConnection.GetTable<{innerTable}>() on {outerItem}.{connectionSchemeItem.IdentityAName} equals {innerItem}.{connectionSchemeItem.IdentityAName}");
                    if (!connectionSchemeItem.IsLast)
                        repositoryDirectChainGetMethodBody.AppendLine($@" into {innerCollection}");

                    outerItem = connectionSchemeItem.Entity.GetVarName() + "_";
                    outerCollection = connectionSchemeItem.Entity.GetPluralVarName();

                    lastItemName = innerItem;

                    ++index;
                }

                repositoryDirectChainGetMethodBody.AppendLine($@"
                    select {lastItemName};");
            }
            repositoryDirectChainGetMethodBody.AppendLine($@"return query.ToList();");
            repositoryDirectChainGetMethodBody.Append("}");

            repositoryDirectChainGetMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetDirectChainGetMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine()
                                                                    .Replace("$METHOD_LOGIC$", repositoryDirectChainGetMethodBody.ToString())
                                                                    .Replace("$METHOD_DTO_CLASS_NAME$", linqExpressionResultTypeName));
        }

        class ConnectionSchemeItem
        {
            public ModelEntity Entity { get; set; }
            public PathItem PathItem { get; set; }
            public PathItem PrevPathItem { get; set; }
            public bool IsLast { get; set; }
            public string IdentityAName { get; set; }
            public string IdentityBName { get; set; }
        }

        internal void AddWithMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            string modelEntityClassName = _modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
            //
            RepositoryMethod repositorygetItemsMethod = _modelEntity.DtoRepository.AppendMethod($"With{_context.LinguisticManager.ToPlural(modelEntityClassName)}");
            repositorygetItemsMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodEnumerableDtoReturnTypeName(_modelEntity);
            //
            MethodParameter funcParameter = new MethodParameter();
            funcParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            funcParameter.Name = "func";
            funcParameter.QualifiedTypeName = $"Func<ITable<{modelEntityClassName}>, {repositorygetItemsMethod.ReturnValueTypeQualifiedName}>";
            funcParameter.DoSimplifyTypeName = true;
            repositorygetItemsMethod.Parameters.Add(funcParameter);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemsMethod.Parameters.Add(outerDataConnectionParameter);
            //
            signatureNamespaceTracker.AddNamespace("LinqToDB" /* for ITable<> in interface specification */);
            StringBuilder getItemsMethodLogic = new StringBuilder();
            getItemsMethodLogic.Append($@"func(dataConnection.GetTable<{modelEntityClassName}>())");
            //
            repositorygetItemsMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetGetItemsMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", getItemsMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        private string GetRepositoryMethodScalarBoolReturnTypeName(ModelEntity modelEntity)
        {
            return "bool";
        }

        public void AddContainerDeleteMethod(NamespaceTracker signaturesNamespaceTracker, NamespaceTracker bodyNamespaceTracker, EntityLink connectingLink, ModelEntity connectedModelEntity)
        {
        }

        private string GetRepositoryMethodVoidReturnTypeName(ModelEntity modelEntity)
        {
            return "void";
        }

        private string GetRepositoryMethodScalarIdentityReturnTypeName(ModelEntity modelEntity)
        {
            return "int";
        }

        public string GetRepositoryMethodEnumerableDtoReturnTypeName(ModelEntity modelEntity)
        {
            return $"{typeof (IEnumerable<object>).Namespace}.IEnumerable<{modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoQualifiedTypeName]}>";
        }

        public string GetRepositoryMethodScalarDtoReturnTypeName(ModelEntity modelEntity)
        {
            return $"{modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoQualifiedTypeName]}";
        }
    }

    internal class MethodsBuilderHelper
    {
        public static string GetGetItemsMethodCodeSnippet()
        {
            return @"
                return RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );";
        }

        public static string GetListMethodCodeSnippet()
        {
            return @"
                return RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );";
        }

        public static string GetWithMethodCodeSnippet()
        {
            return @"
                return RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );";
        }

        public static string GetContainerAddMethodCodeSnippet()
        {
            return @"
                RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );
                return false;";
        }
        public static string GetDirectChainGetMethodCodeSnippet()
        {
            return @"
                return RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );";
        }

        public static string GetContainerDeleteMethodCodeSnippet()
        {
            return @"
                RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );
                return false;";
        }

        public static string GetContainerGetChildrenMethodCodeSnippet()
        {
            return @"
                return RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );";
        }

        public static string GetGetItemMethodCodeSnippet()
        {
            return @"
                return RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );";
        }

        public static string GetCreateItemMethodCodeSnippet()
        {
            return @"
                return RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );";
        }

        public static string GetDeleteItemMethodCodeSnippet()
        {
            return @"
                RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );";
        }

        public static string GetUpdateItemMethodCodeSnippet()
        {
            return @"
                RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );";
        }

        public static string GetCreateItemWithIdentityMethodCodeSnippet()
        {
            return @"
                return RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );";
        }

        internal static string GetRunWithConnectionMethodCodeSnippet()
        {
            return @"
                bool disposeRequired = false;
                LinqToDB.Data.DataConnection dataConnection = null;
                try
                {
                    if (outerDataConnection != null) {
                        dataConnection = outerDataConnection;
                        disposeRequired = false;
                    }
                    else
                    {
                        LinqToDB.Data.DataConnection tlsDataConnection = Thread.GetData(Thread.GetNamedDataSlot(""#eps#dataConnection"")) as LinqToDB.Data.DataConnection;
                        if (tlsDataConnection != null) {
                            dataConnection = tlsDataConnection;
                            disposeRequired = false;
                        }
                        else
                        {
                            dataConnection = GetDataConnection();
                            disposeRequired = true;
                        }
                    }
                    $METHOD_LOGIC$
                }
                finally
                {
                    if (disposeRequired && dataConnection != null)
                        dataConnection.Dispose();
                }";
        }

        internal static string GetRunAnyWithConnectionMethodCodeSnippet()
        {
            return @"
                bool disposeRequired = false;
                LinqToDB.Data.DataConnection dataConnection = null;
                try
                {
                    if (outerDataConnection != null) {
                        dataConnection = outerDataConnection;
                        disposeRequired = false;
                    }
                    else
                    {
                        LinqToDB.Data.DataConnection tlsDataConnection = Thread.GetData(Thread.GetNamedDataSlot(""#eps#dataConnection"")) as LinqToDB.Data.DataConnection;
                        if (tlsDataConnection != null) {
                            dataConnection = tlsDataConnection;
                            disposeRequired = false;
                        }
                        else
                        {
                            dataConnection = GetDataConnection();
                            disposeRequired = true;
                        }
                    }
                    $METHOD_LOGIC$
                }
                finally
                {
                    if (disposeRequired && dataConnection != null)
                        dataConnection.Dispose();
                }";
        }

        internal static string MakeReturnStatement(RepositoryMethod repositoryMethod, NamespaceTracker namespaceTracker)
        {
            if (!repositoryMethod.ReturnValueTypeQualifiedName.Contains("void"))
            {
                return $"return default({namespaceTracker.SimplifyName(repositoryMethod.ReturnValueTypeQualifiedName, true)});";
            }
            else
            {
                return "return;";
            }
        }
    }
}
