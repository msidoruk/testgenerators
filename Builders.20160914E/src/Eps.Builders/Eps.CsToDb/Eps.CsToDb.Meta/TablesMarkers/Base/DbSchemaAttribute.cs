using System;

namespace Eps.CsToDb.Meta.TablesMarkers.Base
{
    public class DbSchemaAttribute : Attribute
    {
        public string Name { get; private set; }
        public DbSchemaAttribute(string dbSchemaName = null)
        {
            Name = dbSchemaName;
        }
    }
}
