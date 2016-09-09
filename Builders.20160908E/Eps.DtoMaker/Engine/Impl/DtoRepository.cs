using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Helpers;

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
            _repositoryMethodsBuilder.AddDeleteItemMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddUpdateItemMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddCreateItemWithIdentityMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddCreateItemMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            //
            _repositoryMethodsBuilder.AddGetItemsMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            if (_modelEntity.IdentityProperties.Count == 1)
                _repositoryMethodsBuilder.AddGetItemMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            //
            _repositoryMethodsBuilder.AddRunAnyWithConnectionMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            _repositoryMethodsBuilder.AddRunWithConnectionMethod(SignaturesNamespaceTracker, BodyNamespaceTracker);
            //
            foreach (var entityAConnectionLink in _modelEntity.IncomingLinks)
            {
                EntityLink entityBConnectionLink;
                ModelEntity connectedModelEntity;
                if (entityAConnectionLink.ReferencingModelEntity.IsItConnectingEntity(_modelEntity, out entityBConnectionLink, out connectedModelEntity))
                {
                    _repositoryMethodsBuilder.AddContainerAddMethod(SignaturesNamespaceTracker, BodyNamespaceTracker, entityAConnectionLink.ReferencingModelEntity, entityAConnectionLink, entityBConnectionLink, connectedModelEntity);
                    _repositoryMethodsBuilder.AddContainerDeleteMethod(SignaturesNamespaceTracker, BodyNamespaceTracker, entityBConnectionLink, connectedModelEntity);
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
            repositoryRunWithConnectionMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodEnumerableReturnTypeName(_modelEntity);
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
            repositoryRunWithConnectionMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetRunAnyWithConnectionMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", runWithConnectionMethodLogic.ToString()));

            repositoryRunWithConnectionMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryRunWithConnectionMethod, bodyNamespaceTracker));
        }

        internal void AddGetItemsMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositorygetItemsMethod = _modelEntity.DtoRepository.AppendMethod($"Get{_context.LinguisticManager.ToPlural(_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName])}");
            repositorygetItemsMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodEnumerableReturnTypeName(_modelEntity);
            //
            MethodParameter outerDataConnectionParameter = new MethodParameter();
            outerDataConnectionParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            outerDataConnectionParameter.Name = "outerDataConnection";
            outerDataConnectionParameter.QualifiedTypeName = "LinqToDB.Data.DataConnection";
            outerDataConnectionParameter.DefaultValueAsString = "null";
            repositorygetItemsMethod.Parameters.Add(outerDataConnectionParameter);
            //
            StringBuilder getItemsMethodLogic = new StringBuilder();
            getItemsMethodLogic.Append($@"dataConnection.GetTable<{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}>()");
            //
            repositorygetItemsMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetGetItemsMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", getItemsMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddGetItemMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Get{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodScalarDtoReturnTypeName(_modelEntity);
            //
            string idParameterName =$"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].LowercaseFirstLetter()}Id";
            string primaryKeyPropertyName = $"{_modelEntity.IdentityProperties[0]}";
            string lambdaParameterName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].MakeAbbreviationByCapitals().ToLower().UpdateNameIfItIsReservedWord()}";
            //
            MethodParameter idParameter = new MethodParameter();
            idParameter.Direction = MethodParameter.ParameterDirection.PdIn;
            idParameter.Name = idParameterName;
            idParameter.QualifiedTypeName = "int";
            repositorygetItemMethod.Parameters.Add(idParameter);
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
            listMethodLogic.Append($@"dataConnection.GetTable<{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}>().FirstOrDefault({lambdaParameterName}=>{lambdaParameterName}.{primaryKeyPropertyName}=={idParameterName})");
            //
            repositorygetItemMethod.MethodBody.AppendLine(MethodsBuilderHelper.GetGetItemMethodCodeSnippet().MakeIndentationByFirstNotEmptyLine().Replace("$METHOD_LOGIC$", listMethodLogic.ToString()));
            // repositoryListMethod.MethodBody.AppendLine(MethodsBuilderHelper.MakeReturnStatement(repositoryListMethod, bodyNamespaceTracker));
        }

        internal void AddCreateItemMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Create{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodScalarIdentityReturnTypeName(_modelEntity);
            //
            string parameterName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].LowercaseFirstLetter()}";
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
            string parameterName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].LowercaseFirstLetter()}";
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

        internal void AddDeleteItemMethod(NamespaceTracker signatureNamespaceTracker, NamespaceTracker bodyNamespaceTracker)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Delete{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodVoidReturnTypeName(_modelEntity);
            //
            string idParameterName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].LowercaseFirstLetter()}Id";
            string primaryKeyPropertyName = $"{_modelEntity.IdentityProperties[0]}";
            string lambdaParameterName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].MakeAbbreviationByCapitals().ToLower().UpdateNameIfItIsReservedWord()}";
            string parameterName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].LowercaseFirstLetter()}";
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
            string idParameterName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].LowercaseFirstLetter()}Id";
            string primaryKeyPropertyName = $"{_modelEntity.IdentityProperties[0]}";
            string lambdaParameterName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].MakeAbbreviationByCapitals().ToLower().UpdateNameIfItIsReservedWord()}";
            string parameterName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].LowercaseFirstLetter()}";
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

        public void AddContainerAddMethod(NamespaceTracker signaturesNamespaceTracker, NamespaceTracker bodyNamespaceTracker, ModelEntity connectingModelEntity, EntityLink entityAConnectionLink, EntityLink entityBConnectionLink, ModelEntity connectedModelEntity)
        {
            RepositoryMethod repositorygetItemMethod = _modelEntity.DtoRepository.AppendMethod($"Add{connectedModelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}");
            repositorygetItemMethod.ReturnValueTypeQualifiedName = GetRepositoryMethodScalarBoolReturnTypeName(_modelEntity);
            //
            string entityAIdentityPropertyName = $"{_modelEntity.OriginalPropertiesNames.FindResultPropertyName(_modelEntity.IdentityProperties[0])}";
            string entityBIdentityPropertyName = $"{connectedModelEntity.OriginalPropertiesNames.FindResultPropertyName(connectedModelEntity.IdentityProperties[0])}";
            string entityAParameterName = $"{_modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].LowercaseFirstLetter()}";
            string entityBParameterName = $"{connectedModelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].LowercaseFirstLetter()}";
            string lambdaParameterName = $"{connectingModelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName].MakeAbbreviationByCapitals().ToLower().UpdateNameIfItIsReservedWord()}";
            string connectingModelEntityClassName = connectingModelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
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
            dtoEntityBParameter.QualifiedTypeName = $"{connectedModelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName]}";
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

        public string GetRepositoryMethodEnumerableReturnTypeName(ModelEntity modelEntity)
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

        public static string GetContainerAddMethodCodeSnippet()
        {
            return @"
                RunAnyWithConnection(
                        (dataConnection) => $METHOD_LOGIC$
                        , outerDataConnection
                );
                return false;";
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
                        dataConnection = GetDataConnection();
                        disposeRequired = true;
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
                        dataConnection = GetDataConnection();
                        disposeRequired = true;
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
