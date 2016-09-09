using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Actions.BuildDtos;
using EPS.DtoMaker.Engine.Helpers;

namespace EPS.DtoMaker.Engine.Impl
{
    public class ModelCsDetails
    {
        private readonly DtoMakerContext _context;

        private readonly StringBuilder _loadErrors = new StringBuilder();

        public bool IsValid { get; private set; }
        public string LoadErrors => _loadErrors.ToString();

        public string CsDtoProjectFilePath;
        public string CsDtoRootFolder;
        public string CsDtoRootNamespace;
        public Dictionary<string, string> EntitiesSpacesRenamingMap;

        public string CsDalProjectFilePath;
        public string CsRepoInterfacesRootNamespace;
        public string CsRepoImplsRootNamespace;
        public string CsRepositoriesInterfacesDestanationFolder;
        public string CsRepositoriesImplDestanationFolder;
        public string CsRepositoriesBaseClassName;
        public string CsRepositoryStaticImplementation;
        public string CsRepositoryStaticImplementationUsings;

        public ModelCsDetails(DtoMakerContext context)
        {
            _context = context;
            IsValid = false;

            LoadFromContext();
        }

        private void LoadFromContext()
        {
            _loadErrors.Clear();

            CsDtoProjectFilePath = _context.FindMark<string>(DtoProjectComposer.Markers.CsDtoProjectFile, m => true).Untuple(()=>_loadErrors.AppendLine("CsDtoProjectFile not found"));
            CsDtoRootFolder = _context.FindMark<string>(DtoProjectComposer.Markers.CsDtoDestanationFolder, m => true).Untuple(() => _loadErrors.AppendLine("CsDtoDestanationFolder not found"));
            CsDtoRootNamespace = _context.FindMark<string>(DtoProjectComposer.Markers.CsDtoRootNamespace, m => true).Untuple(() => _loadErrors.AppendLine("CsDtoRootNamespace not found"));
            EntitiesSpacesRenamingMap = _context.FindMark<Dictionary<string, string>>(BuildDtos.Markers.CsEntitiesSpacesRenaming, m => true).Untuple(() => { });

            CsDalProjectFilePath = _context.FindMark<string>(DtoProjectComposer.Markers.CsDalProjectFile, m => true).Untuple(() => _loadErrors.AppendLine("CsDalProjectFile not found"));
            CsRepoInterfacesRootNamespace = _context.FindMark<string>(DtoProjectComposer.Markers.CsRepoInterfacesRootNamespace, m => true).Untuple(() => _loadErrors.AppendLine("CsRepoInterfacesRootNamespace not found"));
            CsRepoImplsRootNamespace = _context.FindMark<string>(DtoProjectComposer.Markers.CsRepoImplsRootNamespace, m => true).Untuple(() => _loadErrors.AppendLine("CsRepoImplsRootNamespace not found"));
            CsRepositoriesInterfacesDestanationFolder = _context.FindMark<string>(DtoProjectComposer.Markers.CsRepositoriesInterfacesDestanationFolder, m => true).Untuple(() => _loadErrors.AppendLine("CsRepositoriesInterfacesDestanationFolder not found"));
            CsRepositoriesImplDestanationFolder = _context.FindMark<string>(DtoProjectComposer.Markers.CsRepositoriesImplDestanationFolder, m => true).Untuple(() => _loadErrors.AppendLine("CsRepositoriesImplDestanationFolder not found"));
            CsRepositoriesBaseClassName = _context.FindMark<string>(DtoProjectComposer.Markers.CsRepositoriesBaseClassName, m => true).Untuple(() => _loadErrors.AppendLine("CsRepositoriesBaseClassName not found"));
            CsRepositoryStaticImplementation = _context.FindMark<string>(DtoProjectComposer.Markers.CsRepositoryStaticImplementation, m => true).Untuple(() => { });
            CsRepositoryStaticImplementationUsings = _context.FindMark<string>(DtoProjectComposer.Markers.CsRepositoryStaticImplementationUsings, m => true).Untuple(() => { });

            IsValid = _loadErrors.Length == 0;
        }
    }
}
