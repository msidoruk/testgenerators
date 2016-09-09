using System;

namespace Eps.CsToDb.Meta.TablesMarkers.Base
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableAttribute : Attribute
    {
        public string DbTableName { get; set; }
    }
}
