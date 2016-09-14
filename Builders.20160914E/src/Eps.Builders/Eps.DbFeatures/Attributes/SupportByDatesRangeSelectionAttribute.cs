using System;
using System.Xml.Serialization;

namespace Eps.DbFeatures.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    [XmlRoot("SupportByDatesRangeSelection")]
    public class SupportByDatesRangeSelectionAttribute : DbTableFeatureAttribute
    {
        public SupportByDatesRangeSelectionAttribute()
        {
        }

        public SupportByDatesRangeSelectionAttribute(string activationDateColumnName, string endDateColumnName)
        {
            ActivationDateColumnName = activationDateColumnName;
            EndDateColumnName = endDateColumnName;
        }

        [XmlAttribute("ActivationDateColumnName")]
        public string ActivationDateColumnName { get; set; }
        [XmlAttribute("EndDateColumnName")]
        public string EndDateColumnName { get; set; }
    }
}
