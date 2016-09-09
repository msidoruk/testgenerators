using System;

namespace Eps.CsToDb.Meta.TablesMarkers.Base
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class TraitAttribute : Attribute
    {
        public Type TraitType { get; private set; }

        public TraitAttribute(Type traitType)
        {
            TraitType = traitType;
        }
    }
}
