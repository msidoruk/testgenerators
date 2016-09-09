using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using EPS.DtoMaker.Engine.Actions;
using EPS.DtoMaker.Engine.Actions.BuildDtos;
using EPS.DtoMaker.Engine.Actions.InjectEnums;
using EPS.DtoMaker.Engine.Base;
using EPS.DtoMaker.Engine.Helpers;
using Microsoft.CSharp;

namespace EPS.DtoMaker.Engine.Impl
{
    public class DtoProjectComposer : IDtoProjectComposer
    {
        public class Markers
        {
            public const string CsDtoProjectFile = "dtoProjectComposer:cs-project-file";
            public const string CsDalProjectFile = "dalProjectComposer:cs-project-file";
            public const string CsDtoRootNamespace = "dtoProjectComposer:cs-root-namespace";
            public const string CsRepoImplsRootNamespace = "repoImplsProjectComposer:cs-root-namespace";
            public const string CsRepoInterfacesRootNamespace = "repoInterfacesProjectComposer:cs-root-namespace";
            public const string CsDtoDestanationFolder = "dtoProjectComposer:cs-dto-destanation-folder";
            public const string CsRepositoriesInterfacesDestanationFolder = "dtoProjectComposer:cs-repositories-interfaces-destanation-folder";
            public const string CsRepositoriesImplDestanationFolder = "dtoProjectComposer:cs-repositories-impl-destanation-folder";
            public const string CsRepositoriesBaseClassName = "dtoProjectComposer:cs-repositories-base-class";
            public const string CsRepositoryStaticImplementation = "dtoProjectComposer:cs-repositories-static-implementation";
            public const string CsRepositoryStaticImplementationUsings = "dtoProjectComposer:cs-repositories-static-implementation-usings";
        }

        public class WrittenFileCategory
        {
            public const string CsDto = "cs-file-dto";
            public const string CsRepo = "cs-file-repo";
        }

        public class ModelNames
        {
            public const string CsDtoNamespace = "cs.namespace";
            public const string CsDtoOwnNamespace = "cs.dto.own.namespace";
            public const string CsDtoShortTypeName = "cs.dto.short.typename";
            public const string CsDtoQualifiedTypeName = "cs.dto.qualified.typename";
            public const string CsDtoFileName = "dto.cs.file";
            public const string CsDtoFullFilePath = "dto.cs.file.full.path";
            public const string CsRepositoryInterfaceFileName = "repo.itf.cs.file";
            public const string CsRepositoryInterfaceFullFilePath = "repo.itf.cs.file.full.path";
            public const string CsRepositoryInterfaceNamespace = "cs.repo.itf.namespace";
            public const string CsRepositoryInterfaceOwnNamespace = "cs.repo.itf.own.namespace";
            public const string CsRepositoryInterfaceShortTypeName = "cs.repo.itf.short.typename";
            public const string CsRepositoryInterfaceQualifiedTypeName = "cs.repo.itf.qualified.typename";
            public const string CsRepositoryImplementationFileName = "repo.impl.cs.file";
            public const string CsRepositoryImplementationFullFilePath = "repo.impl.cs.file.full.path";
            public const string CsRepositoryImplementationNamespace = "cs.repo.impl.namespace";
            public const string CsRepositoryImplementationOwnNamespace = "cs.repo.impl.own.namespace";
            public const string CsRepositoryImplementationShortTypeName = "cs.repo.impl.short.typename";
            public const string CsRepositoryImplementationQualifiedTypeName = "cs.repo.impl.qualified.typename";
            public const string CsProperyName = "cs.prop";
            public const string CsPropertyTypeName = "cs.prop.typename";
            public const string CsLinkPropertyName = "cs.link.prop";
        }

        private ICsRools _csRools;
        private DtoMakerContext _context;
        private readonly List<ModelEntity> _modelEntities = new List<ModelEntity>();
        //
        public DtoProjectComposer(ICsRools csRools)
        {
            _csRools = csRools;
        }

        //
        public void BindToContext(DtoMakerContext context)
        {
            _context = context;
        }

        public void RegisterEntity(ModelEntity modelEntity)
        {
            _modelEntities.Add(modelEntity);
        }

