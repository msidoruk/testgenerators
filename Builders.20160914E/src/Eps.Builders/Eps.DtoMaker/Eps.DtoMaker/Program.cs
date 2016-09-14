using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Autofac;
using Eps.DbFeatures.Attributes;
using EPS.DtoMaker.Engine;
using EPS.DtoMaker.Engine.Actions;
using EPS.DtoMaker.Engine.Actions.Base;
using EPS.DtoMaker.Engine.Actions.BuildDtos;
using EPS.DtoMaker.Engine.Actions.BuildLinqToDbFluentMapping;
using EPS.DtoMaker.Engine.Actions.ImportEntityFromDb;
using EPS.DtoMaker.Engine.Actions.InjectEnums;
using EPS.DtoMaker.Engine.Base;
using EPS.DtoMaker.Engine.Helpers;
using EPS.DtoMaker.Engine.Impl;
using EPS.DtoMaker.Engine.Impl.DbTableFeaturesImplementors;

namespace EPS.DtoMaker
{
    class Program
    {
        internal class CommandLineParser
        {
            private string[] _args;
            public CommandLineParser(string[] args)
            {
                _args = args;
            }
            public bool IsValid { get; private set; }
            public CommandLineParser Parse()
            {
                IsValid = true;
                return this;
            }
        }

        static void Main(string[] args)
        {
            NamespaceTracker namespaceTracker2 = new NamespaceTracker();
            string simplifiedName = namespaceTracker2.SimplifyName("A1.A2.A3.at<B1.B2.bt<C1.C2.C3.ct<D1.D2.dt <  E1.E2.e3 >>>,BA1.BA2.ba<K1<T1>>>", true);


            CommandLineParser commandLineParser = new CommandLineParser(args).Parse();
            if (!commandLineParser.IsValid)
            {
                Console.WriteLine("Invalid command line arguments");
                return;
            }
            //
            string connectionString = "Server=.;Database=Eps-dev-a1;Trusted_Connection=True;"; //"Server=.;Database=Eps-dev2;Trusted_Connection=True;";
            DtoMakerBase dtoMaker = new DtoMakerImpl(connectionString);
            dtoMaker.Make();
            //
            Console.ReadLine();
        }
    }

    public abstract class DtoMakerBase
    {
        private readonly string _connectionString;

        public DtoMakerBase(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Make()
        {
            ContainerBuilder containerBuilder = new ContainerBuilder();
            //
            containerBuilder.RegisterType(typeof(SqlConnection)).WithProperty("ConnectionString", _connectionString).As<System.Data.IDbConnection>();
            containerBuilder.RegisterType(typeof(DbDictionaryAccessor)).As<IDbDictionaryAccessor>();
            containerBuilder.RegisterType(typeof(SqlServerDbPropertyTypeMapper)).As<IDbPropertyTypeMapper>();
            containerBuilder.RegisterType(typeof(DtoProjectComposer)).As<IDtoProjectComposer>();
            containerBuilder.RegisterType(typeof(CsRools)).As<ICsRools>();
            //
            IContainer container = containerBuilder.Build();
            //
            DtoMakerContext context = new DtoMakerContext(container);
            //
            ImportEntityFromDb importEntityFromDb = new ImportEntityFromDb(context);
            InjectEnum injectEnum = new InjectEnum(context);
            TransformModelToCSharpFiles transformModelToCSharpFiles = new TransformModelToCSharpFiles(context);
            BuildDtos buildDtos = new BuildDtos(context);
            BuildRepositories buildRepositories = new BuildRepositories(context);
            BuildLinqToDbFluentMapping buildLinqToDbFluentMapping = new BuildLinqToDbFluentMapping(context);
            //
            SetupDbFeaturesImplementors(context);
            //
            SetupModel(context, importEntityFromDb, injectEnum);
            context.Model.SealModel();
            //
            buildDtos.Do(context);
            buildRepositories.Do(context);
            //
            buildLinqToDbFluentMapping.Do(context);
            //
            DumpEntities(context);
            //
            transformModelToCSharpFiles.Do(context);
            //
            Console.WriteLine();
            for (int i = context.Model.EntitiesMinId; i < context.Model.EntitiesMaxId; ++i)
            {
                for (int j = context.Model.EntitiesMinId; j < context.Model.EntitiesMaxId; ++j)
                {
                    Console.Write(context.Model.EntitiesConnectionsMatrix[i, j] != null ? "+" : " ");
                }
                Console.WriteLine();
            }
            //
            Console.ReadLine();
        }

        private void SetupDbFeaturesImplementors(DtoMakerContext context)
        {
            context.DbTableFeaturesImplementors[typeof(SupportGetByAttribute).FullName] = new SupportGetByImplementation(context);
            context.DbTableFeaturesImplementors[typeof(SupportByDatesRangeSelectionAttribute).FullName] = new SupportByDatesRangeSelectionImplementor(context);
            context.DbTableFeaturesImplementors[typeof(SupportFilteredGetAttribute).FullName] = new SupportFilteredGetImplementor(context);
        }

        private void DumpEntities(DtoMakerContext context)
        {
            foreach (var entity in context.Model.Entities)
            {
                Console.WriteLine($"Entity[{entity.EntitySpace}.{entity.EntityName}]");
                foreach (var property in entity.Properties)
                {
                    string propertyTypeName = property.Type != null ? property.Type.Name : "";
                    Console.WriteLine($"\t{property.Name} : {propertyTypeName}");
                }
                Console.WriteLine("\tlinks:");
                foreach (var entityLink in entity.EntityLinks)
                {
                    Console.WriteLine($"\t\t{entityLink.ReferencingPropertySourcePropertyName} ==> {entityLink.ReferencedEntitySpace ?? "?"}.{entityLink.ReferencedEntityName ?? "?"}.{entityLink.ReferencedEntityPropertyName ?? "?"}");
                }
            }
        }

        protected abstract void SetupModel(DtoMakerContext context, ImportEntityFromDb importEntityFromDb, InjectEnum injectEnum);
    }

