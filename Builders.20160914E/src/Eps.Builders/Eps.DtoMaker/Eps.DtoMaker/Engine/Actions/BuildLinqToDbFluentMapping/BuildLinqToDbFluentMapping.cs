using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using EPS.DtoMaker.Engine.Actions.Base;
using EPS.DtoMaker.Engine.Impl;

namespace EPS.DtoMaker.Engine.Actions.BuildLinqToDbFluentMapping
{
    public class BuildLinqToDbFluentMapping : BaseAction<BuildLinqToDbFluentMapping>
    {
        public BuildLinqToDbFluentMapping(DtoMakerContext context) : base(context)
        {
        }

        public override BuildLinqToDbFluentMapping Do(DtoMakerContext context)
        {
            // Get file path
            var linqToDbFluentMappingFilePathResult = context.FindMark<string>(Markers.LinqToDbFluentMappingFilePath, m => true);
            if (!linqToDbFluentMappingFilePathResult.Item1)
                throw new Exception($"Can't determine LinqToDbFluentMappingFilePath");
            string linqToDbFluentMappingFilePath = linqToDbFluentMappingFilePathResult.Item2;
            //
            if (!File.Exists(linqToDbFluentMappingFilePath))
                throw new Exception($"Cannot find file '{linqToDbFluentMappingFilePath}' where fluent mapping must be written.");
            //
            StringBuilder fileBody = new StringBuilder();
            string generatedLinesPrefix = string.Empty;
            //
            StringBuilder fileHeader = new StringBuilder();
            StringBuilder fileFooter = new StringBuilder();
            using (System.IO.StreamReader linqToDbFluentMappingFileStream = new System.IO.StreamReader(linqToDbFluentMappingFilePath))
            {
                string line;
                while ((line = linqToDbFluentMappingFileStream.ReadLine()) != null)
                {
                    fileHeader.AppendLine(line);
                    if (line.Contains("$AUTO_CODE_BEGIN"))
                    {
                        generatedLinesPrefix = string.Concat(line.TakeWhile(Char.IsWhiteSpace));
                        break;
                    }
                }
                bool enableAppendToFileFooter = false;
                while ((line = linqToDbFluentMappingFileStream.ReadLine()) != null)
                {
                    if (enableAppendToFileFooter)
                        fileFooter.AppendLine(line);
                    //
                    if (line.Contains("$AUTO_CODE_END"))
                    {
                        fileFooter.AppendLine(line);
                        enableAppendToFileFooter = true;
                    }
                }
            }
            //
            if (fileHeader.Length == 0 || fileFooter.Length == 0)
                throw new Exception($"File {linqToDbFluentMappingFilePath} was not propertly processed (header or footer (or both) is empty). Code will not be generated. Use $AUTO_CODE_BEGIN' and '$AUTO_CODE_END markers.");
            //
            BuildBody(context, fileBody, generatedLinesPrefix.ToString());
            //
            string fileBackupCopyPath = linqToDbFluentMappingFilePath + "~";
            File.Copy(linqToDbFluentMappingFilePath, fileBackupCopyPath, true);
            //
            using (StreamWriter resultFile = File.CreateText(linqToDbFluentMappingFilePath))
            {
                resultFile.Write(fileHeader.ToString());
                resultFile.WriteLine($"{generatedLinesPrefix}// !!!Don't edit anything between $+'AUTO_CODE_BEGIN' and $+'AUTO_CODE_END' these changes will be lost after next generation with DtoMaker.");
                resultFile.Write(fileBody.ToString());
                resultFile.Write(fileFooter.ToString());
            }
            //
            base.Do(context);
            //
            return this;
        }

        private void BuildBody(DtoMakerContext context, StringBuilder fileBody, string linesPrefix)
        {
            foreach (var modelEntity in context.Model.Entities)
            {
                // We put in linqtodb mapping only db entities.
                if (modelEntity.Stereotypes.ContainsKey(ModelEntity.StereotypesNames.NotDb))
                    continue;
                string modelEntityCsName = modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoShortTypeName];
                fileBody.AppendLine($"{linesPrefix}mappingBuilder.Entity<{modelEntity.AlternativeNames[DtoProjectComposer.ModelNames.CsDtoNamespace]}.{modelEntityCsName}>()");
                fileBody.AppendLine($"{linesPrefix}\t.HasSchemaName(\"{modelEntity.EntitySpace}\")");
                fileBody.AppendLine($"{linesPrefix}\t.HasTableName(\"{modelEntity.EntityName}\")");
                Dictionary<string, string> identityPropertiesToSkip = new Dictionary<string, string>();
                foreach (var identityProperty in modelEntity.IdentityProperties)
                {
                    string originalIdentityPropertyName = modelEntity.OriginalPropertiesNames.FindPropertyName(identityProperty);
                    fileBody.AppendLine($"{linesPrefix}\t.Property(s=>s.{originalIdentityPropertyName}).IsPrimaryKey().IsIdentity()");
                    identityPropertiesToSkip[originalIdentityPropertyName] = null;
                }
                foreach (var property in modelEntity.Properties)
                {
                    if (identityPropertiesToSkip.ContainsKey(property.Name))
                        continue;
                    string propertyCsName = property.AlternativeNames[DtoProjectComposer.ModelNames.CsProperyName];
                    fileBody.AppendLine($"{linesPrefix}\t.Property(s=>s.{propertyCsName})");
                }
                foreach (var entityLink in modelEntity.EntityLinks)
                {
                    string linkPropertyCsName = entityLink.AlternativeNames[DtoProjectComposer.ModelNames.CsLinkPropertyName];
                    string associationThisKey = entityLink.ReferencedEntityPropertyName;
                    string associationOtherKey = entityLink.ReferencedModelEntity.IdentityPropertyName();
                    fileBody.AppendLine($"{linesPrefix}\t.Property(s=>s.{linkPropertyCsName}).HasAttribute(new AssociationAttribute{{ThisKey=\"{associationThisKey}\", OtherKey=\"{associationOtherKey}\"}})");
                }
                fileBody.AppendLine($"{linesPrefix}\t; /*End*/");
                fileBody.AppendLine($"");
            }
        }

        public class Markers
        {
            public const string LinqToDbFluentMappingFilePath = "linqToDbFluentMapping:mapping-file-path";
        }
    }

    public static class Extentions
    {
        public static DtoMakerContext LinqToDbFluentMappingFilePath(this DtoMakerContext dtoMakerContext, string linqToDbFluentMappingFilePath)
        {
            dtoMakerContext.AppendMark(BuildLinqToDbFluentMapping.Markers.LinqToDbFluentMappingFilePath, linqToDbFluentMappingFilePath);
            return dtoMakerContext;
        }
    }
}
