using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Eps.CsToDb.Builder;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Builder.DbModel;
using Eps.CsToDb.Builder.Impl;
using log4net;
using log4net.Config;

namespace Eps.CsToDb
{
    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.Configure();
            //
            ILog log = LogManager.GetLogger(typeof(Program));
            log.Debug("Logger test");
            //
            ContainerBuilder containerBuilder = new ContainerBuilder();
            //
            containerBuilder.RegisterType(typeof (DbModelAssembliesResolver)).As<IDbModelAssembliesResolver>();
            containerBuilder.RegisterType(typeof(DbModelBuilder)).As<IDbModelBuilder>();
            containerBuilder.RegisterType(typeof(DbScriptsBuilder)).As<IDbScriptsBuilder>();
            containerBuilder.RegisterType(typeof (DbScriptsWriter)).As<IDbScriptsWriter>();
            //
            IContainer container = containerBuilder.Build();
            //
            BuilderContext context = new BuilderContext(container, log);
            context.DbModelPath = @".";
            context.ResultPath = context.DbModelPath;
            //
            Builder.Builder.Build(context);
        }
    }
}
