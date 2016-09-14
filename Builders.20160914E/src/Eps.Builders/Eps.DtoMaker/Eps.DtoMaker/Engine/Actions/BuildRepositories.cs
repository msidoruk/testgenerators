using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPS.DtoMaker.Engine.Actions.Base;

namespace EPS.DtoMaker.Engine.Actions
{
    public class BuildRepositories : BaseAction<BuildRepositories>
    {
        public BuildRepositories(DtoMakerContext context) : base(context)
        {
        }

        public override BuildRepositories Do(DtoMakerContext context)
        {
            base.Do(context);
            //
            return this;
        }
    }
}
