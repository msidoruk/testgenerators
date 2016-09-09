using System;
using System.Collections.Generic;
using EPS.DtoMaker.Engine.Actions.Base;
using EPS.DtoMaker.Engine.Impl;

namespace EPS.DtoMaker.Engine.Actions.InjectEnums
{
    public class InjectEnum : BaseAction<InjectEnum>
    {
        private string _namespace;
        private string _name;
        private List<object> _members = new List<object>();

        public InjectEnum(DtoMakerContext context)
            : base(context)
        {
        }

        public ModelEntity InjectedModelEntity { get; set; }

        public InjectEnum Namespace(string namespaceName)
        {
            _namespace = namespaceName;
            return this;
        }
        public InjectEnum Name(string name)
        {
            _name = name;
            return this;
        }

        public InjectEnum AddMember(string memberName, object value = null)
        {
            _members.Add(new Tuple<string, object>(memberName, value));
            return this;
        }

        protected override void ResetFieldsToDefaults()
        {
            _name = string.Empty;
            //
            base.ResetFieldsToDefaults();
        }

        public override InjectEnum Do(DtoMakerContext context)
        {
            ActionsApplicationHistoryItem currentActionsApplicationHistoryItem = context.CreateActionsApplicationHistoryItem(new Dictionary<object, object>(Marks));
            //
            string entitySpace = context.FindUpstairsMark<string>(currentActionsApplicationHistoryItem, DtoProjectComposer.Markers.CsDtoRootNamespace, m => true).Item2;
            //
            ModelEntity modelEntity = context.Model.Entities.AppendEntity(context, currentActionsApplicationHistoryItem);
            modelEntity.Stereotypes.Add(ModelEntity.StereotypesNames.IsEnum, null);
            modelEntity.Stereotypes.Add(ModelEntity.StereotypesNames.NotDb, null);
            modelEntity.EntitySpace = _namespace;
            modelEntity.EntityName = _name;
            foreach (Tuple<string, object> member in _members)
            {
                modelEntity.Instances.AppendInstance(new Dictionary<string, object>() { {"MemberName", member.Item1}, {"MemberValue", member.Item2} });
            }
            InjectedModelEntity = modelEntity;
            //
            base.Do(context);
            //
            return this;
        }
    }
}
