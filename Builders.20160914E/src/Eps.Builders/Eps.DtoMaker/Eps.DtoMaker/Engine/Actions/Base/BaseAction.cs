using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS.DtoMaker.Engine.Actions.Base
{
    public class BaseAction<T> where T : BaseAction<T>
    {
        protected Dictionary<object, object> Marks { get; }

        public DtoMakerContext Context { get; private set; }

        protected BaseAction(DtoMakerContext context)
        {
            Context = context;
            Marks = new Dictionary<object, object>();
        }

        public virtual T Mark(object name, object value = null)
        {
            Marks[name] = value;
            //
            return (T) this;
        }

        public virtual T Do(DtoMakerContext context)
        {
            ResetFieldsToDefaults();
            //
            return (T) this;
        }

        protected virtual void ResetFieldsToDefaults()
        {
            Marks.Clear();
        }
    }
}
