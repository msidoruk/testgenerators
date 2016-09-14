using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Eps.DbFeatures.Attributes;
using Eps.DbSharedModel.DbModel.Parts;

namespace Eps.DbSharedModel.DbModel
{
    public class DbSharedModel
    {
        [XmlElement("DbTable")]
        public List<DbTableDef> DbTables { get; set; }

        public DbSharedModel()
        {
            DbTables = new List<DbTableDef>();
        }

        public void Write(XmlTextWriter modelXmlWriter)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(GetType());
            xmlSerializer.Serialize(modelXmlWriter, this);
        }

        public static DbSharedModel Read(XmlTextReader modelXmlReader)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(DbSharedModel));
            DbSharedModel restoredDbSharedModel = (DbSharedModel)xmlSerializer.Deserialize(modelXmlReader);
            foreach (var dbTableDef in restoredDbSharedModel.DbTables)
            {
                foreach (var feature in dbTableDef.Features)
                {
                    DbTableFeatureAttribute dbTableFeature = DbTableFeatureAttribute.Deserialize(feature);
                    dbTableDef.TypedFeatures.Add(dbTableFeature);
                }
            }
            return restoredDbSharedModel;
        }
    }
}
