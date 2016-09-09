using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPS.DtoMaker.Engine.Base
{
    interface IDtoProjectComposer
    {
        void BindToContext(DtoMakerContext context);
        void RegisterEntity(ModelEntity modelEntity);
        void Done();
        void Init();
    }
}
