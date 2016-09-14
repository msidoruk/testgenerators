using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Actions.Base;

namespace EPS.DtoMaker.Engine.Actions
{
    public class TransformModelToCSharpFiles : BaseAction<TransformModelToCSharpFiles>
    {
        public TransformModelToCSharpFiles(DtoMakerContext context)
            : base(context)
        {
        }

        public override TransformModelToCSharpFiles Do(DtoMakerContext context)
        {
            base.Do(context);

            return this;
        }
    }
}