        public void Done()
        {
            _csRools.BindToContext(_context);
            ModelCsDetails modelCsDetails = new ModelCsDetails(_context);
            //
            if (modelCsDetails.IsValid)
            {
                EnhanceModelWithCsRequirements(modelCsDetails);

                Dictionary<string, CsComposer> resultDtosCsFiles = new Dictionary<string, CsComposer>();
                Dictionary<string, CsComposer> resultRepositoriesCsFiles = new Dictionary<string, CsComposer>();

                _context.WrittenFilesRegistry.RegisterWrittenFilesCategory(WrittenFileCategory.CsDto);
                _context.WrittenFilesRegistry.RegisterWrittenFilesCategory(WrittenFileCategory.CsRepo);

                BuildModelCsDtos(resultDtosCsFiles);

                // Этот метод создает файлы для двух проектов: определения интерфейсов репозиториев помещаются в Dto-проект, реализации репозиториев помещаются в Dal-проект.
                BuildModelCsRepositories(modelCsDetails, resultDtosCsFiles, resultRepositoriesCsFiles);

                WriteCsFiles(WrittenFileCategory.CsDto, resultDtosCsFiles);
                UpdateVisualStudioProjectFile(modelCsDetails.CsDtoProjectFilePath, _context.WrittenFilesRegistry.GetCategoryWrittenFiles(WrittenFileCategory.CsDto));

                WriteCsFiles(WrittenFileCategory.CsRepo, resultRepositoriesCsFiles);
                UpdateVisualStudioProjectFile(modelCsDetails.CsDalProjectFilePath, _context.WrittenFilesRegistry.GetCategoryWrittenFiles(WrittenFileCategory.CsRepo));
            }
            else
            {
                throw new Exception($"Cannot read model cs details: {modelCsDetails.LoadErrors}");
            }
        }

        internal class CsComposer
        {
            private readonly StringBuilder _entityBodyStringBuilder = new StringBuilder();
            private readonly StringBuilder _usingSectionStringBuilder = new StringBuilder();
            private readonly List<StringBuilder> _stringBuilders = new List<StringBuilder>();

            public CsComposer()
            {
                _stringBuilders.Add(_usingSectionStringBuilder);
                _stringBuilders.Add(_entityBodyStringBuilder);
            }

            public void AddBodyLine(string bodyLine)
            {
                _entityBodyStringBuilder.AppendLine(bodyLine);
            }

            public void AddUsingLine(string usingLine)
            {
                _usingSectionStringBuilder.AppendLine(usingLine);
            }

            public string GetCsFileContent()
            {
                StringBuilder csFileContent = new StringBuilder();
                csFileContent.Append(_usingSectionStringBuilder);
                csFileContent.AppendLine();
                csFileContent.Append(_entityBodyStringBuilder);
                return csFileContent.ToString();
            }
        }

        private void BuildModelCsDtos(Dictionary<string, CsComposer> resultCsFiles)
        {
            foreach (var modelEntity in _modelEntities)
            {
                CsComposer csComposer = new CsComposer();
                resultCsFiles.Add(modelEntity.AlternativeNames[ModelNames.CsDtoFullFilePath], csComposer);

                if (!modelEntity.Stereotypes.ContainsKey(ModelEntity.StereotypesNames.IsEnum))
                    BuildClass(_context, modelEntity, csComposer);
                else
                    BuildEnum(_context, modelEntity, csComposer);
            }
        }

