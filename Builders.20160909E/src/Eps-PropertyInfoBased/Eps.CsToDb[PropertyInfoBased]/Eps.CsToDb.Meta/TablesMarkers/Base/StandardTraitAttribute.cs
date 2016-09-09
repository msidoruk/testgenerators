using System;

namespace Eps.CsToDb.Meta.TablesMarkers.Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class StandardTraitAttribute : Attribute
    {
        public Type TraitType { get; private set; }

        public StandardTraitAttribute(Type traitType)
        {
            TraitType = traitType;
        }
    }
}
