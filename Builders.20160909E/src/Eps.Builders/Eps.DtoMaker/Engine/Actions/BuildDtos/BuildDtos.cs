using System;
using System.Collections.Generic;
using Autofac;
using EPS.DtoMaker.Engine.Actions.Base;
using EPS.DtoMaker.Engine.Base;

namespace EPS.DtoMaker.Engine.Actions.BuildDtos
{
    public class BuildDtos : BaseAction<BuildDtos>
    {
        public BuildDtos(DtoMakerContext context)
            : base(context)
        {
        }

        public class Markers
        {
            public const string WantsRepo = "buildDtos:wantsRepository";
            public const string IsPublic = "buildDtos:doPublish";
            public const string InlinedEnum = "buildDtos:inlinedEnum";
            public const string CsRootNamespace = "buildDtos:cs-root-namespace";
            public const string CsRootFolder = "buildDtos:cs-root-folder";
            public const string CsEntitiesSpacesRenaming = "buildDtos:entity-space-renaming";
        }

        public override BuildDtos Do(DtoMakerContext context)
        {
            IDtoProjectComposer dtoProjectComposer =
                (IDtoProjectComposer) context.DiContainer.Resolve(typeof (IDtoProjectComposer));
            dtoProjectComposer.BindToContext(context);
            dtoProjectComposer.Init();
            foreach (var entity in context.Model.Entities)
            {
                //
                if (context.IsMarked(entity.ActionsApplicationHistoryItem, Markers.IsPublic))
                    dtoProjectComposer.RegisterEntity(entity);
            }
            dtoProjectComposer.Done();
            //
            base.Do(context);
            //
            return this;
        }

        public static string IssueMarkForDto()
        {
            return Markers.IsPublic;
        }
    }

    public static class Extentions
    {
        public static T IsPublic<T>(this T action) where T : BaseAction<T>
        {
            action.Mark(BuildDtos.Markers.IsPublic);
            return action;
        }

        public static T WantsRepo<T>(this T action) where T : BaseAction<T>
        {
            action.Mark(BuildDtos.Markers.WantsRepo);
            return action;
        }

        public static DtoMakerContext EntitiesSpacesRenaming(this DtoMakerContext dtoMakerContext, Dictionary<string, string> renamingMap)
        {
            dtoMakerContext.AppendMark(BuildDtos.Markers.CsEntitiesSpacesRenaming,
                                        new Dictionary<string, string>(renamingMap,
                                            StringComparer.InvariantCultureIgnoreCase /*db schemas are case-insensitive*/));
            return dtoMakerContext;
        }
    }

}
