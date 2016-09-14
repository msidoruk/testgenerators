using System.Globalization;
using Autofac;
using Eps.CsToDb.Builder.Impl;
using Eps.CsToDb.Builder.Impl.Collections;
using log4net;
using System.Data.Entity.Design.PluralizationServices;

namespace Eps.CsToDb.Builder.Base
{
    public class BuilderContext
    {
        public IContainer Container { get; private set; }
        public ILog Logger { get; private set; }
        public string DbModelPath { get; set; }
        public string ResultPath { get; set; }
        public DbModelAssembliesCollection DbModelAssemblies { get; private set; }
        public DbModelTypesCollection DbModelTypes { get; private set; }
        public DbModel.DbModel DbModel { get; set; }

        public PluralizationService PluralizationService { get; private set; }
        public ICsToSqlTypesMapper CsToSqlTypesMapper { get; set; }
        public string DbSharedModelFileName { get; set; }

        public BuilderContext(IContainer container, ILog logger)
        {
            Container = container;
            Logger = logger;
            DbModelAssemblies = new DbModelAssembliesCollection();
            DbModelTypes = new DbModelTypesCollection();
            PluralizationService = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));
            CsToSqlTypesMapper = new CsToSqlTypesMapper(this);
            DbSharedModelFileName = "_DbSharedModel.xml";
        }
    }
}