        private void EnhanceModelWithCsRequirements(ModelCsDetails modelCsDetails)
        {
            foreach (var modelEntity in _modelEntities)
            {
                // Преимущественно элементы, касающиеся Dto
                string modelEntitySpace = modelEntity.EntitySpace;
                string classNamespaceTemp;
                if (modelCsDetails.EntitiesSpacesRenamingMap == null || !modelCsDetails.EntitiesSpacesRenamingMap.TryGetValue(modelEntitySpace, out classNamespaceTemp) || string.IsNullOrEmpty(classNamespaceTemp))
                    classNamespaceTemp = modelEntitySpace;
                string dtoClassAndRepositoryOwnNamespace = classNamespaceTemp.CapitalizeFirstLetter();
                //
                string modelEntityClassName = _csRools.MakeDtoClassName(modelEntity);
                //
                modelEntity.AlternativeNames[ModelNames.CsDtoNamespace] = $"{modelCsDetails.CsDtoRootNamespace}.{dtoClassAndRepositoryOwnNamespace}";
                modelEntity.AlternativeNames[ModelNames.CsDtoOwnNamespace] = $"{dtoClassAndRepositoryOwnNamespace}";
                modelEntity.AlternativeNames[ModelNames.CsDtoShortTypeName] = modelEntityClassName;
                modelEntity.AlternativeNames[ModelNames.CsDtoQualifiedTypeName] = _csRools.MakeQualifiedClassName(modelEntity.AlternativeNames[ModelNames.CsDtoNamespace], modelEntityClassName);
                modelEntity.AlternativeNames[ModelNames.CsDtoFileName] = $"{modelEntityClassName}.cs";
                modelEntity.AlternativeNames[ModelNames.CsDtoFullFilePath] = $"{modelCsDetails.CsDtoRootFolder}\\{modelEntity.AlternativeNames[ModelNames.CsDtoOwnNamespace]}\\{modelEntity.AlternativeNames[ModelNames.CsDtoFileName]}";
                //
                foreach (var property in modelEntity.Properties)
                {
                    property.AlternativeNames[ModelNames.CsProperyName] = property.Name;
                    property.AlternativeNames[ModelNames.CsPropertyTypeName] = _csRools.MakePropertyTypeName(property.Type);
                }
                //
                foreach (var entityLink in modelEntity.EntityLinks)
                {
                    entityLink.ReferencingModelEntity = modelEntity;
                    entityLink.ReferencingPropertySourceProperty = entityLink.ReferencingModelEntity.Properties.GetProperty(entityLink.ReferencingPropertySourcePropertyName);
                    if (entityLink.ReferencingPropertySourceProperty == null)
                        throw new Exception($"Property '{entityLink.ReferencingPropertySourcePropertyName}' cannot be found in entity '{entityLink.ReferencingModelEntity.EntityName}'");

                    entityLink.ReferencingPropertySourceProperty.AssociatedLink = entityLink;
                    string referencingPropertyName = _csRools.MakeReferencingPropertyName(modelEntity, entityLink.ReferencingPropertySourceProperty, entityLink.ReferencedEntityName);
                    entityLink.AlternativeNames[ModelNames.CsLinkPropertyName] = referencingPropertyName;

                    entityLink.ReferencedModelEntity = _context.Model.Entities.GetEntity(entityLink.ReferencedEntitySpace, entityLink.ReferencedEntityName);
                    if (entityLink.ReferencedModelEntity == null)
                    {
                        string referencingKeyInfo = $"{modelEntity.EntitySpace}.{modelEntity.EntityName}.{entityLink.ReferencingPropertySourcePropertyName}";
                        string referencedEntityInfo = $"{entityLink.ReferencedEntitySpace}.{entityLink.ReferencedEntityName}";
                        throw new Exception(
                            $"Foreign key of '{referencingKeyInfo}' refers to '{referencedEntityInfo}'... " +
                            $"But entity '{referencedEntityInfo}' is unavailable (was not read from Db to internal collection)." +
                            "Please check that it was mentioned in Db tables to read list.");
                    }
                    entityLink.ReferencedModelEntity.IncomingLinks.Add(entityLink);
                }

                // Элементы, касающиеся Repository
                string modelEntityRepositoryInterfaceName = _csRools.MakeRepositoryInterfaceName(modelEntity);
                string modelEntityRepositoryImplementationClassName = _csRools.MakeRepositoryImplementationClassName(modelEntity);

                modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceNamespace] = $"{modelCsDetails.CsRepoInterfacesRootNamespace}.{dtoClassAndRepositoryOwnNamespace}";
                modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceOwnNamespace] = $"{dtoClassAndRepositoryOwnNamespace}";
                modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceShortTypeName] = modelEntityRepositoryInterfaceName;
                modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceQualifiedTypeName] = _csRools.MakeQualifiedClassName(modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceNamespace], modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceShortTypeName]);
                modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceFileName] = $"{modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceShortTypeName]}.cs";
                modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceFullFilePath] = $"{modelCsDetails.CsRepositoriesInterfacesDestanationFolder}\\{modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceOwnNamespace]}\\{modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceFileName]}";

                modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationNamespace] = $"{modelCsDetails.CsRepoImplsRootNamespace}.{dtoClassAndRepositoryOwnNamespace}";
                modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationOwnNamespace] = $"{dtoClassAndRepositoryOwnNamespace}";
                modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationShortTypeName] = modelEntityRepositoryImplementationClassName;
                modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationQualifiedTypeName] = _csRools.MakeQualifiedClassName(modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationNamespace], modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationShortTypeName]);
                modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationFileName] = $"{modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationShortTypeName]}.cs";
                modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationFullFilePath] = $"{modelCsDetails.CsRepositoriesImplDestanationFolder}\\{modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationOwnNamespace]}\\{modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationFileName]}";
            }
        }

        private void BuildClass(DtoMakerContext context,
                                ModelEntity modelEntity,
                                CsComposer csComposer)
        {
            SortedList<string, string> usedTypesNamespaces = new SortedList<string, string>();
            //
            string partiality = string.Empty;
            string baseClassesSpecification;
            BuildBaseClassSpecification(modelEntity, out baseClassesSpecification, usedTypesNamespaces);
            //
            string modelEntityClassName = $"{modelEntity.AlternativeNames[ModelNames.CsDtoShortTypeName]}";
            //
            csComposer.AddBodyLine($"namespace {modelEntity.AlternativeNames[ModelNames.CsDtoNamespace]}");
            csComposer.AddBodyLine("{");
            csComposer.AddBodyLine($"\t// Automatically built for '{modelEntity.EntitySpace}.{modelEntity.EntityName}'");
            csComposer.AddBodyLine($"\tpublic{partiality} class {modelEntityClassName}{baseClassesSpecification}");
            csComposer.AddBodyLine("\t{");
            csComposer.AddBodyLine("\t\t// Methods");
            csComposer.AddBodyLine($"\t\tpublic {modelEntityClassName}()");
            csComposer.AddBodyLine("\t\t{");
            csComposer.AddBodyLine("\t\t}");
            csComposer.AddBodyLine($"\t\tpublic {modelEntityClassName}({modelEntityClassName} other)");
            csComposer.AddBodyLine("\t\t{");
            foreach (var property in modelEntity.Properties)
            {
                string csPropertyName = property.AlternativeNames[ModelNames.CsProperyName];
                csComposer.AddBodyLine($"\t\t\tthis.{csPropertyName} = other.{csPropertyName};");
            }
            csComposer.AddBodyLine("\t\t}");
            csComposer.AddBodyLine("");
            csComposer.AddBodyLine("\t\t// Properties");
            foreach (var property in modelEntity.Properties)
            {
                string csPropertyName = property.AlternativeNames[ModelNames.CsProperyName];
                usedTypesNamespaces[property.Type.Namespace] = property.Type.Namespace;
                string nullableMark = property.Type.IsValueType && property.IsNullable ? "?" : string.Empty;
                csComposer.AddBodyLine($"\t\t// '{modelEntity.EntityName}.{property.Name}'");
                csComposer.AddBodyLine($"\t\tpublic {property.AlternativeNames[ModelNames.CsPropertyTypeName]}{nullableMark} {csPropertyName} {{ get; set; }}");
            }
            if (modelEntity.EntityLinks.Any())
            {
                csComposer.AddBodyLine("");
                csComposer.AddBodyLine("\t\t// Links");
                foreach (var entityLink in modelEntity.EntityLinks)
                {
                    string referencedTypeNameNamespace = entityLink.ReferencedModelEntity.AlternativeNames[ModelNames.CsDtoNamespace];
                    string referencedTypeNameForProperty =
                        $"{entityLink.ReferencedModelEntity.AlternativeNames[ModelNames.CsDtoOwnNamespace]}.{entityLink.ReferencedModelEntity.AlternativeNames[ModelNames.CsDtoShortTypeName]}";
                    usedTypesNamespaces[referencedTypeNameNamespace] = referencedTypeNameNamespace;
                    string referencingPropertyName = entityLink.AlternativeNames[ModelNames.CsLinkPropertyName];
                    csComposer.AddBodyLine($"\t\t// '{modelEntity.EntityName}.{entityLink.ReferencingPropertySourcePropertyName}' --> '{entityLink.ReferencedEntitySpace}.{entityLink.ReferencedEntityName}'");
                    csComposer.AddBodyLine($"\t\tpublic {referencedTypeNameForProperty} {referencingPropertyName} {{ get; set; }}");
                }
            }
            if (modelEntity.RawMembers.Any())
            {
                csComposer.AddBodyLine("");
                csComposer.AddBodyLine("\t\t// Raw members specified manually");
                foreach (var rawMember in modelEntity.RawMembers)
                {
                    csComposer.AddBodyLine($"\t\t{rawMember}");
                }
            }
            csComposer.AddBodyLine("\t}");
            csComposer.AddBodyLine("}");
            //
            foreach (var usedTypesNamespace in usedTypesNamespaces)
            {
                csComposer.AddUsingLine($"using {usedTypesNamespace.Value};");
            }
        }

        private void BuildBaseClassSpecification(ModelEntity modelEntity, out string baseClassSpecification, SortedList<string, string> usedNamespaces)
        {
            baseClassSpecification = string.Empty;
            if (modelEntity.RawBaseClasses.Any())
            {
                baseClassSpecification = " : ";
                int baseClassIndex = 0;
                foreach (var rawBaseClassSpecification in modelEntity.RawBaseClasses)
                {
                    if (!string.IsNullOrEmpty(rawBaseClassSpecification.Name))
                        usedNamespaces[rawBaseClassSpecification.SourceNamespace] = rawBaseClassSpecification.SourceNamespace;
                    baseClassSpecification = baseClassSpecification + (baseClassIndex++ > 0  ? ", " : string.Empty) + rawBaseClassSpecification.Name;
                }
            }
        }

        private void BuildEnum(DtoMakerContext context, ModelEntity modelEntity, CsComposer csComposer)
        {
            //
            csComposer.AddBodyLine($"namespace {modelEntity.AlternativeNames[ModelNames.CsDtoNamespace]}");
            csComposer.AddBodyLine("{");
            csComposer.AddBodyLine($"\tpublic enum {modelEntity.AlternativeNames[ModelNames.CsDtoShortTypeName]}");
            csComposer.AddBodyLine("\t{");
            foreach (Dictionary<string, object> instanceProperties in modelEntity.Instances)
            {
                object memberName;
                instanceProperties.TryGetValue("MemberName", out memberName);
                object memberValue;
                instanceProperties.TryGetValue("MemberValue", out memberValue);
                string memberValueAsString = (memberValue != null) ? $" = {memberValue}," : String.Empty;
                csComposer.AddBodyLine($"\t\t{memberName}{memberValueAsString}");
            }
            csComposer.AddBodyLine("\t}");
            csComposer.AddBodyLine("}");
        }

        // $r
        private void BuildModelCsRepositories(ModelCsDetails modelCsDetails, Dictionary<string, CsComposer> resultDtosCsFiles, Dictionary<string, CsComposer> resultRepositoriesCsFiles)
        {
            foreach (var modelEntity in _modelEntities)
            {
                if (!modelEntity.IsRepositoryRequired)
                    continue;
                if (!IsRepositoryPossible(modelEntity))
                    continue;

                CsComposer csRepositoryInterfaceComposer = new CsComposer();
                resultDtosCsFiles.Add(modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceFullFilePath], csRepositoryInterfaceComposer);
                CsComposer csRepositoryImplComposer = new CsComposer();
                resultRepositoriesCsFiles.Add(modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationFullFilePath], csRepositoryImplComposer);

                DtoRepository dtoRepository = new DtoRepository(_context, modelEntity);
                dtoRepository.BuildRepository();

                BuildRepositoryInterface(modelCsDetails, modelEntity, csRepositoryInterfaceComposer, new NamespaceTracker(dtoRepository.SignaturesNamespaceTracker));
                BuildRepositoryImplementation(modelCsDetails, modelEntity, csRepositoryImplComposer, new NamespaceTracker(new List<NamespaceTracker>() { dtoRepository.SignaturesNamespaceTracker, dtoRepository.BodyNamespaceTracker}));
            }
        }

        private void BuildRepositoryInterface(ModelCsDetails modelCsDetails, ModelEntity modelEntity, CsComposer csRepositoryInterfaceComposer, NamespaceTracker namespaceTracker)
        {
            //
            string baseClassesSpecification = string.Empty;
            //
            csRepositoryInterfaceComposer.AddBodyLine($"namespace {modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceNamespace]}");
            csRepositoryInterfaceComposer.AddBodyLine("{");
            csRepositoryInterfaceComposer.AddBodyLine($"\t// Automatically built for '{modelEntity.EntitySpace}.{modelEntity.EntityName}'");
            csRepositoryInterfaceComposer.AddBodyLine($"\tpublic partial interface {modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceShortTypeName]}{baseClassesSpecification}");
            csRepositoryInterfaceComposer.AddBodyLine("\t{");
            foreach (var repositoryMethod in modelEntity.DtoRepository.RepositoryMethods)
            {
                csRepositoryInterfaceComposer.AddBodyLine($"\t\t{MakeSignature(modelEntity, repositoryMethod, namespaceTracker, "\t\t")};");
            }
            csRepositoryInterfaceComposer.AddBodyLine("\t}");
            csRepositoryInterfaceComposer.AddBodyLine("}");
            //
            foreach (var usedTypesNamespace in namespaceTracker.FacedNamespaces)
            {
                csRepositoryInterfaceComposer.AddUsingLine($"using {usedTypesNamespace};");
            }
        }

        private string MakeSignature(ModelEntity modelEntity, RepositoryMethod repositoryMethod, NamespaceTracker namespaceTracker, string basicMethodSignatureIndentation)
        {
            string methodReturnValueTypeName = namespaceTracker.SimplifyName(repositoryMethod.ReturnValueTypeQualifiedName, true);
            string parameterIndentation = 
                basicMethodSignatureIndentation + new string(' ', "public".Length + methodReturnValueTypeName.Length + repositoryMethod.Name.Length + repositoryMethod.GenericSpecification.Length + "(".Length + ")".Length);
            StringBuilder methodParametersListStringBuilder = new StringBuilder();
            int parameterIndex = 0;
            foreach (var parameter in repositoryMethod.Parameters)
            {
                if (parameterIndex > 0)
                {
                    methodParametersListStringBuilder.AppendLine(",");
                    methodParametersListStringBuilder.Append(parameterIndentation);
                }
                string direction = string.Empty;
                switch (parameter.Direction)
                {
                    case MethodParameter.ParameterDirection.PdIn:
                        break;
                    case MethodParameter.ParameterDirection.PdOut:
                        methodParametersListStringBuilder.AppendLine("[out] ");
                        break;
                    case MethodParameter.ParameterDirection.PdRef:
                        methodParametersListStringBuilder.AppendLine("[ref] ");
                        break;
                    default:
                        throw new Exception($"Entity '{modelEntity.EntityName}' has wrong parameter '{parameter.Name}' direction.");
                }
                string parameterType = parameter.DoSimplifyTypeName ? namespaceTracker.SimplifyName(parameter.QualifiedTypeName, true) : parameter.QualifiedTypeName;
                string parameterDefaultValueSpecification = parameter.DefaultValueAsString;
                if (!string.IsNullOrEmpty(parameterDefaultValueSpecification))
                    parameterDefaultValueSpecification = " = " + parameterDefaultValueSpecification;
                methodParametersListStringBuilder.Append($"{direction}{parameterType} {parameter.Name}{parameterDefaultValueSpecification}");
                ++parameterIndex;
            }

            string methodSignature = $"{methodReturnValueTypeName} {repositoryMethod.Name}{repositoryMethod.GenericSpecification}({methodParametersListStringBuilder})";
            return methodSignature;
        }

        private void BuildRepositoryImplementation(ModelCsDetails modelCsDetails, ModelEntity modelEntity, CsComposer csRepositoryImplComposer, NamespaceTracker namespaceTracker)
        {
            //
            if (!string.IsNullOrEmpty(modelCsDetails.CsRepositoryStaticImplementationUsings))
                namespaceTracker.AddNamespaces(modelCsDetails.CsRepositoryStaticImplementationUsings.Split(';'));
            //
            string partiality = "partial";
            string baseClassesSpecification = BuildRepositoryImplementationBaseClassSpecification(modelCsDetails, modelEntity, namespaceTracker);
            //
            csRepositoryImplComposer.AddBodyLine($"namespace {modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationNamespace]}");
            csRepositoryImplComposer.AddBodyLine("{");
            csRepositoryImplComposer.AddBodyLine($"\t// Automatically built for '{modelEntity.EntitySpace}.{modelEntity.EntityName}'");
            csRepositoryImplComposer.AddBodyLine($"\tpublic partial class {modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationShortTypeName]}{baseClassesSpecification}");
            csRepositoryImplComposer.AddBodyLine("\t{");
            csRepositoryImplComposer.AddBodyLine($"\t\t{modelCsDetails.CsRepositoryStaticImplementation.Replace("$REPOSITORY_CLASS$", modelEntity.AlternativeNames[ModelNames.CsRepositoryImplementationShortTypeName])}");
            foreach (var repositoryMethod in modelEntity.DtoRepository.RepositoryMethods)
            {
                csRepositoryImplComposer.AddBodyLine($"\t\tpublic {MakeSignature(modelEntity, repositoryMethod, namespaceTracker, "\t\t")}");
                csRepositoryImplComposer.AddBodyLine($"\t\t{{");
                foreach (var methodBodyLine in repositoryMethod.MethodBody.ToString().Split('\n'))
                {
                    csRepositoryImplComposer.AddBodyLine($"\t\t\t{methodBodyLine.Replace("\n", string.Empty).Replace("\r", string.Empty)}");
                }
                csRepositoryImplComposer.AddBodyLine($"\t\t}}");
            }
            csRepositoryImplComposer.AddBodyLine("\t}");
            csRepositoryImplComposer.AddBodyLine("}");
            //
            foreach (var usedTypesNamespace in namespaceTracker.FacedNamespaces)
            {
                csRepositoryImplComposer.AddUsingLine($"using {usedTypesNamespace};");
            }
        }

        private string BuildRepositoryImplementationBaseClassSpecification(ModelCsDetails modelCsDetails, ModelEntity modelEntity, NamespaceTracker namespaceTracker)
        {
            StringBuilder stringBuilder = new StringBuilder();
            if (!string.IsNullOrEmpty(modelCsDetails.CsRepositoriesBaseClassName))
            {
                stringBuilder.Append(namespaceTracker.SimplifyName(modelCsDetails.CsRepositoriesBaseClassName, true));
            }
            if (stringBuilder.Length > 0 )
                stringBuilder.Append(", ");
            string implementedInterface = modelEntity.AlternativeNames[ModelNames.CsRepositoryInterfaceQualifiedTypeName];
            implementedInterface = namespaceTracker.SimplifyName(implementedInterface, true);
            stringBuilder.Append($"{implementedInterface}");
            if (stringBuilder.Length > 0)
                stringBuilder.Insert(0, " : ");
            return stringBuilder.ToString();
        }

        private bool IsRepositoryPossible(ModelEntity modelEntity)
        {
            return true;
        }


        private void WriteCsFiles(string csFileCategory, Dictionary<string, CsComposer> resultCsFiles)
        {
            foreach (var resultCsFile in resultCsFiles)
            {
                bool overriden;
                WriteTextFileIfChanged(resultCsFile.Key, resultCsFile.Value.GetCsFileContent(), out overriden);

                _context.WrittenFilesRegistry.RegisterWrittenFile(csFileCategory, resultCsFile.Key);
            }
        }

        public static void WriteTextFile(string path, string content)
        {
            string directoryPath = Path.GetDirectoryName(path);
            DirectoryInfo directoryInfo = new DirectoryInfo(directoryPath);
            if (!directoryInfo.Exists)
                directoryInfo.Create();
            //
            using (var resultCsFileWriter = new StreamWriter(path))
            {
                resultCsFileWriter.WriteLine(content);
            }
        }

        public static void WriteTextFileIfChanged(string path, string content, out bool overriden)
        {
            overriden = false;
            string currentContent = string.Empty;
            if (File.Exists(path))
            {
                using (var fileReader = new StreamReader(path))
                {
                    currentContent = fileReader.ReadToEnd();
                }
            }
            if (string.IsNullOrEmpty(currentContent) || !currentContent.Trim().Equals(content.Trim()) /*We need trim cause WriteTextFile() or ReadToEnd() appends \r\n to end of line*/)
            {
                WriteTextFile(path, content);
                overriden = true;
            }
        }
        private void UpdateVisualStudioProjectFile(string csProjectFile, IEnumerable<WrittenFileInfo> writtenFileInfos)
        {
            if (!File.Exists(csProjectFile))
                return;

            XmlDocument csProjectFileXD = new XmlDocument();
            csProjectFileXD.Load(csProjectFile);
            File.Copy(csProjectFile, csProjectFile + "~", true);
            XmlElement csProjectRootXE = csProjectFileXD.DocumentElement;
            XmlNamespaceManager ns = new XmlNamespaceManager(csProjectFileXD.NameTable);
            ns.AddNamespace("msbld", "http://schemas.microsoft.com/developer/msbuild/2003");
            XmlNodeList projectFilesIncludesXL = csProjectRootXE.SelectNodes("//Project/ItemGroup[Compile[@Include]]");
            foreach (XmlElement projectFileIncludeXE in projectFilesIncludesXL)
            {
            }
            string csProjectFolder = Path.GetDirectoryName(csProjectFile);
            XmlElement projectItemGroupXE = csProjectFileXD.CreateElement("ItemGroup", csProjectRootXE.NamespaceURI);
            XmlElement appendedProjectItemGroupXE = (XmlElement)csProjectRootXE.AppendChild(projectItemGroupXE);
            foreach (WrittenFileInfo writtenFileInfo in writtenFileInfos)
            {
                string csFilePathForProject = writtenFileInfo.FilePath;
                if (csFilePathForProject.StartsWith(csProjectFolder))
                {
                    // Remove path to DTO's root folder for concrete file.
                    csFilePathForProject = csFilePathForProject.Substring(csProjectFolder.Length + 1);
                }
                // Search with $"/msbld:Project/msbld:ItemGroup/msbld:Compile[@msbld:Include='{csFilePathForProject}']" doesn't work - we will use loop - $2do: try to find solution later.
                XmlElement existingFileXE = null;
                XmlNodeList compileItemsXL = csProjectRootXE.SelectNodes("/msbld:Project/msbld:ItemGroup/msbld:Compile[@Include]", ns);
                foreach (XmlElement compileItemXE in compileItemsXL)
                {
                    if (compileItemXE.GetAttribute("Include").Equals(csFilePathForProject, StringComparison.InvariantCultureIgnoreCase))
                    {
                        existingFileXE = compileItemXE;
                        break;
                    }
                }
                if (existingFileXE == null)
                {
                    XmlElement projectItemGroupItemXE = csProjectFileXD.CreateElement("Compile", csProjectFileXD.DocumentElement.NamespaceURI);
                    XmlElement appendedProjectItemGroupItemXE = (XmlElement)appendedProjectItemGroupXE.AppendChild(projectItemGroupItemXE);
                    appendedProjectItemGroupItemXE.SetAttribute("Include", csFilePathForProject);
                }
            }
            csProjectFileXD.Save(csProjectFile);
        }

        public void Init()
        {
        }
    }

    public static class Extentions
    {
        public static DtoMakerContext CsDtoProjectFilePath(this DtoMakerContext dtoMakerContext, string csDtoProjectFilePath)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsDtoProjectFile, csDtoProjectFilePath);
            return dtoMakerContext;
        }
        public static DtoMakerContext CsDalProjectFilePath(this DtoMakerContext dtoMakerContext, string csDalProjectFilePath)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsDalProjectFile, csDalProjectFilePath);
            return dtoMakerContext;
        }
        public static DtoMakerContext CsDtoRootNamespace(this DtoMakerContext dtoMakerContext, string rootNamespace)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsDtoRootNamespace, rootNamespace);
            return dtoMakerContext;
        }
        public static DtoMakerContext CsRepoInterfacesRootNamespace(this DtoMakerContext dtoMakerContext, string rootNamespace)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsRepoInterfacesRootNamespace, rootNamespace);
            return dtoMakerContext;
        }
        public static DtoMakerContext CsRepoImplsRootNamespace(this DtoMakerContext dtoMakerContext, string rootNamespace)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsRepoImplsRootNamespace, rootNamespace);
            return dtoMakerContext;
        }
        public static DtoMakerContext CsDtoDestanationFolder(this DtoMakerContext dtoMakerContext, string dtoDestanationFolder)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsDtoDestanationFolder, dtoDestanationFolder);
            return dtoMakerContext;
        }

        public static DtoMakerContext CsRepositoriesInterfacesDestanationFolder(this DtoMakerContext dtoMakerContext, string dtoDestanationFolder)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsRepositoriesInterfacesDestanationFolder, dtoDestanationFolder);
            return dtoMakerContext;
        }

        public static DtoMakerContext CsRepositoriesImplDestanationFolder(this DtoMakerContext dtoMakerContext, string dtoDestanationFolder)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsRepositoriesImplDestanationFolder, dtoDestanationFolder);
            return dtoMakerContext;
        }

        public static DtoMakerContext CsRepositoriesBaseClassName(this DtoMakerContext dtoMakerContext, string dtoDestanationFolder)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsRepositoriesBaseClassName, dtoDestanationFolder);
            return dtoMakerContext;
        }

        public static DtoMakerContext CsRepositoryStaticImplementation(this DtoMakerContext dtoMakerContext, string dtoDestanationFolder)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsRepositoryStaticImplementation, dtoDestanationFolder);
            return dtoMakerContext;
        }
        public static DtoMakerContext CsRepositoryStaticImplementationUsings(this DtoMakerContext dtoMakerContext, string dtoDestanationFolder)
        {
            dtoMakerContext.AppendMark(DtoProjectComposer.Markers.CsRepositoryStaticImplementationUsings, dtoDestanationFolder);
            return dtoMakerContext;
        }
    }
}
