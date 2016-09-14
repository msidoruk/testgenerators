using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Eps.DbFeatures.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [XmlRoot("SupportFilteredGetAttribute")]
    public class SupportFilteredGetAttribute : DbTableFeatureAttribute
    {
        public SupportFilteredGetAttribute()
        {
        }

        public SupportFilteredGetAttribute(Type filterTypeName)
        {
            FilterTypeName = filterTypeName.AssemblyQualifiedName;
        }

        [XmlAttribute("filterTypeName")]
        public string FilterTypeName { get; set; }
    }
}