    public class DtoMakerImpl : DtoMakerBase
    {
        public DtoMakerImpl(string connectionString) : base(connectionString)
        {
        }

        protected override void SetupModel(DtoMakerContext context, ImportEntityFromDb importEntityFromDb, InjectEnum injectEnum)
        {
            //
            bool useEpsAssembly = true;
            if (useEpsAssembly)
            {
                context
                    // Set solution folder
                    .SetVariable("SOLUTION_ROOT", @"C:\Projects\Eps\src\Arm")
                    // Set Dto-project folder
                    .SetVariable("DTO_PROJECT_PATH", @"${SOLUTION_ROOT}$\Eps.Arm.Dal.Defs")
                    // Set Dal-project folder
                    .SetVariable("DAL_PROJECT_PATH", @"${SOLUTION_ROOT}$\Eps.Arm.Dal")
                    // Set Dto VS-project file path (generated dtos- and repo-interfaces files will be appended there).
                    .CsDtoProjectFilePath(@"${DTO_PROJECT_PATH}$\Eps.Arm.Dal.Defs.csproj")
                    // Set Dal VS-project file path (generated repo-implementations files will be appended there).
                    .CsDalProjectFilePath(@"${DAL_PROJECT_PATH}$\Eps.Arm.Dal.csproj")
                    // Dto project root namespace
                    .CsDtoRootNamespace("Eps.Arm.Dal.Core.Dto")
                    // Repositories interfaces root namespace
                    .CsRepoInterfacesRootNamespace("Eps.Arm.Dal.Core.AutoRepositories")
                    // Repositories implementations root namespace
                    .CsRepoImplsRootNamespace("Eps.Dal.AutoRepositories")
                    // Dto project folder where generated classes will be put
                    .CsDtoDestanationFolder(@"${DTO_PROJECT_PATH}$\Dto")
                    // Folders that will receive generated parts of repositories
                    .CsRepositoriesInterfacesDestanationFolder(@"${DTO_PROJECT_PATH}$\AutoRepositories")
                    .CsRepositoriesImplDestanationFolder(@"${DAL_PROJECT_PATH}$\AutoRepositories")
                    // Global base class for all repositories
                    .CsRepositoriesBaseClassName("Eps.Dal.Repositories.Base.RepositoryBase")
                    .CsRepositoryStaticImplementationUsings("log4net")
                    .CsRepositoryStaticImplementation("public $REPOSITORY_CLASS$(DataConnectionFactory.DataConnectionFactory dbFactory, ILog log) : base(dbFactory, log){}")
                    // Dal project fiel where linqtodb mapping will be inserted
                    .LinqToDbFluentMappingFilePath(@"${DAL_PROJECT_PATH}$\DataConnectionFactory\DataConnectionFactoryAuto.cs")
                    .DbSharedModelPath(@"D:\PrivateProjects\EPS\src\Eps.Builders\Eps.Db.Model\bin\Debug\_DbSharedModel.xml")
                    .CsRepositoryStaticImplementationUsings("log4net");
            }
            else
            {
                context
                    // Set solution folder
                    .SetVariable("SOLUTION_ROOT", @"D:\PrivateProjects\EPS\src\Eps.DtoMaker")
                    // Set Dto-project folder
                    .SetVariable("DTO_PROJECT_PATH", @"${SOLUTION_ROOT}$\TestProject.ForDtoReceive")
                    // Set Dal-project folder
                    .SetVariable("DAL_PROJECT_PATH", @"${SOLUTION_ROOT}$\TestProject.ForDtoReceive")
                    // Set Dto VS-project file path (generated files will be appended there).
                    .CsDtoProjectFilePath(@"${DTO_PROJECT_PATH}$\TestProject.ForDtoReceive.csproj")
                    // Dto project root namespace
                    .CsDtoRootNamespace("TestProject.ForDtoReceive.Dto.Autobuilt")
                    // Dto project folder where generated classes will be put
                    .CsDtoDestanationFolder(@"${DTO_PROJECT_PATH}$\Dto\Autobuilt")
                    // Dal project fiel where linqtodb mapping will be inserted
                    .LinqToDbFluentMappingFilePath(@"${DAL_PROJECT_PATH}$\DataConnectionFactoryAuto.cs");
            }

            context
                // Mapping dbSchema(s) to c# namespace(s)
                .EntitiesSpacesRenaming(new Dictionary<string, string>() { { "eps", "Eps" }, { "jp", "Jp" }, { "dicts", "Dicts" } } )
                // Manager that helps to converse singular word forms to plural and vice versa
                .LinguisticManager.AddWordsForms(new Dictionary<string, string>() { { "Address", "Addresses" }, { "Dictionary", "Dictionaries" } });

            // Experiment
            injectEnum
                .IsPublic()
                .Namespace("Enums")
                .Name("JuridicalPersonAccountTypes")
                    .AddMember("Red", 1)
                    .AddMember("Green")
                .Do(context);

            // Auth echema
            context.UseDbSchema("Auth");
            importEntityFromDb.IsPublic().Table("Users")/*.HasRawBaseClass("IUser<int>", "Microsoft.AspNet.Identity").HasRawMember("public int Id => UserId;")*/.WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Roles")/*.HasRawBaseClass("IRole<int>", "Microsoft.AspNet.Identity").HasRawMember("public int Id => RoleId;")*/.WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Groups").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Permissions").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Claims").WantsRepo().Do(context);
            // Auth entities relations
            importEntityFromDb.IsPublic().Table("UserRoles").Do(context);
            importEntityFromDb.IsPublic().Table("UserGroups").Do(context);
            importEntityFromDb.IsPublic().Table("GroupRoles").Do(context);
            importEntityFromDb.IsPublic().Table("RolePermissions").Do(context);
            importEntityFromDb.IsPublic().Table("PermissionClaims").Do(context);
            // User associated stuff
            importEntityFromDb.IsPublic().Table("UserSessions").WantsRepo().Do(context);
            //
            // Eps schema
            context.UseDbSchema("eps");
            importEntityFromDb.IsPublic().Table("JuridicalPersons").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("DocumentScans").WantsRepo().Do(context);
            //
            // Jp schema
            context.UseDbSchema("jp");
            importEntityFromDb.IsPublic().Table("Contractors").ColumnToProperty("ContractorId", "Id").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Accounts").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("BankRequisites").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Checks").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Contacts").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Documents").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("EmployeeAddresses").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("EmployeeContacts").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("EmployeeDocuments").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("EmployeeDocumentScans").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Employees").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Infos").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("BankProducts").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Contracts").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("GoodsCategories").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("StateRegistrations").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("StateRegAddresses").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("StateRegOkveds").WantsRepo().Do(context);

            // Dicts schema
            context.UseDbSchema("dicts");
            importEntityFromDb.IsPublic().Table("ApprovementStatuses").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("BankDepartments").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("BankProducts").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("BusinessAreas").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("DocumentsStoringPlaces").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("GoodsCategories").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("JuridicalPersonAbonentsNotificationWayTypes").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("JuridicalPersonAccountTypes").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("JuridicalPersonAddressTypes").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("JuridicalPersonCheckTypes").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("JuridicalPersonContactTypes").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("JuridicalPersonContractStateTypes").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("JuridicalPersonDocumentTypes").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("EmployeeContactTypes").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("EmployeeDocumentTypes").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("JuridicalPersonFinancialStates").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Okopfs").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("Regions").WantsRepo().Do(context);
            importEntityFromDb.IsPublic().Table("ContactEmployeeTypes").WantsRepo().Do(context);
            // importEntityFromDb.IsPublic().Table("PaymentServiceParams").Do(context);
            // importEntityFromDb.IsPublic().Table("PaymentServices").Do(context);

        }
    }
}
