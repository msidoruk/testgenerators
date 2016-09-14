using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using EPS.DtoMaker.Engine.Actions;
using EPS.DtoMaker.Engine.Actions.Base;
using System.Data.Entity.Design.PluralizationServices;
using System.Deployment.Internal;
using System.Xml;
using Eps.DbSharedModel.DbModel;
using EPS.DtoMaker.Engine.Actions.BuildDtos;
using EPS.DtoMaker.Engine.Base;
using EPS.DtoMaker.Engine.Impl;

namespace EPS.DtoMaker.Engine
{
    public class ActionsApplicationHistoryItem : DtoMakerContext.IMarkableItem
    {
        private DtoMakerContext _context;
        public ActionsApplicationHistoryItem(DtoMakerContext context, Dictionary<object, object> marks)
        {
            _context = context;
            //
            Marks = new Dictionary<object, object>(marks);
        }
        public ActionsApplicationHistoryItem PreviousItem { get; set; }

        Dictionary<object, object> DtoMakerContext.IMarkableItem.Marks
        {
            get { return Marks; }
        }

        private Dictionary<object, object> Marks { get; set; }
    }

    public class WrittenFileInfo
    {
        public string FilePath { get; set; }
    }

    public class DtoMakerContext
    {
        internal interface IMarkableItem
        {
            Dictionary<object, object> Marks { get; }
        }

        public List<ActionsApplicationHistoryItem> ActionsApplicationHistory { get; private set; }

        public IContainer DiContainer { get; private set; }
        public DtoMakerModel Model { get; private set; }
        public LinguisticManager LinguisticManager { get; set; }
        public WrittenFilesRegistry WrittenFilesRegistry { get; set; }

        private Dictionary<string, string> _variables = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private string _dbSharedModelPath;

        public DbSharedModel DbSharedModel { get; private set; }
        public Dictionary<string, IDbTableFeatureImplementor> DbTableFeaturesImplementors { get; set; }

        public DtoMakerContext(IContainer diContainer)
        {
            DiContainer = diContainer;
            Model = new DtoMakerModel(this);
            LinguisticManager = new LinguisticManager(this);
            WrittenFilesRegistry = new WrittenFilesRegistry();
            ActionsApplicationHistory = new List<ActionsApplicationHistoryItem>();
            DbSharedModel = new DbSharedModel();
            DbTableFeaturesImplementors = new Dictionary<string, IDbTableFeatureImplementor>();
        }
 
        public void AppendMark(string mark, object value = null)
        {
            ActionsApplicationHistory.Add(new ActionsApplicationHistoryItem(this, new Dictionary<object, object>() {{mark, value}}) { PreviousItem = ActionsApplicationHistory.Any() ? ActionsApplicationHistory.Last() : null });
        }

        public void AppendMarks(Dictionary<object, object> marks)
        {
            ActionsApplicationHistory.Add(new ActionsApplicationHistoryItem(this, new Dictionary<object, object>(marks)) { PreviousItem = ActionsApplicationHistory.Any() ? ActionsApplicationHistory.Last() : null });
        }

        public ActionsApplicationHistoryItem CreateActionsApplicationHistoryItem(Dictionary<object, object> marks)
        {
            ActionsApplicationHistoryItem actionsApplicationHistoryItem = new ActionsApplicationHistoryItem(this, marks) { PreviousItem = ActionsApplicationHistory.Any() ? ActionsApplicationHistory.Last() : null };
            ActionsApplicationHistory.Add(actionsApplicationHistoryItem);
            return actionsApplicationHistoryItem;
        }

        public Tuple<bool,T> FindUpstairsMark<T>(ActionsApplicationHistoryItem startItem, object markName, Func<object, bool> func)
        {
            for (ActionsApplicationHistoryItem item = startItem; item != null; item = item.PreviousItem)
            {
                object markValue;
                if (((IMarkableItem)item).Marks.TryGetValue(markName, out markValue) && (markValue is T) && func(markValue))
                {
                    if (markValue is string)
                    {
                        markValue = SubstitudeVariables((string)markValue);
                    }
                    return Tuple.Create(true, (T) markValue);
                }
            }
            return Tuple.Create(false, default(T));
        }

        private const string BeginTag = "${";
        private const string EndTag = "}$";

        private string SubstitudeVariables(string value)
        {
            string currentValue = value;
            if (!string.IsNullOrEmpty(currentValue))
            {
                while (true)
                {
                    int index1 = currentValue.IndexOf(BeginTag);
                    int index2 = currentValue.LastIndexOf(EndTag);
                    if (index1 < 0 && index2 < 0)
                        break;
                    if (index1 < 0 || index2 < 0)
                        throw new Exception($"Not closed variable tag in: {value}");
                    string variableName = currentValue.Substring(index1 + BeginTag.Length, index2 - BeginTag.Length);
                    if (string.IsNullOrEmpty(variableName))
                        throw new Exception($"Variable name is empty in: {value}");
                    string variableValue;
                    if (!_variables.TryGetValue(variableName, out variableValue))
                        throw new Exception($"Undefined variable '{variableName}' referenced in: {value}");
                    currentValue = currentValue.Substring(0, index1) + variableValue + currentValue.Substring(index2 + EndTag.Length);
                }
            }
            return currentValue;
        }

