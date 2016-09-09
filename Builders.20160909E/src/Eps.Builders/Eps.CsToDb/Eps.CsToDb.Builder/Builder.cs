using System;
using Autofac;
using Eps.CsToDb.Builder.Base;
using Eps.CsToDb.Meta;

namespace Eps.CsToDb.Builder
{
    public class Builder
    {
        public static void Build(BuilderContext context)
        {
            ReadDbModelParts(context);
            //
            IDbModelBuilder dbModelBuilder = context.Container.Resolve<IDbModelBuilder>();
            dbModelBuilder.Build(context);
            //
            IDbScriptsBuilder dbScriptsBuilder = context.Container.Resolve<IDbScriptsBuilder>();
            dbScriptsBuilder.Build(context);
            //
            IDbScriptsWriter dbScriptsWriter = context.Container.Resolve<IDbScriptsWriter>();
            dbScriptsWriter.Write(context);
        }

        private static void ReadDbModelParts(BuilderContext context)
        {
            IDbModelAssembliesResolver dbModelAssembliesResolver = context.Container.Resolve<IDbModelAssembliesResolver>();
            dbModelAssembliesResolver.Resolve(context.DbModelPath, (assembly) =>
            {
                context.Logger.Debug($"Assembly '{assembly.FullName}' will be included to modeling");
                context.DbModelAssemblies.AddAssembly(assembly);
                Type[] assemblyTypes = assembly.GetTypes();
                foreach (var assemblyType in assemblyTypes)
                {
                    object[] attributes = assemblyType.GetCustomAttributes(typeof(DbModelPartAttribute), false);
                    DbModelPartAttribute attribute = (attributes.Length > 0) ? attributes[0] as DbModelPartAttribute : null;
                    if (attribute != null)
                    {
                        context.Logger.Debug($"DbModelType '{assemblyType.FullName}' will be included to modeling");
                        context.DbModelTypes.AddType(assemblyType);
                    }
                }
                return true;
            });
        }
    }
}
