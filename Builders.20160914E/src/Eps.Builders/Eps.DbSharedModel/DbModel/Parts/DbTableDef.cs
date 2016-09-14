using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Eps.DbFeatures.Attributes;

namespace Eps.DbSharedModel.DbModel.Parts
{
    [Serializable]
    public class DbTableDef
    {
        private string _schema;
        private string _dbTableName;

        public DbTableDef()
        {
            TypedFeatures = new List<DbTableFeatureAttribute>();
        }

        [XmlAttribute("schema")]
        public string TableSchema
        {
            get { return _schema; }
            set { _schema = value; }
        }

        [XmlAttribute("name")]
        public string TableName {
            get { return _dbTableName; }
            set { _dbTableName = value; }
        }

        [XmlArray("DbColumns")]
        [XmlArrayItem("DbColumn")]
        public List<DbColumnDef> Columns { get; set; }

        [XmlArray("DbTableFeatures")]
        [XmlArrayItem("DbTableFeature")]
        public List<string> Features { get; set; }

        [XmlIgnore]
        public List<DbTableFeatureAttribute> TypedFeatures { get; set; }
    }
}