        public Tuple<bool, T> FindMark<T>(object markName, Func<object, bool> func)
        {
            foreach (ActionsApplicationHistoryItem applicationHistoryItem in ActionsApplicationHistory)
            {
                object markValue;
                if (((IMarkableItem)applicationHistoryItem).Marks.TryGetValue(markName, out markValue) && (markValue is T) && func(markValue))
                {
                    if (markValue is string)
                    {
                        markValue = SubstitudeVariables((string)markValue);
                    }
                    return Tuple.Create(true, (T) markValue);
                }
            }
            return Tuple.Create(false, default(T));
        }

        public DtoMakerContext SetVariable(string name, string value)
        {
            _variables[name] = value;
            return this;
        }

        public bool IsMarked(ActionsApplicationHistoryItem actionsApplicationHistoryItem, string mark)
        {
            return ((IMarkableItem)actionsApplicationHistoryItem).Marks.ContainsKey(mark);
        }

        public DtoMakerContext DbSharedModelPath(string dbSharedModelPath)
        {
            _dbSharedModelPath = dbSharedModelPath;
            using (var modelXmlReader = new XmlTextReader(_dbSharedModelPath))
            {
                DbSharedModel = DbSharedModel.Read(modelXmlReader);
            }
            return this;
        }

    }

    public class WrittenFilesRegistry
    {
        public Dictionary<string, List<WrittenFileInfo>> WrittenFiles { get; private set; }

        public WrittenFilesRegistry()
        {
            WrittenFiles = new Dictionary<string, List<WrittenFileInfo>>();
        }

        public void RegisterWrittenFilesCategory(string writtenFilesCategory)
        {
            WrittenFiles[writtenFilesCategory] = new List<WrittenFileInfo>();
        }

        public void RegisterWrittenFile(string writtenFileCategory, string filePath)
        {
            WrittenFiles[writtenFileCategory].Add(new WrittenFileInfo() { FilePath = filePath });
        }

        public IEnumerable<WrittenFileInfo> GetCategoryWrittenFiles(string writtenFilesCategory)
        {
            List<WrittenFileInfo> result;
            WrittenFiles.TryGetValue(writtenFilesCategory, out result);
            if (result == null)
                result = new List<WrittenFileInfo>();
            return result;
        }
    }

    public class LinguisticManager
    {
        private DtoMakerContext _dtoMakerContext;

        private readonly Dictionary<string, string> _singularToPlural = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private readonly Dictionary<string, string> _pluralToSingular = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        private PluralizationService _pluralizationService = PluralizationService.CreateService(CultureInfo.GetCultureInfo("en-us"));

        public LinguisticManager(DtoMakerContext dtoMakerContext)
        {
            _dtoMakerContext = dtoMakerContext;
        }

        public DtoMakerContext AddWordForm(string singular, string plural)
        {
            _singularToPlural[singular] = plural;
            _pluralToSingular[plural] = singular;
            //
            return _dtoMakerContext;
        }

        public DtoMakerContext AddWordsForms(Dictionary<string, string> wordsForms)
        {
            foreach (var wordForm in wordsForms)
                AddWordForm(wordForm.Key, wordForm.Value);
            return _dtoMakerContext;
        }
        public string ToSingular(string plural)
        {
            string singular;
            if (!_pluralToSingular.TryGetValue(plural, out singular))
                singular = _pluralizationService.Singularize(plural);
            else
                singular = plural;
            return CapitalizeBySourceTemplate(plural, singular);
        }

        public string ToPlural(string singular)
        {
            string plural;
            if (!_pluralToSingular.TryGetValue(singular, out plural))
                plural = _pluralizationService.Pluralize(singular);
            else
                plural = singular;
            return CapitalizeBySourceTemplate(singular, plural);
        }
        private string CapitalizeBySourceTemplate(string capitalizationTemplate, string value)
        {
            if (!string.IsNullOrEmpty(capitalizationTemplate))
            {
                bool[] capitalization = new bool[capitalizationTemplate.Length];
                int templateCharIndex = 0;
                foreach (var capitalizationTemplateChar in capitalizationTemplate)
                    capitalization[templateCharIndex++] = Char.IsUpper(capitalizationTemplateChar);
                //
                StringBuilder resultStringBuilder = new StringBuilder();
                int valueCharIndex = 0;
                bool isCapital = false;
                foreach (var valueChar in value)
                {
                    if (valueCharIndex < capitalization.Length || valueChar == 0)
                        isCapital = capitalization[valueCharIndex++];
                    resultStringBuilder.Append(isCapital ? Char.ToUpper(valueChar) : Char.ToLower(valueChar));
                }
                return resultStringBuilder.ToString();
            }
            return value;
        }
    }
}
