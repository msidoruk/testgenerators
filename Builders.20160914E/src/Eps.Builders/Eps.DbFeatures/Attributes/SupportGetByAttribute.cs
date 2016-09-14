using System;
using System.Xml.Serialization;

namespace Eps.DbFeatures.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [XmlRoot("SupportGetByAttribute")]
    public class SupportGetByAttribute : DbTableFeatureAttribute
    {
        public SupportGetByAttribute()
        {
        }

        public SupportGetByAttribute(string columnName)
        {
            ColumnName = columnName;
        }

        [XmlAttribute("columnName")]
        public string ColumnName { get; set; }
    }
}
